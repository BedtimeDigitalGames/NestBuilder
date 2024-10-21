using System;
using System.ComponentModel;
using UnityEditor;

namespace BedtimeCore.NestBuilder
{
	[Serializable]
	public class AppleModule : ISettingsModule
	{
		[Category("Apple"), PlatformRestriction(BuildTarget.iOS, BuildTarget.StandaloneOSX, BuildTarget.tvOS)]
		public BoolSetting UseMacAppStoreValidation = new(x => PlayerSettings.useMacAppStoreValidation = x);
	}
}