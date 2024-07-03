namespace BedtimeCore.NestBuilder
{
	public class BedtimeLoggingModule
	{
#if BEDTIME_LOGGING
		[Category("Logging"), Recompile]
		public BoolSetting BedLogEnabled = new();

		[Category("Logging")]
		public EnumSetting<LogLevelEnum> LogLevel = new(LogSettings.SetLogLevelBuild);

		[Category("Logging")]
		public EnumSetting<BedLogTarget> LogTargets = new(LogSettings.SetLogTargetsBuild);
#endif
	}
}