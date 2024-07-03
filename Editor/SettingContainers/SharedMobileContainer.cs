using System;
using System.ComponentModel;
using UnityEditor;

namespace BedtimeCore.BuildPipeline
{
	[Serializable]
	public class SharedMobileContainer : ISettingsContainer
	{
		[Category("iOS/Android")]
		public BoolSetting EnableInternalProfiler = new(x => PlayerSettings.enableInternalProfiler = x);

		[Category("iOS/Android")]
		public IntSetting AccelerometerFrequency = new(x => PlayerSettings.accelerometerFrequency = x);
		
		[Category("iOS/Android"), PlatformRestriction(BuildTargetGroup.Android, BuildTargetGroup.iOS)]
		public BoolSetting UseAnimatedAutorotation = new(x => PlayerSettings.useAnimatedAutorotation = x);
		
		[Category("iOS/Android"), PlatformRestriction(BuildTargetGroup.Android, BuildTargetGroup.iOS)]
		public BoolSetting AllowedAutorotateToLandscapeLeft = new(x => PlayerSettings.allowedAutorotateToLandscapeLeft = x);

		[Category("iOS/Android"), PlatformRestriction(BuildTargetGroup.Android, BuildTargetGroup.iOS)]
		public BoolSetting AllowedAutorotateToLandscapeRight = new(x => PlayerSettings.allowedAutorotateToLandscapeRight = x);

		[Category("iOS/Android"), PlatformRestriction(BuildTargetGroup.Android, BuildTargetGroup.iOS)]
		public BoolSetting AllowedAutorotateToPortrait = new(x => PlayerSettings.allowedAutorotateToPortrait = x);

		[Category("iOS/Android"), PlatformRestriction(BuildTargetGroup.Android, BuildTargetGroup.iOS)]
		public BoolSetting AllowedAutorotateToPortraitUpsideDown = new(x => PlayerSettings.allowedAutorotateToPortraitUpsideDown = x);

	}
}