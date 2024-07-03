using UnityEditor;
using UnityEditor.Build;

namespace BedtimeCore.NestBuilder
{
	/// <summary>
	/// Settings that cannot always be mapped one-to-one with the Unity API.
	/// </summary>
	public static class BuildSettingMapper
	{
		public static Il2CppCompilerConfiguration Il2CPPCompilerConfiguration
		{
#if UNITY_2022_1_OR_NEWER
			get => PlayerSettings.GetIl2CppCompilerConfiguration(Platform.AsNamedBuildTarget);
			set => PlayerSettings.SetIl2CppCompilerConfiguration(Platform.AsNamedBuildTarget, value);
#else
			get => PlayerSettings.GetIl2CppCompilerConfiguration(Platform.AsBuildTargetGroup);
			set => PlayerSettings.SetIl2CppCompilerConfiguration(Platform.AsBuildTargetGroup, value);
#endif
		}

		public static Il2CppCodeGeneration Il2CppCodeGeneration
		{
#if UNITY_2022_1_OR_NEWER
			get => PlayerSettings.GetIl2CppCodeGeneration(Platform.AsNamedBuildTarget);
			set => PlayerSettings.SetIl2CppCodeGeneration(Platform.AsNamedBuildTarget, value);
#else
			get => default;
			set { }
#endif
		}

		public static string Version
		{
			get { return PlayerSettings.bundleVersion; }
			set
			{
				PlayerSettings.bundleVersion = value;
#if UNITY_SWITCH && !UNITY_2023_3_OR_NEWER
				PlayerSettings.Switch.displayVersion = value;
#endif
			}
		}

		public static ManagedStrippingLevel StrippingLevel
		{
			get => PlayerSettings.GetManagedStrippingLevel(Platform.AsNamedBuildTarget);
			set => PlayerSettings.SetManagedStrippingLevel(Platform.AsNamedBuildTarget, value);
		}
		
		public static Platform Platform
		{
			get => new Platform(EditorUserBuildSettings.activeBuildTarget);
			set => EditorUserBuildSettings.SwitchActiveBuildTarget(value.AsBuildTargetGroup, value.buildTarget);
		}

		public static ScriptingImplementation ScriptingImplementation
		{
			get => PlayerSettings.GetScriptingBackend(Platform.AsNamedBuildTarget);
			set => PlayerSettings.SetScriptingBackend(Platform.AsNamedBuildTarget, value);
		}
	}
}