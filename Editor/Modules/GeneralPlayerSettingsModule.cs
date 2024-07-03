using System;
using System.ComponentModel;
using UnityEditor;

namespace BedtimeCore.NestBuilder
{
	[Serializable]
	public class GeneralPlayerSettingsModule : ISettingsModule
	{
		[Category("Apple"), PlatformRestriction(BuildTarget.iOS, BuildTarget.StandaloneOSX)]
		public BoolSetting UseMacAppStoreValidation = new(x => PlayerSettings.useMacAppStoreValidation = x);
		public BoolSetting MuteOtherAudioSources = new(x => PlayerSettings.muteOtherAudioSources = x);
		public BoolSetting BakeCollisionMeshes = new(x => PlayerSettings.bakeCollisionMeshes = x);
		public BoolSetting StripUnusedMeshComponents = new(x => PlayerSettings.stripUnusedMeshComponents = x);
		public BoolSetting UseIncrementalGC = new(x => PlayerSettings.gcIncremental = x);
	}
}