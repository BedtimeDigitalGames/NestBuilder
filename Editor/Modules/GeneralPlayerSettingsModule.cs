using System;
using System.ComponentModel;
using UnityEditor;

namespace BedtimeCore.NestBuilder
{
	[Serializable]
	public class GeneralPlayerSettingsModule : ISettingsModule
	{
		public BoolSetting MuteOtherAudioSources = new(x => PlayerSettings.muteOtherAudioSources = x);
		public BoolSetting BakeCollisionMeshes = new(x => PlayerSettings.bakeCollisionMeshes = x);
		public BoolSetting StripUnusedMeshComponents = new(x => PlayerSettings.stripUnusedMeshComponents = x);
		public BoolSetting UseIncrementalGC = new(x => PlayerSettings.gcIncremental = x);
		public EnumSetting<InsecureHttpOption> InsecureHttp = new(x => PlayerSettings.insecureHttpOption = x);
	}
}