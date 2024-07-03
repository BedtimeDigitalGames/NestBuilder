using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;
#if BEDTIME_ASSET_SYSTEM
using BedtimeCore.Assets;
#endif
#if BEDTIME_VERSIONING
using BedtimeCore.Versioning;
#endif

namespace BedtimeCore.NestBuilder
{
	[Serializable]
	public class PathSetting : BuildSetting<string>
	{
		public const string STREAMING_ASSETS_PATH = "$STREAMING_ASSETS_PATH$";
		public const string CONFIGURATION_NAME = "$CONFIGURATION_NAME$";
		public const string PROJECT_PATH = "$PROJECT_PATH$";
		public const string WORKSPACE_PATH = "$WORKSPACE_PATH$";
		public const string ASSETS_PATH = "$ASSETS_PATH$";
		public const string PRODUCT_NAME = "$PRODUCT_NAME$";
		public const string COMPANY_NAME = "$COMPANY_NAME$";
		public const string PLATFORM = "$PLATFORM$";
		public const string FILE_TYPE = "$FILE_TYPE$";
		public const string DATE = "$DATE$";
		public const string VERSION = "$VERSION$";
		public const string GIT_INDEX = "$GIT_INDEX$";
		public const string GIT_HASH = "$GIT_HASH$";
		public const string ASSETBUNDLES_PATH = "$ASSETBUNDLES_PATH$";
		public const string ASSETBUNDLES_BUILD_ROOT = "$ASSETBUNDLES_BUILD_ROOT$";
		
		private const string DATE_FORMAT = "yyyy-MM-dd";

#if BEDTIME_VERSIONING
		private static Revision revision;
		private static double lastRevisionFetch;
		private Revision CurrentRevision
		{
			get
			{
				const int TIME_BETWEEN_FETCH = 5;
				var time = EditorApplication.timeSinceStartup;
				if (time > lastRevisionFetch + TIME_BETWEEN_FETCH)
				{
					lastRevisionFetch = time;
					revision = RepositoryRevision.FromGitDirectory().Revision;
				}

				return revision;
			}
		}
#endif
		public override string Value
		{
			get
			{
				if (string.IsNullOrEmpty(value))
				{
					return string.Empty;
				}

				return value;
			}
		}

		private string GetExtension(BuildConfiguration environment)
		{
			const string FALLBACK_EXTENSION = "invalid";

			var platform = environment.BuildSettings.Main.Platform;
			var target = platform.Value.buildTarget;

			switch (target)
			{
				case BuildTarget.StandaloneOSX:
					return "app";
				case BuildTarget.StandaloneWindows64:
				case BuildTarget.StandaloneWindows:
					return "exe";
				case BuildTarget.StandaloneLinux64:
					return "x86_64";
				case BuildTarget.Android:
					if (environment.BuildSettings.Android.buildAppBundle)
                    {
						return "aab";
                    }
                    else
                    {
						return "apk";
					}
				case BuildTarget.WebGL:
					return "html";
				case BuildTarget.Switch:
					return "nsp";
				default:
					return FALLBACK_EXTENSION;
			}
		}

		public PathSetting(string defaultPath, Action<string> applyAction = null) : base(applyAction)
		{
			this.value = defaultPath;
		}

		public override void DrawValue(BuildConfiguration environment, SerializedProperty property)
		{
			if (GUI.enabled)
			{
				var builder = new StringBuilder();
				builder.AppendLine($"{CONFIGURATION_NAME} : Name of the Configuration");
				builder.AppendLine($"{PROJECT_PATH} : Root of the project");
				builder.AppendLine($"{WORKSPACE_PATH} : Commandline overridable, otherwise the same as ${nameof(PROJECT_PATH)}$");
				builder.AppendLine($"{ASSETS_PATH} : Assets folder");
				builder.AppendLine($"{STREAMING_ASSETS_PATH} : StreamingAssets folder");
				builder.AppendLine($"{PRODUCT_NAME} : Name of the game");
				builder.AppendLine($"{COMPANY_NAME} : Name of the company");
				builder.AppendLine($"{PLATFORM} : BuildTarget name");
				builder.AppendLine($"{FILE_TYPE} : Automatic file type");
				builder.AppendLine($"{VERSION} : Build version");
				builder.AppendLine($"{GIT_INDEX} : Git revision index");
				builder.AppendLine($"{GIT_HASH} : Git revision hash");
				builder.AppendLine($"{ASSETBUNDLES_PATH} : Runtime path to AssetBundles");
				builder.AppendLine($"{ASSETBUNDLES_BUILD_ROOT} : Editor path to where AssetBundles for all platforms are stored");
				
				builder.Append($"{DATE} : Current date in ISO 8601 format");

				EditorGUILayout.HelpBox(builder.ToString(), MessageType.Info);
			}


			value = EditorGUILayout.TextField(value, GUILayout.Height(20));
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			var label = ParsePath(value, environment, true);
			EditorGUILayout.LabelField(label, RichText);
			EditorGUILayout.EndVertical();
		}

		public string ParsePath(string path, BuildConfiguration environment, bool richColor = false)
		{
			path = path ?? "";
			Func<string, string> enrich = x =>
			{
				if (richColor)
				{
					return $"{(GUI.enabled ? "<color=yellow>" : "<color=green>")}{x}</color>";
				}
				else
				{
					return x;
				}
			};

			if (environment == null)
			{
				environment = Owner;
			}

			var platform = environment.BuildSettings.Main.Platform;
			var companyName = environment.BuildSettings.Application.CompanyName;
			var productName = environment.BuildSettings.Application.ProductName;
			var version = environment.BuildSettings.Application.Version;

			if (platform != null)
			{
				path = path.Replace(PLATFORM, enrich(platform.Value.buildTarget.ToString()));
				path = path.Replace(FILE_TYPE, enrich(GetExtension(environment)));
			}

			if (companyName != null)
			{
				path = path.Replace(COMPANY_NAME, enrich(companyName.Value));
			}

			if (productName != null)
			{
				path = path.Replace(PRODUCT_NAME, enrich(productName.Value));
			}

			if (version != null)
			{
				path = path.Replace(VERSION, enrich(version.Value));
			}


			path = path.Replace(CONFIGURATION_NAME, enrich(environment.name));
			path = path.Replace(ASSETS_PATH, enrich(Application.dataPath));
			path = path.Replace(STREAMING_ASSETS_PATH, enrich(Application.streamingAssetsPath));
#if BEDTIME_ASSET_SYSTEM
			path = path.Replace(ASSETBUNDLES_PATH, enrich(AssetManager.AssetBundlesRuntimePath));
			path = path.Replace(ASSETBUNDLES_BUILD_ROOT, enrich(EditorAssetManager.AssetBundleBuildPathRoot));
#endif
			
#if BEDTIME_VERSIONING
			if (CurrentRevision != null)
			{
				path = path.Replace(GIT_INDEX, enrich(CurrentRevision.Index));
				path = path.Replace(GIT_HASH, enrich(CurrentRevision.ShortIdentifier));
			}
#endif
			string projectPath = Path.GetDirectoryName(Application.dataPath);
			string workspacePath = projectPath;
#if BEDTIME_COMMANDLINE
			if (!BedtimeCore.Utility.CommandLineArgumentUtility.TryGetValueAfterArgument<string>(Builder.WorkspacePath, out workspacePath))
			{
				workspacePath = projectPath;
			}
#endif
			path = path.Replace(PROJECT_PATH, enrich(projectPath));
			path = path.Replace(WORKSPACE_PATH, enrich(workspacePath));
			path = path.Replace(DATE, enrich(DateTime.UtcNow.ToString(DATE_FORMAT)));
			return path;
		}

		private string AbsoluteToRelative(string absolutePath)
		{
			Uri pathUri = new Uri(absolutePath);
			string folder = Environment.CurrentDirectory;
			if (!folder.EndsWith(Path.DirectorySeparatorChar.ToString()))
			{
				folder += Path.DirectorySeparatorChar;
			}
			Uri folderUri = new Uri(folder);
			return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
		}

		private string RelativeToAbsolute(string relativePath)
		{
			return Path.GetFullPath(relativePath);
		}

		public enum PathType
		{
			SelectFile,
			SelectFolder,
			SaveFile,
		}

		private static GUIStyle richText = null;
		private static GUIStyle RichText
		{
			get
			{
				if (richText == null)
				{
					richText = new GUIStyle(EditorStyles.whiteMiniLabel);
					richText.richText = true;
				}
				return richText;
			}
		}
	}
}
