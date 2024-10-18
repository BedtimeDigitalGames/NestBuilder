using System;
using System.ComponentModel;
using UnityEditor;

namespace BedtimeCore.NestBuilder
{
	[Serializable]
	public class NintendoSwitchModule : ISettingsModule
	{
		[Category("Nintendo Switch")]
		public BoolSetting CreateRomFile = new(x => EditorUserBuildSettings.switchCreateRomFile = x);

		[Category("Nintendo Switch")]
		public BoolSetting EnableDebugPad = new(x => EditorUserBuildSettings.switchEnableDebugPad = x);

		[Category("Nintendo Switch")]
		public BoolSetting NVNGraphicsDebugger = new(x => EditorUserBuildSettings.switchNVNGraphicsDebugger = x);

		[Category("Nintendo Switch")]
		public BoolSetting RedirectWritesToHostMount = new(x => EditorUserBuildSettings.switchEnableHostIO = x);

		[Category("Nintendo Switch")]
		public BoolSetting NetworkInterfaceManagerInitializeEnabled = new(x => PlayerSettings.Switch.networkInterfaceManagerInitializeEnabled = x);

		[Category("Nintendo Switch")]
		public EnumSetting<PlayerSettings.Switch.ScreenResolutionBehavior> ScreenResolutionBehavior = new(x => PlayerSettings.Switch.screenResolutionBehavior = x);

		[Category("Nintendo Switch")]
		public BoolSetting SocketConfigEnabled = new(x => PlayerSettings.Switch.socketConfigEnabled = x);

		[Category("Nintendo Switch")]
		public BoolSetting SocketInitializeEnabled = new(x => PlayerSettings.Switch.socketInitializeEnabled = x);

		[Category("Nintendo Switch")]
		public BoolSetting UseSwitchCPUProfiler = new(x => PlayerSettings.Switch.useSwitchCPUProfiler = x);

		[Category("Nintendo Switch")]
		public StringSetting NsoDependencies = new(x => PlayerSettings.Switch.nsoDependencies = x);
	}
}