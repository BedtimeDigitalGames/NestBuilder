using System;
using System.ComponentModel;

namespace BedtimeCore.NestBuilder
{
	[Serializable]
	public class BedtimeLoggingModule : ISettingsModule
	{
		#if BEDTIME_LOGGING
		[Category("Bedtime Logging"), Recompile]
		public BoolSetting BedLogEnabled = new();

		[Category("Bedtime Logging")]
		public EnumSetting<LogLevelEnum> LogLevel = new(BedtimeCore.ProjectSettings.LogSettings.SetLogLevelBuild);

		[Category("Bedtime Logging")]
		public EnumSetting<BedLogTarget> LogTargets = new(BedtimeCore.ProjectSettings.LogSettings.SetLogTargetsBuild);
		
		[UnityEditor.Callbacks.DidReloadScripts]
		private static void OnScriptReload()
		{
			if (Builder.IsBuilding)
			{
				return;
			}
			
			BedtimeCore.ProjectSettings.LogSettingsUtility.SetLogDefine();
		}
		#endif
	}
}