using System;
using UnityEditor;
using UnityEngine;

namespace BedtimeCore.BuildPipeline
{
	/// <summary>
	///     Nintendo Switch specific
	/// </summary>
	public static partial class BuildSettingMapper
	{
		public static class SwitchSettings
		{

			public static bool CreateRomFile
			{
				get => EditorUserBuildSettings.switchCreateRomFile;
				set => EditorUserBuildSettings.switchCreateRomFile = value;
			}
			
			public static bool EnableDebugPad
			{
				get => EditorUserBuildSettings.switchEnableDebugPad;
				set => EditorUserBuildSettings.switchEnableDebugPad = value;
			}

			public static bool NVNGraphicsDebugger
			{
				get => EditorUserBuildSettings.switchNVNGraphicsDebugger;
				set => EditorUserBuildSettings.switchNVNGraphicsDebugger = value;
			}

			public static bool RedirectWritesToHostMount
			{
				get => EditorUserBuildSettings.switchRedirectWritesToHostMount;
				set => EditorUserBuildSettings.switchRedirectWritesToHostMount = value;
			}
			
			public static bool NetworkInterfaceManagerInitializeEnabled
			{
				get => PlayerSettings.Switch.networkInterfaceManagerInitializeEnabled;
				set => PlayerSettings.Switch.networkInterfaceManagerInitializeEnabled = value;
			}

			public static string NsoDependencies
			{
				get => PlayerSettings.Switch.nsoDependencies;
				set => PlayerSettings.Switch.nsoDependencies = value;
			}
			
			public static PlayerSettings.Switch.ScreenResolutionBehavior ScreenResolutionBehavior
			{
				get => PlayerSettings.Switch.screenResolutionBehavior;
				set => PlayerSettings.Switch.screenResolutionBehavior = value;
			}
			
			public static int SocketAllocatorPoolSize
			{
				get => PlayerSettings.Switch.socketAllocatorPoolSize;
				set => PlayerSettings.Switch.socketAllocatorPoolSize = value;
			}

			public static int SocketBufferEfficiency
			{
				get => PlayerSettings.Switch.socketBufferEfficiency;
				set => PlayerSettings.Switch.socketBufferEfficiency = value;
			}

			public static int SocketConcurrencyLimit
			{
				get => PlayerSettings.Switch.socketConcurrencyLimit;
				set => PlayerSettings.Switch.socketConcurrencyLimit = value;
			}

			public static bool SocketConfigEnabled
			{
				get => PlayerSettings.Switch.socketConfigEnabled;
				set => PlayerSettings.Switch.socketConfigEnabled = value;
			}

			public static bool SocketInitializeEnabled
			{
				get => PlayerSettings.Switch.socketInitializeEnabled;
				set => PlayerSettings.Switch.socketInitializeEnabled = value;
			}

			public static int SocketMemoryPoolSize
			{
				get => PlayerSettings.Switch.socketMemoryPoolSize;
				set => PlayerSettings.Switch.socketMemoryPoolSize = value;
			}
			
			public static int TcpAutoReceiveBufferSizeMax
			{
				get => PlayerSettings.Switch.tcpAutoReceiveBufferSizeMax;
				set => PlayerSettings.Switch.tcpAutoReceiveBufferSizeMax = value;
			}

			public static int TcpAutoSendBufferSizeMax
			{
				get => PlayerSettings.Switch.tcpAutoSendBufferSizeMax;
				set => PlayerSettings.Switch.tcpAutoSendBufferSizeMax = value;
			}

			public static int TcpInitialReceiveBufferSize
			{
				get => PlayerSettings.Switch.tcpInitialReceiveBufferSize;
				set => PlayerSettings.Switch.tcpInitialReceiveBufferSize = value;
			}

			public static int TcpInitialSendBufferSize
			{
				get => PlayerSettings.Switch.tcpInitialSendBufferSize;
				set => PlayerSettings.Switch.tcpInitialSendBufferSize = value;
			}

			public static int UdpReceiveBufferSize
			{
				get => PlayerSettings.Switch.udpReceiveBufferSize;
				set => PlayerSettings.Switch.udpReceiveBufferSize = value;
			}

			public static int UdpSendBufferSize
			{
				get => PlayerSettings.Switch.udpSendBufferSize;
				set => PlayerSettings.Switch.udpSendBufferSize = value;
			}
			
			public static bool UseSwitchCPUProfiler
			{
				get => PlayerSettings.Switch.useSwitchCPUProfiler;
				set => PlayerSettings.Switch.useSwitchCPUProfiler = value;
			}
		}
	}
}