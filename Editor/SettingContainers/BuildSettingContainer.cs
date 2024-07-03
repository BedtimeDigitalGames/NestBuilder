using System;
using System.ComponentModel;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
#if BEDTIME_LOGGING
using BedtimeCore.ProjectSettings;
#endif
#if BEDTIME_METADATA
using BedtimeCore.Persistence;
#endif

namespace BedtimeCore.BuildPipeline
{
	[Serializable]
	public class BuildSettingContainer
	{
#if BEDTIME_METADATA
		[Category("Application")]
		public MetadataSetting Metadata = new(EditorMetadata.ClearAndApply);
#endif
		[Category("Application")]
		public StringSetting CompanyName = new(x => PlayerSettings.companyName = x);

		[Category("Application")]
		public StringSetting ProductName = new(x => PlayerSettings.productName = x);

		[Category("Application")]
		public StringSetting Version = new(x => BuildSettingMapper.Version = x);
		
		[Category("Application")]
		public BoolSetting ShowSplashScreen = new(x => PlayerSettings.SplashScreen.show = x);
		
		[Required, Recompile, Priority(0), Tooltip("The target platform"), Category("Main")]
		public PlatformSetting Platform = new(x => BuildSettingMapper.Platform = x);

		[Required, Recompile, Priority(1), Category("Main")]
		public EnumSetting<ScriptingImplementation> ScriptingBackend = new(x => BuildSettingMapper.ScriptingImplementation = x);

		[Recompile, Priority(2), Category("Main")]
		public EnumSetting<ApiCompatibilityLevel> APICompatibilityLevel = new(x => PlayerSettings.SetApiCompatibilityLevel(NamedBuildTarget, x));

		[Recompile, Priority(3), Category("Main")]
		public ScriptingDefineSymbolsSetting ScriptingDefineSymbols = new();
		
#if BEDTIME_ASSET_SYSTEM
		[Category("Main")]
		public AssetInitializerLoadOrderSetting AssetInitializerLoadOrder = new(x => x.Save());
#endif
		[Required, Tooltip("Scenes to include in the build"), Category("Main")]
		public SceneSetting Scenes = new();

		[Required, Tooltip("The path and filename of the build output"), Category("Main")]
		public PathSetting LocationPathName = new("$WORKSPACE_PATH$/BUILDS/$PRODUCT_NAME$/$CONFIGURATION_NAME$/$DATE$/$PRODUCT_NAME$.$FILE_TYPE$");

#if BEDTIME_ASSET_SYSTEM
		[Category("AssetBundles")]
		public AssetBundleBuildInfoSetting assetBundlesCopy = new();
#endif

		[Category("Application")]
		public StringSetting ApplicationIdentifier = new(x => PlayerSettings.SetApplicationIdentifier(NamedBuildTarget, x));

		[Category("Rendering")]
		public BoolSetting MultiThreadedRendering = new(x => PlayerSettings.MTRendering = x);

		[Category("Rendering")]
		public BoolSetting Use32BitDisplayBuffer = new(x => PlayerSettings.use32BitDisplayBuffer = x);

		[Category("Rendering")]
		public BoolSetting UseHDRDisplay = new(x => PlayerSettings.useHDRDisplay = x);
		
		[Category("Rendering")]
		public BoolSetting GPUSkinning = new(x => PlayerSettings.gpuSkinning = x);

		[Category("Rendering")]
		public BoolSetting GraphicsJobs = new(x => PlayerSettings.graphicsJobs = x);

		[Category("Apple"), PlatformRestriction(BuildTarget.iOS, BuildTarget.StandaloneOSX)]
		public BoolSetting UseMacAppStoreValidation = new(x => PlayerSettings.useMacAppStoreValidation = x);

		public BoolSetting MuteOtherAudioSources = new(x => PlayerSettings.muteOtherAudioSources = x);
		public BoolSetting BakeCollisionMeshes = new(x => PlayerSettings.bakeCollisionMeshes = x);
		public BoolSetting StripUnusedMeshComponents = new(x => PlayerSettings.stripUnusedMeshComponents = x);
		public BoolSetting UseIncrementalGC = new(x => PlayerSettings.gcIncremental = x);
		
		public IL2CPPContainer IL2CPP;

		public BuildStepsContainer BuildSteps;

		public DebugSettingsContainer Debugging;

		public StandaloneContainer Standalone;

		public NintendoSwitchContainer NintendoSwitch;

		public PS4Container PS4;
		
		public WebGlContainer WebGL;

		public SharedMobileContainer SharedMobile;

		public IOSContainer iOS;

		public AndroidContainer Android;

		public SteamContainer Steam;

		public EpicContainer Epic;

#if BEDTIME_LOGGING
		[Category("Logging"), Recompile]
		public BoolSetting BedLogEnabled = new();

		[Category("Logging")]
		public EnumSetting<LogLevelEnum> LogLevel = new(LogSettings.SetLogLevelBuild);

		[Category("Logging")]
		public EnumSetting<BedLogTarget> LogTargets = new(LogSettings.SetLogTargetsBuild);
#endif
		private static NamedBuildTarget NamedBuildTarget => BuildPipeline.Platform.GetNamedBuildTarget(EditorUserBuildSettings.activeBuildTarget);
	}
}