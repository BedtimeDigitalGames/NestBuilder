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
            var buttonText = "Upload to Steam (" + build.Configuration.BuildSettings.steamSetLiveOnBranch.Value + ")";
			if (_uploader.IsBusy)
			{
				buttonText = "Please Wait";
				GUI.enabled = false;
			}
			if(build.Metadata.Contains(SteamUploader.UPLOAD_COMPLETE_METADATA))
			{
				GUI.enabled = false;
				buttonText = "Upload Complete (" + build.Configuration.BuildSettings.steamSetLiveOnBranch.Value + ")";
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

			if (settings.platform.Value.AsBuildTargetGroup != BuildTargetGroup.Standalone)
			{
				return false;
			}
			
			var login = settings.steamLogin;
			var pass = settings.steamPassword;
			var appID = settings.steamAppID;
			var depotID = settings.steamDepotID;
			var sdkPath = settings.steamSDKPath;

			if (!login.IsSet || !pass.IsSet || !appID.IsSet || !depotID.IsSet || !sdkPath.IsSet)
			{
				return false;
			}
			
			return true;
		}
	}
}