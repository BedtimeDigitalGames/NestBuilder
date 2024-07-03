using System;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;

namespace BedtimeCore.NestBuilder
{
	[Serializable]
	public class MainModule : ISettingsModule
	{
		[Required, Recompile, Priority(0), Tooltip("The target platform"), Category("Main")]
		public PlatformSetting Platform = new(x => BuildSettingMapper.Platform = x);

		[Required, Recompile, Priority(1), Category("Main")]
		public EnumSetting<ScriptingImplementation> ScriptingBackend = new(x => BuildSettingMapper.ScriptingImplementation = x);

		[Recompile, Priority(2), Category("Main")]
		public EnumSetting<ApiCompatibilityLevel> APICompatibilityLevel = new(x => PlayerSettings.SetApiCompatibilityLevel(NestBuilder.Platform.GetNamedBuildTarget(EditorUserBuildSettings.activeBuildTarget), x));

		[Recompile, Priority(3), Category("Main")]
		public ScriptingDefineSymbolsSetting ScriptingDefineSymbols = new();
		
		[Required, Tooltip("Scenes to include in the build"), Category("Main")]
		public SceneSetting Scenes = new();

		[Required, Tooltip("The path and filename of the build output"), Category("Main")]
		public PathSetting LocationPathName = new("$WORKSPACE_PATH$/BUILDS/$PRODUCT_NAME$/$CONFIGURATION_NAME$/$DATE$/$PRODUCT_NAME$.$FILE_TYPE$");
		
#if BEDTIME_ASSET_SYSTEM
		[Category("Main")]
		public AssetInitializerLoadOrderSetting AssetInitializerLoadOrder = new(x => x.Save());
		
		[Category("AssetBundles")]
		public AssetBundleBuildInfoSetting AssetBundlesCopy = new();
#endif
	}
}