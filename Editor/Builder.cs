using System.Collections.Generic;
using UnityEditor;
using System;
using UnityEngine;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
#if BEDTIME_VERSIONING
using BedtimeCore.Versioning.Editor;
#endif
using Unity.EditorCoroutines.Editor;
using UnityEditor.Build.Reporting;
using Debug = System.Diagnostics.Debug;
#if BEDTIME_ASSET_SYSTEM
using BedtimeCore.Assets;
#endif

#pragma warning disable 4014

namespace BedtimeCore.BuildPipeline
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public static class Builder
	{
		public const string BuildPresetDefaultPath = "EditorResources/Editor/BuildPipeline/Configurations/";
		public const string WorkspacePath = "-workspacepath";
		
		private const string Presets = "-presets";
		private const string PresetsPath = "-presetpath";

		private static BuildQueue _buildQueue;
		
		private static bool IsBatchMode => Application.isBatchMode;

		/// <summary>
		/// The Build Queue which is processed by the Builder.
		/// </summary>
		public static BuildQueue BuildQueue
		{
			get
			{
				if (_buildQueue == null)
				{
					_buildQueue = BuildQueue.GetAsset();
				}
				return _buildQueue;
			}
		}


		public static bool IsBuilding => BuildQueue.Count > 0 && !_buildQueue.IsPaused;
		
		private static bool _isQuitting;
		private static bool _isProcessingBuild;

		/// <summary>
		/// Try to create, enqueue, and process one or more builds based on command line arguments.
		/// </summary>
		public static void Build()
		{
#if BEDTIME_COMMANDLINE
			if (BedtimeCore.Utility.CommandLineArgumentUtility.ContainsArgument(Presets))
			{
				foreach (var preset in BedtimeCore.Utility.CommandLineArgumentUtility.GetValuesUntilDelimiter(Presets))
				{
					string presetsPath = BuildPresetDefaultPath;

					if (BedtimeCore.Utility.CommandLineArgumentUtility.ContainsArgument(PresetsPath))
					{
						BedtimeCore.Utility.CommandLineArgumentUtility.TryGetValueAfterArgument(PresetsPath, out presetsPath);
					}

					List<BuildConfiguration> configurations = GetBuildConfigurations(presetsPath);

					BuildConfiguration configuration = configurations.FirstOrDefault(c => c.name.Equals(preset, StringComparison.InvariantCultureIgnoreCase));

					if (configuration == null)
					{
						$"Could not find preset {preset} in {presetsPath}".LogError();
						continue;
					}
					
					EnqueueBuild(configuration);
				}
			}
#endif
			if (IsBatchMode && BuildQueue.Count == 0)
			{
				"No builds were queued!".LogError();
				EndBuildProcess(1);
			}
		}

		/// <summary>
		/// Create and enqueue a new build. The build will be processed as the build-queue is processed.
		/// </summary>
		/// <param name="configuration">The build Configuration to create a build from</param>
		/// <param name="modifiers">Modifies the build</param>
		/// <returns>Returns whether the build could successfully be created and enqueued</returns>
		public static bool EnqueueBuild(BuildConfiguration configuration, BuildOptions modifiers = BuildOptions.None)
		{
			var illegalSettings = configuration.GetIllegalSettings().ToArray();
			if (illegalSettings.Length > 0)
			{
				string illegals = illegalSettings[0].Name;
				for (int i = 1; i < illegalSettings.Length; i++)
				{
					illegals += ", " + illegalSettings[i].Name;
				}

				$"Skipping build \"{configuration.name}\"! \nThe following settings are not properly configured: {illegals}".LogError();
				return false;
			}

			Build build;
			try
			{
				build = new Build(configuration, GetBuildPlayerOptions(configuration, modifiers));
			}
			catch (Exception e)
			{
				e.ToString().LogError();
				return false;
			}
			
			if (!IsBatchMode)
			{
				EditorWindow.GetWindow(typeof(BuildQueueWindow));
			}
			BuildQueue.EnqueueBuild(build);
			return true;
		}

		private static async Task BuildInternal(Build build)
		{
			try
			{
				SuppressCallStackLog(true);
				switch (build.BuildStep)
				{
					case BuildStep.Ready:
						await SetBuildStep(build, BuildStep.PreConfiguration);
						$"Location: \"{build.PlayerOptions.locationPathName}\"".Log();
						$"Target: {ObjectNames.NicifyVariableName(build.PlayerOptions.target.ToString())}".Log();
						break;
					case BuildStep.PreConfiguration:
						if (!build.Configuration.ApplyToEditor())
						{
							throw new OperationCanceledException("Failed to apply configurations");
						}
						if (EditorApplication.isCompiling)
						{
							"Compiling... Pausing build temporarily".Log();
							return;
						}
						await SetBuildStep(build, BuildStep.PostConfiguration);
						break;
					case BuildStep.PostConfiguration:
						LogAllSettings();
#if BEDTIME_ASSET_SYSTEM
						if (build.Configuration.BuildSettings.assetBundlesCopy.Value.copyAssetBundles)
						{
							build.AssetBundlesStatus = AssetBundlesStatus.Check;
							await SetBuildStep(build, BuildStep.AddAssetBundles);
						}
						else
#endif
						{
							await SetBuildStep(build, BuildStep.PreBuild);
						}
						break;
					case BuildStep.AddAssetBundles:
						if (AddAssetBundles(build))
						{
							await SetBuildStep(build, BuildStep.PreBuild);
						}
						break;
					case BuildStep.PreBuild:
						await SetBuildStep(build, BuildStep.Building);
						break;
					case BuildStep.Building:
						ParseBuildOutput(UnityEditor.BuildPipeline.BuildPlayer(build.PlayerOptions.BuildPlayerOptions));
						await SetBuildStep(build, BuildStep.PostBuild);
						break;
					case BuildStep.PostBuild:
						await SetBuildStep(build, BuildStep.Completed);
						break;
					case BuildStep.Failed:
					case BuildStep.Completed:
					case BuildStep.Unavailable:
						DequeueBuild(build);
						break;
				}
			}
			catch (Exception e)
			{
				CleanUpTemporaryFiles(build);
				SetBuildStep(build, BuildStep.Failed);
				e.ToString().LogError();
			}
		}

		private static bool AddAssetBundles(Build build)
		{
#if BEDTIME_ASSET_SYSTEM
			const string extManifest = ".manifest";
			const string ignoreName = "_ignore";

			AssetBundleBuildInfo assetBundles = build.Configuration.BuildSettings.assetBundlesCopy.Value;

			if (assetBundles.buildAssetBundles && build.AssetBundlesStatus == AssetBundlesStatus.Check)
			{
				EditorAssetManager.BuildAllBundleMappings();
				var options = assetBundles.buildAssetBundleOptions;
				EditorAssetManager.BuildAllAssetBundles(build.PlayerOptions.target, options);
				build.AssetBundlesStatus = AssetBundlesStatus.Built;
				return false; //wait a frame before importing
			}

			var source = assetBundles.AssetBundlesSource;
			var destination = assetBundles.AssetBundlesDestination;

			foreach (string newPath in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
			{
				if (Path.GetExtension(newPath).Equals(extManifest) || Path.GetFileName(newPath) == ignoreName)
				{
					continue;
				}

				var output = newPath.Replace(source, destination);
				
				var includes = assetBundles.includeManifest;
				if (includes != null && !includes.Includes(output))
				{
					continue;
				}
				
				var fileInfo = new FileInfo(output);
				Debug.Assert(fileInfo.Directory != null, "fileInfo.Directory != null");
				Directory.CreateDirectory(fileInfo.Directory.FullName);
				File.Copy(newPath, output, true);
			}
			
			AssetLookup.BuildGUIDMap();
			AssetDatabase.Refresh();
#endif
			build.AssetBundlesStatus = AssetBundlesStatus.Copied;
			return true;
		}

		private static void CleanUpAssetBundles(Build build)
		{
#if BEDTIME_ASSET_SYSTEM
			var assetBundles = build.Configuration.BuildSettings.assetBundlesCopy.Value;
			if (assetBundles.copyAssetBundles)
			{
				var destination = assetBundles.AssetBundlesDestination;
				if (Directory.Exists(destination))
				{
					FileUtil.DeleteFileOrDirectory(destination);
				}
			}
#endif
		}

		private static async Task SetBuildStep(Build build, BuildStep buildStep)
		{
			build.BuildStep = buildStep;

			var settings = build.Configuration.BuildSettings;
			var processorSetting = buildStep switch
			{
				BuildStep.PreConfiguration => settings.BuildSteps.PreConfigurationStep, 
				BuildStep.PostConfiguration => settings.BuildSteps.PostConfigurationStep,
				BuildStep.PreBuild => settings.BuildSteps.PreBuildStep,
				BuildStep.PostBuild => settings.BuildSteps.PostBuildStep,
				_ => null,
			};

			if (processorSetting?.Value != null)
			{
				foreach (IBuildProcessor buildProcessor in processorSetting.Value)
				{
					if (buildProcessor == null)
					{
						continue;
					}
					
					bool result = await buildProcessor.ProcessBuild(build);
					if (!result)
					{
						throw new InvalidOperationException($"Build Processor {buildProcessor} failed!");
					}
				}
			}
		}

		private static void ParseBuildOutput(BuildReport output)
		{
			$"Total warnings: {output.summary.totalWarnings}".Log();
			$"Total errors: {output.summary.totalErrors}".Log();
			$"Build result: {output.summary.result}".Log();

			switch (output.summary.result)
			{
				case BuildResult.Unknown:
					throw new InvalidOperationException($"Build result unknown!");
				case BuildResult.Failed:
					throw new InvalidOperationException($"Build failed!");
				case BuildResult.Cancelled:
					throw new InvalidOperationException($"Build cancelled!");
			}

			$"Build location: {output.summary.outputPath}".Log();
			$"Total build size: {output.summary.totalSize}".Log();

			if (output.strippingInfo != null && output.strippingInfo.includedModules != null)
			{
				foreach (var module in output.strippingInfo.includedModules)
				{
					$"Included module: {module}".Log();
				}
			}
		}

		private static BuildOptions GetBuildOptions(BuildConfiguration configuration)
		{
			BuildOptions buildOptions = BuildOptions.None;

			var settings = configuration.BuildSettings;

			var development = settings.Debugging.IsDevelopment.Value;
			var allowDebugging = settings.Debugging.AllowDebugging.Value;
			var autoConnectProfiler = settings.Debugging.AutoConnectProfiler.Value;
			var buildScriptsOnly = settings.Debugging.BuildScriptsOnly.Value;
			var platform = settings.Platform.Value;

			buildOptions |= BuildOptions.DetailedBuildReport;
			
			if (development)
			{
				buildOptions |= BuildOptions.Development;
			}
			if (allowDebugging && development)
			{
				buildOptions |= BuildOptions.AllowDebugging;
			}
			if (autoConnectProfiler && (development || platform.buildTarget == BuildTarget.WSAPlayer))
			{
				buildOptions |= BuildOptions.ConnectWithProfiler;
			}
			if (buildScriptsOnly)
			{
				buildOptions |= BuildOptions.BuildScriptsOnly;
			}

			return buildOptions;
		}

		private static BuildPlayerOptions GetBuildPlayerOptions(BuildConfiguration configuration, BuildOptions modifiers)
		{
			var playerOptions = new BuildPlayerOptions();
			var settings = configuration.BuildSettings;
			var platform = settings.Platform;
			var scenes = settings.Scenes;
			var locationSetting = settings.LocationPathName.GetCascaded();
			
			if (settings == null || !platform.IsSet || !scenes.IsSet || !locationSetting.IsSet)
			{
				throw new ArgumentException("Settings related to PlayerBuildOptions must be set!");
			}
			
			var location = (locationSetting as PathSetting)?.ParsePath(locationSetting.Value, configuration);
			var buildOptions = GetBuildOptions(configuration);

			playerOptions.target = platform.Value.buildTarget;
			playerOptions.targetGroup = platform.Value.AsBuildTargetGroup;
			playerOptions.scenes = scenes.Value;
			playerOptions.options = buildOptions | modifiers;

			playerOptions.locationPathName = location;
			return playerOptions;
		}

		private static IEnumerator ExecutePendingBuilds()
		{
			const int waitFrames = 10;
			
			while (!_isQuitting)
			{
				for (var i = 0; i < waitFrames; i++)
				{
					yield return null;
				}
				
				if(!_isProcessingBuild)
				{
					ProcessInternal();
				}
			}
			
			async ValueTask ProcessInternal()
			{
				if (EditorApplication.isCompiling || BuildQueue.IsPaused)
				{
					return;
				}
			
				_isProcessingBuild = true;
				var activeBuild = BuildQueue.ActiveBuild;
				if (activeBuild != null)
				{
					await BuildInternal(activeBuild);
				}
				_isProcessingBuild = false;
			}
		}

		private static void CleanUpTemporaryFiles(Build build) => CleanUpAssetBundles(build);

		private static void DequeueBuild(Build build)
		{
			CleanUpTemporaryFiles(build);
			BuildQueue.DequeueBuild(build);

			if (IsBatchMode && build is { BuildStep: BuildStep.Failed })
			{
				$"Halted build process. Failed to build {build.Configuration.name}".LogError();
				EndBuildProcess(1);
			}
			else if (BuildQueue.Count == 0)
			{
				"No more builds in queue".Log();
				EndBuildProcess();
			}
		}
		private static void EndBuildProcess(int status = 0)
		{
			$"Build process is ending... Quitting: {IsBatchMode}".Log();
			SuppressCallStackLog(false);
			if (IsBatchMode)
			{
				$"Exiting with status code {status}".Log();
				_isQuitting = true;
				EditorApplication.delayCall += () => EditorApplication.Exit(status);
			}
		}

		private static List<BuildConfiguration> GetBuildConfigurations(string path)
		{
			string[] fileEntries = Directory.GetFiles(Application.dataPath + "/" + path);

			var configurations = new List<BuildConfiguration>();
			foreach (var file in fileEntries)
			{
				var localPath = file.Replace(Application.dataPath, "Assets");
				var asset = AssetDatabase.LoadMainAssetAtPath(localPath);
				if (asset is BuildConfiguration)
				{
					configurations.Add(asset as BuildConfiguration);
				}
			}
			return configurations;
		}

		private static void LogAllSettings()
		{
			var FLAGS = BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty;
			var types = new[] { typeof(PlayerSettings), typeof(EditorUserBuildSettings) }.Concat(typeof(PlayerSettings).GetNestedTypes(BindingFlags.Public));

			foreach (var type in types)
			{
				if (!type.IsSealed)
				{
					continue;
				}
				foreach (var property in type.GetProperties(FLAGS))
				{
					if (!property.CanRead || property.GetIndexParameters().Length > 0)
					{
						continue;
					}
					try
					{
						var value = property.GetValue(null, null);
						if (value != null)
						{
							$"DEBUG: <{ObjectNames.NicifyVariableName(type.Name).ToUpper()}> {property.Name} = {value}".Log();

							if (property.PropertyType != typeof(string) && value is IEnumerable enumerable)
							{
								int i = 0;
								foreach (var item in enumerable)
								{
									$"{property.Name}[{i++}] {(item != null ? item.ToString() : "NULL")}".Log();
								}
							}
						}
					}
					catch (Exception)
					{
						// ignored
					}
				}
			}
		}

		[InitializeOnLoadMethod]
		private static void InitializeOnLoad()
		{
			try
			{
				EditorCoroutineUtility.StartCoroutineOwnerless(ExecutePendingBuilds());
			}
			catch (Exception e)
			{
				$"Builder failed to check for resuming builds. Reason: {e}".Log();
			}
		}

		private static void SuppressCallStackLog(bool suppressed)
		{
			var logType = suppressed ? StackTraceLogType.None : StackTraceLogType.Full;
			Application.SetStackTraceLogType(LogType.Warning, logType);
			Application.SetStackTraceLogType(LogType.Log, logType);
			Application.SetStackTraceLogType(LogType.Assert, logType);
			Application.SetStackTraceLogType(LogType.Error, logType);
			Application.SetStackTraceLogType(LogType.Exception, logType);
		}

#if !BEDTIME_LOGGING
		private static void Log(this string message) => UnityEngine.Debug.Log($"[{nameof(Builder)}] {message}");
		private static void LogError(this string message) => UnityEngine.Debug.LogError($"[{nameof(Builder)}] {message}");
#endif
	}
}