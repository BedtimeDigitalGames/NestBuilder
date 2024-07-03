using UnityEditor;
using System;
#if BEDTIME_LOGGING
using BedtimeCore.ProjectSettings;
#endif

namespace BedtimeCore.NestBuilder
{
	[Serializable]
	public class ScriptingDefineSymbolsSetting : StringSetting
	{
		public ScriptingDefineSymbolsSetting(Action<string> applyAction = null) : base(applyAction)
		{
		}

		public override void ApplyToEditor(BuildConfiguration configuration)
		{
			Platform platform = configuration.BuildSettings.Main.Platform.Value;
			string newSymbols = ValueSelf;
			
			#if BEDTIME_LOGGING
			var loggingEnabledSetting = configuration.BuildSettings.BedtimeLogging.bedLogEnabled;
			var loggingEnabled = loggingEnabledSetting.IsSet ? loggingEnabledSetting.Value : LogSettings.LoggingEnabled;
			newSymbols = LogSettingsUtility.AddOrRemoveLoggingSymbol(ValueSelf, loggingEnabled);
			#endif
			
			PlayerSettings.SetScriptingDefineSymbols(platform.AsNamedBuildTarget, newSymbols);
		}

		public override void DrawValue(BuildConfiguration topLevel, SerializedProperty property)
		{
			value = EditorGUILayout.TextField(value);
		}

		public override bool WillTriggerRecompile
		{
			get
			{
				var platform = Owner.BuildSettings.Main.Platform.Value;
				return PlayerSettings.GetScriptingDefineSymbols(platform.AsNamedBuildTarget) != ValueSelf;
			}
		}
	}
}
