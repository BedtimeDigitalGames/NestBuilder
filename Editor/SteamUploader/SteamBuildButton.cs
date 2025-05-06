using BedtimeCore.NestBuilder;
using UnityEditor;
using UnityEngine;
#pragma warning disable 4014

namespace BedtimeCore.SteamUploader
{
	public class SteamBuildButton : IBuildQueueExtension
	{
		private SteamUploader _uploader = new SteamUploader();
        private string _branchNameOverride = string.Empty;
		
		public void OnGUI(Build build)
        {
            var buildBranch = build.Configuration.BuildSettings.Steam.SetLiveOnBranch.Value;
            if (!string.IsNullOrEmpty(_branchNameOverride))
            {
                buildBranch = _branchNameOverride;
            }
            
            var buttonText = $"Upload to Steam ({buildBranch})";
			if (_uploader.IsBusy)
			{
				buttonText = "Please Wait";
				GUI.enabled = false;
			}
			if(build.Metadata.Contains(SteamUploader.UPLOAD_COMPLETE_METADATA))
			{
				GUI.enabled = false;
				buttonText = $"Upload Complete ({buildBranch})";
			}
			if(build.Metadata.Contains(SteamUploader.UPLOAD_FAILED_METADATA))
			{
				GUI.enabled = true;
				buttonText = "Upload Failed.. Retry?";
			}

            EditorGUILayout.BeginHorizontal("box");
            if (GUILayout.Button(buttonText))
            {
                _uploader.Upload(build, buildBranch);
            }
            _branchNameOverride = TextInput(_branchNameOverride, "Branch Override");
            EditorGUILayout.EndHorizontal();
            
            GUI.enabled = true;
		}

		public bool ShouldDisplay(Build build)
		{
			if (build.BuildStep != BuildStep.Completed)
			{
				return false;
			}

			var settings = build.Configuration.BuildSettings;

			if (settings.Main.Platform.Value.AsBuildTargetGroup != BuildTargetGroup.Standalone)
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
        
        private string TextInput(string text, string placeholder) {
            var newText = EditorGUILayout.TextField(text);
            if (string.IsNullOrEmpty(text.Trim())) {
                const int textMargin = 2;
                var guiColor = GUI.color;
                GUI.color = Color.grey;
                var textRect = GUILayoutUtility.GetLastRect();
                var position = new Rect(textRect.x + textMargin, textRect.y, textRect.width, textRect.height);
                EditorGUI.LabelField(position, placeholder);
                GUI.color = guiColor;
            }
            return newText;
        }
	}
}