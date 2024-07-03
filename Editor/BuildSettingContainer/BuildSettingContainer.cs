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
		public MetadataSetting metadata = new(EditorMetadata.ClearAndApply);
#endif
		[Category("Application")]
		public StringSetting companyName = new(x => PlayerSettings.companyName = x);

		[Category("Application")]
		public StringSetting productName = new(x => PlayerSettings.productName = x);

		[Category("Application")]
		public StringSetting version = new(x => BuildSettingMapper.Version = x);
		
		[Category("Application")]
		public BoolSetting showSplashScreen = new(x => PlayerSettings.SplashScreen.show = x);
		
		[Required, Recompile, Priority(0), Tooltip("The target platform"), Category("Main")]
		public PlatformSetting platform = new(x => BuildSettingMapper.Platform = x);

		[Required, Recompile, Priority(1), Category("Main")]
		public EnumSetting<ScriptingImplementation> scriptingBackend = new(x => BuildSettingMapper.ScriptingImplementation = x);

		[Recompile, Priority(2), Category("Main")]
		public EnumSetting<ApiCompatibilityLevel> APICompatabilityLevel = new(x => PlayerSettings.SetApiCompatibilityLevel(NamedBuildTarget, x));

		[Recompile, Priority(3), Category("Main")]
		public ScriptingDefineSymbolsSetting scriptingDefineSymbols = new();
#if BEDTIME_ASSET_SYSTEM
		[Category("Main")]
		public AssetInitializerLoadOrderSetting assetInitializerLoadOrder = new(x => x.Save());
#endif
		[Required, Tooltip("Scenes to include in the build"), Category("Main")]
		public SceneSetting scenes = new();

		[Required, Tooltip("The path and filename of the build output"), Category("Main")]
		public PathSetting locationPathName = new("$WORKSPACE_PATH$/BUILDS/$PRODUCT_NAME$/$CONFIGURATION_NAME$/$DATE$/$PRODUCT_NAME$.$FILE_TYPE$");

#if BEDTIME_ASSET_SYSTEM
		[Category("AssetBundles")]
		public AssetBundleBuildInfoSetting assetBundlesCopy = new();
#endif

		[Category("Debug")]
		public BoolSetting autoConnectProfiler = new(x => EditorUserBuildSettings.connectProfiler = x);

		[Category("Debug")]
		public BoolSetting allowDebugging = new(x => EditorUserBuildSettings.allowDebugging = x);

		[Category("Debug")]
		public BoolSetting buildScriptsOnly = new(x => EditorUserBuildSettings.buildScriptsOnly = x);

		[Category("Debug")]
		public BoolSetting development = new(x => EditorUserBuildSettings.development = x);

		[Category("Debug")]
		public EnumSetting<StandaloneBuildSubtarget> enableHeadlessMode = new(x => EditorUserBuildSettings.standaloneBuildSubtarget = x);

		[Category("Debug")]
		public BoolSetting explicitDivideByZeroChecks = new(x => EditorUserBuildSettings.explicitDivideByZeroChecks = x);

		[Category("Debug")]
		public BoolSetting explicitNullChecks = new(x => EditorUserBuildSettings.explicitNullChecks = x);

		[Category("Debug")]
		public BoolSetting waitForManagedDebugger = new(x => EditorUserBuildSettings.waitForManagedDebugger = x);
		
		[Category("Debug")]
		public BoolSetting usePlayerLog = new(x => PlayerSettings.usePlayerLog = x);
		
		[Category("Debug")]
		public BoolSetting enableCrashReportAPI = new(x => PlayerSettings.enableCrashReportAPI = x);

		
		
		[Category("BuildStep Methods")]
		public BuildProcessorSetting preConfigurationStep = new(BuildStep.PreConfiguration);

		[Category("BuildStep Methods")]
		public BuildProcessorSetting postConfigurationStep = new(BuildStep.PostConfiguration);

		[Category("BuildStep Methods")]
		public BuildProcessorSetting preBuildStep = new(BuildStep.PreBuild);

		[Category("BuildStep Methods")]
		public BuildProcessorSetting postBuildStep = new(BuildStep.PostBuild);
		
		

		[Category("Standalone"), PlatformRestriction(BuildTargetGroup.Standalone)]
		public Texture2DSetting defaultCursor = new(x => PlayerSettings.defaultCursor = x);

		[Category("Standalone"), PlatformRestriction(BuildTargetGroup.Standalone)]
		public Vector2Setting cursorHotSpot = new(x => PlayerSettings.cursorHotspot = x);

		[Category("Application"), ScriptingBackendRestriction(ScriptingImplementation.IL2CPP)]
		public EnumSetting<Il2CppCompilerConfiguration> il2cppCompilerConfiguration = new(x => BuildSettingMapper.Il2CPPCompilerConfiguration = x);

		[Category("Application")]
		public StringSetting applicationIdentifier = new(x => PlayerSettings.SetApplicationIdentifier(NamedBuildTarget, x));

		[Category("Rendering")]
		public BoolSetting multiThreadedRendering = new(x => PlayerSettings.MTRendering = x);

		[Category("Rendering")]
		public BoolSetting use32BitDisplayBuffer = new(x => PlayerSettings.use32BitDisplayBuffer = x);

		[Category("Rendering")]
		public BoolSetting useHDRDisplay = new(x => PlayerSettings.useHDRDisplay = x);
		
		[Category("Rendering")]
		public BoolSetting GPUSkinning = new(x => PlayerSettings.gpuSkinning = x);

		[Category("Rendering")]
		public BoolSetting graphicsJobs = new(x => PlayerSettings.graphicsJobs = x);
		
		
		
		[Category("Standalone"), PlatformRestriction(BuildTargetGroup.Standalone)]
		public BoolSetting allowFullscreenSwitch = new(x => PlayerSettings.allowFullscreenSwitch = x);

		[Category("Standalone"), PlatformRestriction(BuildTargetGroup.Standalone)]
		public BoolSetting runInBackground = new(x => PlayerSettings.runInBackground = x);

		[Category("Standalone"), PlatformRestriction(BuildTargetGroup.Standalone)]
		public BoolSetting visibleInBackground = new(x => PlayerSettings.visibleInBackground = x);

		[Category("Standalone"), PlatformRestriction(BuildTargetGroup.Standalone)]
		public BoolSetting captureSingleScreen = new(x => PlayerSettings.captureSingleScreen = x);

		[Category("Standalone"), PlatformRestriction(BuildTargetGroup.Standalone)]
		public EnumSetting<FullScreenMode> fullScreenMode = new(x => PlayerSettings.fullScreenMode = x);

		[Category("Standalone"), PlatformRestriction(BuildTargetGroup.Standalone)]
		public BoolSetting forceSingleInstance = new(x => PlayerSettings.forceSingleInstance = x);
        
		[Category("Nintendo Switch")]
		public BoolSetting switchCreateRomFile = new(x => BuildSettingMapper.SwitchSettings.CreateRomFile = x);

		[Category("Nintendo Switch")]
		public BoolSetting switchEnableDebugPad = new(x => BuildSettingMapper.SwitchSettings.EnableDebugPad = x);

		[Category("Nintendo Switch")]
		public BoolSetting switchNVNGraphicsDebugger = new(x => BuildSettingMapper.SwitchSettings.NVNGraphicsDebugger = x);

		[Category("Nintendo Switch")]
		public BoolSetting switchRedirectWritesToHostMount = new(x => BuildSettingMapper.SwitchSettings.RedirectWritesToHostMount = x);

		[Category("Nintendo Switch")]
		public BoolSetting switchNetworkInterfaceManagerInitializeEnabled = new(x => BuildSettingMapper.SwitchSettings.NetworkInterfaceManagerInitializeEnabled = x);

		[Category("Nintendo Switch")]
		public EnumSetting<PlayerSettings.Switch.ScreenResolutionBehavior> switchScreenResolutionBehavior = new(x => BuildSettingMapper.SwitchSettings.ScreenResolutionBehavior = x);

		[Category("Nintendo Switch")]
		public BoolSetting switchSocketConfigEnabled = new(x => BuildSettingMapper.SwitchSettings.SocketConfigEnabled = x);

		[Category("Nintendo Switch")]
		public BoolSetting switchSocketInitializeEnabled = new(x => BuildSettingMapper.SwitchSettings.SocketInitializeEnabled = x);

		[Category("Nintendo Switch")]
		public BoolSetting switchUseSwitchCPUProfiler = new(x => BuildSettingMapper.SwitchSettings.UseSwitchCPUProfiler = x);

		[Category("Nintendo Switch")]
		public StringSetting switchNsoDependencies = new(x => BuildSettingMapper.SwitchSettings.NsoDependencies = x);

		[Category("PS4")]
		public StringSetting ps4ContentID = new(x => PlayerSettings.PS4.contentID = x);

		[Category("PS4")]
		public StringSetting ps4AppVersion = new(x => PlayerSettings.PS4.appVersion = x);

		[Category("PS4")]
		public StringSetting ps4MasterVersion = new(x => PlayerSettings.PS4.masterVersion = x);

		[Category("PS4")]
		public StringSetting ps4NpTitlePath = new(x => PlayerSettings.PS4.NPtitleDatPath = x);

		[Category("PS4")]
		public StringSetting ps4NpTrophyPackPath = new(x => PlayerSettings.PS4.npTrophyPackPath = x);

		[Category("PS4")]
		public IntSetting ps4ParentalLockLevel = new(x => PlayerSettings.PS4.parentalLevel = x);


		[Category("iOS/Android")]
		public BoolSetting enableInternalProfiler = new(x => PlayerSettings.enableInternalProfiler = x);

		[Category("iOS/Android")]
		public IntSetting accelerometerFrequency = new(x => PlayerSettings.accelerometerFrequency = x);
		
		[Category("iOS/Android"), PlatformRestriction(BuildTargetGroup.Android, BuildTargetGroup.iOS)]
		public BoolSetting useAnimatedAutorotation = new(x => PlayerSettings.useAnimatedAutorotation = x);
		
		[Category("iOS/Android"), PlatformRestriction(BuildTargetGroup.Android, BuildTargetGroup.iOS)]
		public BoolSetting allowedAutorotateToLandscapeLeft = new(x => PlayerSettings.allowedAutorotateToLandscapeLeft = x);

		[Category("iOS/Android"), PlatformRestriction(BuildTargetGroup.Android, BuildTargetGroup.iOS)]
		public BoolSetting allowedAutorotateToLandscapeRight = new(x => PlayerSettings.allowedAutorotateToLandscapeRight = x);

		[Category("iOS/Android"), PlatformRestriction(BuildTargetGroup.Android, BuildTargetGroup.iOS)]
		public BoolSetting allowedAutorotateToPortrait = new(x => PlayerSettings.allowedAutorotateToPortrait = x);

		[Category("iOS/Android"), PlatformRestriction(BuildTargetGroup.Android, BuildTargetGroup.iOS)]
		public BoolSetting allowedAutorotateToPortraitUpsideDown = new(x => PlayerSettings.allowedAutorotateToPortraitUpsideDown = x);

		
		
		[Category("iOS")]
		public StringSetting appleDeveloperTeamID = new(x => PlayerSettings.iOS.appleDeveloperTeamID = x);
		
		[Category("iOS")]
		public EnumSetting<iOSBackgroundMode> iosBackgroundMode = new(x => PlayerSettings.iOS.backgroundModes = x);

		[Category("iOS")]
		public EnumSetting<iOSStatusBarStyle> iosStatusBarStyle = new(x => PlayerSettings.iOS.statusBarStyle = x);
		
		[Category("iOS")]
		public EnumSetting<iOSShowActivityIndicatorOnLoading> iosShowActivityIndicatorOnLoading = new(x => PlayerSettings.iOS.showActivityIndicatorOnLoading = x);
		
		[Category("iOS")]
		public EnumSetting<iOSTargetDevice> iosTargetDevice = new(x => PlayerSettings.iOS.targetDevice = x);
		
		[Category("iOS")]
		public EnumSetting<iOSSdkVersion> iosSdkVersion = new(x => PlayerSettings.iOS.sdkVersion = x);
		
		[Category("iOS")]
		public StringSetting iosMinimumVersion = new(x => PlayerSettings.iOS.targetOSVersionString = x);
		
		
        
        [Category("Android")]
        public EnumSetting<AndroidArchitecture> androidTargetArchitecture = new(x => PlayerSettings.Android.targetArchitectures = x);
		[Category("Android")]
		public BoolSetting buildAppBundle = new(x => EditorUserBuildSettings.buildAppBundle = x);

		[Category("Android")]
		public IntSetting bundleVersionCode = new(x => PlayerSettings.Android.bundleVersionCode = x);

		[Category("Android")]
		public BoolSetting UseCustomKeyStore = new(x => PlayerSettings.Android.useCustomKeystore = x);

		[Category("Android")]
		public SecretSetting keyStorePath = new("ANDROID_KEYSTORE_NAME", false, x => PlayerSettings.Android.keystoreName = x);

		[Category("Android")]
		public SecretSetting keyStorePassword = new("ANDROID_KEYSTORE_PASS", x => PlayerSettings.Android.keystorePass = x);

		[Category("Android")]
		public SecretSetting keyStoreAlias = new("ANDROID_KEYSTORE_ALIAS",false, x => PlayerSettings.Android.keyaliasName = x);

		[Category("Android")]
		public SecretSetting keyStoreAliasPassword = new("ANDROID_KEYSTORE_ALIAS_PASS",x => PlayerSettings.Android.keyaliasPass = x);
		
		

		[Category("WebGL")]
		public BoolSetting webglThreadsSupport = new(x => PlayerSettings.WebGL.threadsSupport = x);

		[Category("WebGL")]
		public BoolSetting webglDataCaching = new(x => PlayerSettings.WebGL.dataCaching = x);

		[Category("WebGL")]
		public EnumSetting<WebGLDebugSymbolMode> webglDebugSymbols = new(x => PlayerSettings.WebGL.debugSymbolMode = x);

		[Category("WebGL")]
		public BoolSetting webglAnalyzeBuildSize = new(x => PlayerSettings.WebGL.analyzeBuildSize = x);

		[Category("WebGL")]
		public BoolSetting webglUseEmbeddedResources = new(x => PlayerSettings.WebGL.useEmbeddedResources = x);

		[Category("WebGL")]
		public BoolSetting webglNameFilesAsHashes = new(x => PlayerSettings.WebGL.nameFilesAsHashes = x);

		[Category("WebGL")]
		public StringSetting webglEmscriptenArgs = new(x => PlayerSettings.WebGL.emscriptenArgs = x);

		[Category("WebGL")]
		public EnumSetting<WebGLExceptionSupport> webglExceptionSupport = new(x => PlayerSettings.WebGL.exceptionSupport = x);

		[Category("WebGL")]
		public EnumSetting<WebGLLinkerTarget> webglLinkerTarget = new(x => PlayerSettings.WebGL.linkerTarget = x);

		[Category("WebGL")]
		public EnumSetting<WebGLCompressionFormat> webglCompressionFormat = new(x => PlayerSettings.WebGL.compressionFormat = x);
		
		

		[Category("Steam")]
		public StringSetting steamSetLiveOnBranch = new();

		[Category("Steam")]
		public IntSetting steamAppID = new();

		[Category("Steam")]
		public IntSetting steamDepotID = new();

		[Category("Steam")]
		public SecretSetting steamSDKPath = new("STEAM_SDK_PATH", false);

		[Category("Steam")]
		public SecretSetting steamLogin = new("STEAM_BUILDER_LOGIN", false);

		[Category("Steam")]
		public SecretSetting steamPassword = new("STEAM_BUILDER_LOGIN_CODE", hideSecret: true);
		
		

		[Category("Epic")]
		public StringSetting epicProductId;

		[Category("Epic")]
		public StringSetting epicClientId;

		[Category("Epic")]
		public StringSetting epicClientSecret;

		[Category("Epic")]
		public StringSetting epicEncryptionKey;
		
		
		
		[Category("Apple"), PlatformRestriction(BuildTarget.iOS, BuildTarget.StandaloneOSX)]
		public BoolSetting useMacAppStoreValidation = new(x => PlayerSettings.useMacAppStoreValidation = x);
		
		
		
		[Category("IL2CPP"), ScriptingBackendRestriction(ScriptingImplementation.IL2CPP)]
		public BoolSetting stripEngineCode = new(x => PlayerSettings.stripEngineCode = x);
        [Category("IL2CPP")]
		public EnumSetting<ManagedStrippingLevel> strippingLevel = new(x => BuildSettingMapper.StrippingLevel = x);

        [Category("IL2CPP")]
        public EnumSetting<Il2CppCodeGeneration> il2cppCodeGeneration = new(x => BuildSettingMapper.Il2CppCodeGeneration = x);

        [Category("IL2CPP")]
        public StringSetting il2cppAdditionalCompilerArguments = new(PlayerSettings.SetAdditionalIl2CppArgs);
        
        
        
		public BoolSetting muteOtherAudioSources = new(x => PlayerSettings.muteOtherAudioSources = x);
		public BoolSetting bakeCollisionMeshes = new(x => PlayerSettings.bakeCollisionMeshes = x);
		public BoolSetting stripUnusedMeshComponents = new(x => PlayerSettings.stripUnusedMeshComponents = x);
		public BoolSetting useIncrementalGC = new(x => PlayerSettings.gcIncremental = x);


		#region helpers

		protected static NamedBuildTarget NamedBuildTarget => Platform.GetNamedBuildTarget(EditorUserBuildSettings.activeBuildTarget);

		#endregion

#if BEDTIME_LOGGING
		[Category("Logging"), Recompile]
		public BoolSetting bedLogEnabled = new();

		[Category("Logging")]
		public EnumSetting<LogLevelEnum> logLevel = new(LogSettings.SetLogLevelBuild);

		[Category("Logging")]
		public EnumSetting<BedLogTarget> logTargets = new(LogSettings.SetLogTargetsBuild);
#endif
	}
}