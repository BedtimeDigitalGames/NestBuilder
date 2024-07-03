using System;
using System.ComponentModel;
using UnityEditor;
using UnityEditor.Build;

namespace BedtimeCore.NestBuilder
{
	[Serializable]
	public class IL2CPPModule : ISettingsModule
	{
		[Category("IL2CPP"), ScriptingBackendRestriction(ScriptingImplementation.IL2CPP)]
		public EnumSetting<Il2CppCompilerConfiguration> CompilerConfiguration = new(x => BuildSettingMapper.Il2CPPCompilerConfiguration = x);

		[Category("IL2CPP"), ScriptingBackendRestriction(ScriptingImplementation.IL2CPP)]
		public BoolSetting StripEngineCode = new(x => PlayerSettings.stripEngineCode = x);
		
		[Category("IL2CPP")]
		public EnumSetting<ManagedStrippingLevel> StrippingLevel = new(x => BuildSettingMapper.StrippingLevel = x);

		[Category("IL2CPP")]
		public EnumSetting<Il2CppCodeGeneration> CodeGeneration = new(x => BuildSettingMapper.Il2CppCodeGeneration = x);

		[Category("IL2CPP")]
		public StringSetting AdditionalCompilerArguments = new(PlayerSettings.SetAdditionalIl2CppArgs);
	}
}