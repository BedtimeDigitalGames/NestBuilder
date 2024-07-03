using System;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;

namespace BedtimeCore.BuildPipeline
{
	[Serializable]
	public class StandaloneContainer : ISettingsContainer
	{
		[Category("Standalone"), PlatformRestriction(BuildTargetGroup.Standalone)]
		public Texture2DSetting DefaultCursor = new(x => PlayerSettings.defaultCursor = x);
        
		[Category("Standalone"), PlatformRestriction(BuildTargetGroup.Standalone)]
		public Vector2Setting CursorHotSpot = new(x => PlayerSettings.cursorHotspot = x);
        
		[Category("Standalone"), PlatformRestriction(BuildTargetGroup.Standalone)]
		public BoolSetting AllowFullscreenSwitch = new(x => PlayerSettings.allowFullscreenSwitch = x);
        
		[Category("Standalone"), PlatformRestriction(BuildTargetGroup.Standalone)]
		public BoolSetting RunInBackground = new(x => PlayerSettings.runInBackground = x);
        
		[Category("Standalone"), PlatformRestriction(BuildTargetGroup.Standalone)]
		public BoolSetting VisibleInBackground = new(x => PlayerSettings.visibleInBackground = x);
        
		[Category("Standalone"), PlatformRestriction(BuildTargetGroup.Standalone)]
		public BoolSetting CaptureSingleScreen = new(x => PlayerSettings.captureSingleScreen = x);
        
		[Category("Standalone"), PlatformRestriction(BuildTargetGroup.Standalone)]
		public EnumSetting<FullScreenMode> FullScreenMode = new(x => PlayerSettings.fullScreenMode = x);
        
		[Category("Standalone"), PlatformRestriction(BuildTargetGroup.Standalone)]
		public BoolSetting ForceSingleInstance = new(x => PlayerSettings.forceSingleInstance = x);
        
	}
}