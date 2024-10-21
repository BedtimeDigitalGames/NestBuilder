using System;
using System.ComponentModel;
using UnityEditor;
using UnityEngine.iOS;

namespace BedtimeCore.NestBuilder
{
	[Serializable]
	public class IOSModule : ISettingsModule
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
		
		[Category("iOS")]
		public BoolSetting HideHomeButton = new(x => PlayerSettings.iOS.hideHomeButton = x);
		
        [Category("iOS")]
		public EnumSetting<SystemGestureDeferMode> DeferSystemGesturesMode = new(x => PlayerSettings.iOS.deferSystemGesturesMode = x);
		
        [Category("iOS")]
		public BoolSetting DisableDepthAndStencilBuffers = new(x => PlayerSettings.iOS.disableDepthAndStencilBuffers = x);
		
        [Category("iOS")]
		public BoolSetting RequiresPersistentWiFi = new(x => PlayerSettings.iOS.requiresPersistentWiFi = x);
		
        [Category("iOS")]
		public BoolSetting ForceHardShadowsOnMetal = new(x => PlayerSettings.iOS.forceHardShadowsOnMetal = x);
		
        [Category("iOS")]
		public BoolSetting UseOnDemandResources = new(x => PlayerSettings.iOS.useOnDemandResources = x);
	}
}