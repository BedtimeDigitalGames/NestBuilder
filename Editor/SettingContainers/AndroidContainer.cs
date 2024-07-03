using System;
using System.ComponentModel;
using UnityEditor;

namespace BedtimeCore.BuildPipeline
{
	[Serializable]
	public class AndroidContainer : ISettingsContainer
	{
		[Category("Android")]
		public EnumSetting<AndroidArchitecture> TargetArchitecture = new(x => PlayerSettings.Android.targetArchitectures = x);
		[Category("Android")]
		public BoolSetting buildAppBundle = new(x => EditorUserBuildSettings.buildAppBundle = x);

		[Category("Android")]
		public IntSetting bundleVersionCode = new(x => PlayerSettings.Android.bundleVersionCode = x);

		[Category("Android")]
		public BoolSetting useCustomKeyStore = new(x => PlayerSettings.Android.useCustomKeystore = x);

		[Category("Android")]
		public SecretSetting keyStorePath = new("ANDROID_KEYSTORE_NAME", false, x => PlayerSettings.Android.keystoreName = x);

		[Category("Android")]
		public SecretSetting keyStorePassword = new("ANDROID_KEYSTORE_PASS", x => PlayerSettings.Android.keystorePass = x);

		[Category("Android")]
		public SecretSetting keyStoreAlias = new("ANDROID_KEYSTORE_ALIAS",false, x => PlayerSettings.Android.keyaliasName = x);

		[Category("Android")]
		public SecretSetting keyStoreAliasPassword = new("ANDROID_KEYSTORE_ALIAS_PASS",x => PlayerSettings.Android.keyaliasPass = x);
	}
}