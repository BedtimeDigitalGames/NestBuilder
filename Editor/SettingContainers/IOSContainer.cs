using System;
using System.ComponentModel;
using UnityEditor;

namespace BedtimeCore.BuildPipeline
{
	[Serializable]
	public class IOSContainer : ISettingsContainer
	{
		[Category("iOS")]
		public StringSetting AppleDeveloperTeamID = new(x => PlayerSettings.iOS.appleDeveloperTeamID = x);
		
		[Category("iOS")]
		public EnumSetting<iOSBackgroundMode> BackgroundMode = new(x => PlayerSettings.iOS.backgroundModes = x);

		[Category("iOS")]
		public EnumSetting<iOSStatusBarStyle> StatusBarStyle = new(x => PlayerSettings.iOS.statusBarStyle = x);
		
		[Category("iOS")]
		public EnumSetting<iOSShowActivityIndicatorOnLoading> ShowActivityIndicatorOnLoading = new(x => PlayerSettings.iOS.showActivityIndicatorOnLoading = x);
		
		[Category("iOS")]
		public EnumSetting<iOSTargetDevice> TargetDevice = new(x => PlayerSettings.iOS.targetDevice = x);
		
		[Category("iOS")]
		public EnumSetting<iOSSdkVersion> SdkVersion = new(x => PlayerSettings.iOS.sdkVersion = x);
		
		[Category("iOS")]
		public StringSetting MinimumVersion = new(x => PlayerSettings.iOS.targetOSVersionString = x);
	}
}