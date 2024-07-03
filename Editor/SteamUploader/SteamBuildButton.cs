using BedtimeCore.BuildPipeline;
using UnityEditor;
using UnityEngine;
#pragma warning disable 4014

namespace BedtimeCore.SteamUploader
{
	public class SteamBuildButton : IBuildQueueExtension
	{
		private SteamUploader _uploader = new SteamUploader();
		
		public void OnGUI(Build build)
		{
            var buttonText = "Upload to Steam (" + build.Configuration.BuildSettings.Steam.SetLiveOnBranch.Value + ")";
			if (_uploader.IsBusy)
			{
				buttonText = "Please Wait";
				GUI.enabled = false;
			}
			if(build.Metadata.Contains(SteamUploader.UPLOAD_COMPLETE_METADATA))
			{
				GUI.enabled = false;
				buttonText = "Upload Complete (" + build.Configuration.BuildSettings.Steam.SetLiveOnBranch.Value + ")";
			}
			if(build.Metadata.Contains(SteamUploader.UPLOAD_FAILED_METADATA))
			{
				GUI.enabled = true;
				buttonText = "Upload Failed.. Retry?";
			}
			if (GUILayout.Button(buttonText))
			{
				_uploader.Upload(build);
			}
			GUI.enabled = true;
		}

		public bool ShouldDisplay(Build build)
		{
			if (build.BuildStep != BuildStep.Completed)
			{
				return false;
			}

			var settings = build.Configuration.BuildSettings;

			if (settings.Platform.Value.AsBuildTargetGroup != BuildTargetGroup.Standalone)
			{
				return false;
			}
			
			var login = settings.Steam.Login;
			var pass = settings.Steam.Password;
			var appID = settings.Steam.AppID;
			var depotID = settings.Steam.DepotID;
			var sdkPath = settings.Steam.SDKPath;

			if (!login.IsSet || !pass.IsSet || !appID.IsSet || !depotID.IsSet || !sdkPath.IsSet)
			{
				return false;
			}
			
			return true;
		}
	}
}