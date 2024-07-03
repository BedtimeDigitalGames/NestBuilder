using System;
using System.ComponentModel;

namespace BedtimeCore.NestBuilder
{
	[Serializable]
	public class SteamModule : ISettingsModule
	{
		[Category("Steam")]
		public StringSetting SetLiveOnBranch = new();

		[Category("Steam")]
		public IntSetting AppID = new();

		[Category("Steam")]
		public IntSetting DepotID = new();

		[Category("Steam")]
		public SecretSetting SDKPath = new("STEAM_SDK_PATH", false);

		[Category("Steam")]
		public SecretSetting Login = new("STEAM_BUILDER_LOGIN", false);

		[Category("Steam")]
		public SecretSetting Password = new("STEAM_BUILDER_LOGIN_CODE", hideSecret: true);

	}
}