using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BedtimeCore.BuildPipeline;
#if BEDTIME_VERSIONING
using BedtimeCore.Versioning;
#endif
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
#pragma warning disable 4014

namespace BedtimeCore.SteamUploader
{
	internal class SteamUploader : IBuildProcessor
	{
		private TaskCompletionSource<SteamResult> _tcs;
		private int _progressId;
		
		public const string UPLOAD_COMPLETE_METADATA = "SteamUploadComplete";
		public const string UPLOAD_FAILED_METADATA = "SteamUploadFailed";

		public bool IsBusy => _tcs != null;

		public enum SteamResult
		{
			Success,
			InvalidCredentials,
			MissingSDK,
			SteamGuard,
			Failure,
		}
		
		public BuildStep ValidBuildSteps => BuildStep.PostBuild;

		public async Task<bool> ProcessBuild(Build build)
		{
			await Upload(build);
			return true;
		}

		private void Report(string description, float t)
		{
			if (Application.isBatchMode)
			{
				Log(description);
			}
			Progress.Report(_progressId, t, description);
		}

		private void Finish(Progress.Status status) => Progress.Finish(_progressId, status);

		private SteamResult Fail(SteamResult result, Build build)
		{
			build.Metadata.Add(UPLOAD_FAILED_METADATA);
			LogError(result.ToString());
			Finish(Progress.Status.Failed);
			_tcs = null;
			return result;
		}
		
		public async Task<SteamResult> Upload(Build build)
		{
			if (_tcs != null)
			{
				return await _tcs.Task;
			}
			_tcs = new TaskCompletionSource<SteamResult>();
			
			_progressId = Progress.Start("Steam Uploader", "Starting Steam Uploader", Progress.Options.Sticky);
			Progress.SetTimeDisplayMode(_progressId, Progress.TimeDisplayMode.ShowRunningTime);
			Progress.ShowDetails(false);
			Report("Starting Steam Uploader", 0.1f);
			
			if (await RetrieveSDK(build) is var steamCmd && steamCmd == null)
			{
				return Fail(SteamResult.MissingSDK, build);
			}
			
			var vdfPath = await WriteVDF(build);
			var credentials = GetCredentials(build);
			
			if (!credentials.HasValue)
			{
				return Fail(SteamResult.InvalidCredentials, build);
			}
			
			Report("Launching SteamCMD", 0.5f);
			var args = $"+login {credentials.Value.login} {credentials.Value.password} +run_app_build \"{vdfPath}\" +quit";
			(int exitCode, string output) processResult = await RunProcessAsync(steamCmd.FullName, args);
			Report("Finished SteamCMD", 0.8f);
			if (processResult.output.Contains("Steam Guard"))
			{
				if (Application.isBatchMode)
				{
					return Fail(SteamResult.SteamGuard, build);
				}
				
				Report("Steam Guard", 0.8f);
				var steamGuardCode = await SteamGuardWindow.ShowWindow();
				args = $"+set_steam_guard_code {steamGuardCode} +login {credentials.Value.login} {credentials.Value.password} +run_app_build \"{vdfPath}\" +quit";
				processResult = await RunProcessAsync(steamCmd.FullName, args);
			}
			
			var result = SteamResult.Failure;

			var success = processResult.exitCode == 0;
		
			var dateTime = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
			if (success)
			{
				result = SteamResult.Success;
				Report($"Upload Complete [{dateTime}]",1f);
			}
			else
			{
				LogError($"Upload Failed. SteamCMD exited with an exit code = {processResult.exitCode}. [{dateTime}]");
			}
			Finish(success ? Progress.Status.Succeeded : Progress.Status.Failed);
			
			_tcs = null;
		
			build.Metadata.Add(UPLOAD_COMPLETE_METADATA);
			return result;
		}
		
		Task<FileInfo> RetrieveSDK(Build build)
		{
			if (!Secrets.TryGetValue(build.Configuration.BuildSettings.Steam.SDKPath.Value, out var sdkPath))
			{
				var log = $"Missing Steam SDK path secret";
				Report(log, 0.2f);
				Finish(Progress.Status.Failed);
				return Task.FromResult<FileInfo>(null);
			}

			var steamCmd = new FileInfo($"{sdkPath}/tools/ContentBuilder/builder/steamcmd.exe");
			if (!steamCmd.Exists)
			{
				var log = $"Failed to find Steam SDK at {sdkPath}";
				Report(log, 0.2f);
				Finish(Progress.Status.Failed);
				return Task.FromResult<FileInfo>(null);
			}		
				
			return Task.FromResult(steamCmd);
		}
		
		async Task<string> WriteVDF(Build build)
		{
			var config = build.Configuration;
			var buildOutput = new DirectoryInfo($"{Application.temporaryCachePath}/Steam");
			buildOutput.Create();
		
			var contentRoot = new DirectoryInfo($"{build.OutputDirectory}"); 
			contentRoot.Create();

			var appID = config.BuildSettings.Steam.AppID;
			var depotID = config.BuildSettings.Steam.DepotID;
			var setLiveBranch = config.BuildSettings.Steam.SetLiveOnBranch;
			
			var dateTime = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
			var description = dateTime;
			
#if BEDTIME_VERSIONING
			var revision = RepositoryRevision.FromGitDirectory().Revision;
			if (revision != null)
			{
				description = $"[{dateTime}] [v{config.BuildSettings.version.Value}] [{revision.ShortIdentifier}] ({revision.Message})";
			}
#endif	
			
			var vdf = new VDF
			{
				BuildOutput = $"{buildOutput.FullName}",
				ContentRoot = $"{contentRoot.FullName}",
				AppID = appID.Value,
				Depot = new Depot(depotID.Value, "*", "."),
				Preview = false,
				Description = description,
				SetLiveBranch = setLiveBranch.Value,
			};

			var vdfPath = $"{buildOutput.FullName}/build.vdf";
			Report("Writing Upload VDF", 0.25f);
			Log($"VDF:\n{vdf}");

			var vdfString = vdf.ToString();
			#if UNITY_2021_2_OR_NEWER
			await File.WriteAllTextAsync(vdfPath, vdfString);
			#else
			byte[] encodedText = Encoding.Unicode.GetBytes(vdfString);
			using (var sourceStream = new FileStream(vdfPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
			{
				await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
			};
			#endif
			
			return vdfPath;
		}
		
		(string login, string password)? GetCredentials(Build build)
		{
			var login = build.Configuration.BuildSettings.Steam.Login;
			var password = build.Configuration.BuildSettings.Steam.Password;

			if (!Secrets.TryGetValue(login.Value, out var user) || !Secrets.TryGetValue(password.Value, out var pass))
			{
				var log = $"Missing Steam login information!";
				LogError(log);
				Report(log, 0.3f);
				Finish(Progress.Status.Failed);
				return null;
			}

			return (user, pass);
		}
		
		private void Log(string s) => Debug.Log($"[Steam Uploader] {s}");
		private void LogError(string s) => Debug.LogError($"[Steam Uploader] {s}");

		private Task<(int exitCode, string output)> RunProcessAsync(string fileName, string args)
		{
			var process = new System.Diagnostics.Process
			{
				StartInfo =
				{
					FileName = fileName,
					Arguments = args,
					UseShellExecute = false,
					CreateNoWindow = true,
					RedirectStandardOutput = true,
					RedirectStandardInput = true,
				},
				EnableRaisingEvents = true
			};
			{
				return RunProcessAsync(process);
			}
		}    
		
		private Task<(int exitCode, string output)> RunProcessAsync(System.Diagnostics.Process process)
		{
			try
			{
				var tcs = new TaskCompletionSource<(int, string)>();
				var sb = new StringBuilder();

				process.OutputDataReceived += (_, ea) => sb.AppendLine(ea.Data);
				process.Exited += (_, __) =>
				{
					var output = sb.ToString();
					Log(output);
					tcs.SetResult((process.ExitCode, output));
					process.Dispose();
				};
				
				if (!process.Start())
				{
					throw new InvalidOperationException("Could not start process: " + process);
				}

				process.StandardInput.AutoFlush = true;
				process.StandardInput.WriteLine("");
				process.BeginOutputReadLine();

				return tcs.Task;
			}
			catch (Exception e)
			{
				LogError($"{e}");
				throw;
			}
		}
	}
}
