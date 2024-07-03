using System;
using System.ComponentModel;
using UnityEditor;

namespace BedtimeCore.NestBuilder
{
	[Serializable]
	public class DebugSettingsModule : ISettingsModule
	{
		[Category("Debug")]
		public BoolSetting AutoConnectProfiler = new(x => EditorUserBuildSettings.connectProfiler = x);

		[Category("Debug")]
		public BoolSetting AllowDebugging = new(x => EditorUserBuildSettings.allowDebugging = x);

		[Category("Debug")]
		public BoolSetting BuildScriptsOnly = new(x => EditorUserBuildSettings.buildScriptsOnly = x);

		[Category("Debug")]
		public BoolSetting IsDevelopment = new(x => EditorUserBuildSettings.development = x);

		[Category("Debug")]
		public EnumSetting<StandaloneBuildSubtarget> EnableHeadlessMode = new(x => EditorUserBuildSettings.standaloneBuildSubtarget = x);

		[Category("Debug")]
		public BoolSetting ExplicitDivideByZeroChecks = new(x => EditorUserBuildSettings.explicitDivideByZeroChecks = x);

		[Category("Debug")]
		public BoolSetting ExplicitNullChecks = new(x => EditorUserBuildSettings.explicitNullChecks = x);

		[Category("Debug")]
		public BoolSetting WaitForManagedDebugger = new(x => EditorUserBuildSettings.waitForManagedDebugger = x);
		
		[Category("Debug")]
		public BoolSetting UsePlayerLog = new(x => PlayerSettings.usePlayerLog = x);
		
		[Category("Debug")]
		public BoolSetting EnableCrashReportAPI = new(x => PlayerSettings.enableCrashReportAPI = x);
	}
}