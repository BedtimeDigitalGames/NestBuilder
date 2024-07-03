using System;
using System.ComponentModel;

namespace BedtimeCore.NestBuilder
{
	[Serializable]
	public class EpicModule : ISettingsModule
	{
		[Category("Epic")]
		public StringSetting ProductId;

		[Category("Epic")]
		public StringSetting ClientId;

		[Category("Epic")]
		public SecretSetting ClientSecret = new("EPIC_CLIENT_SECRET", false);

		[Category("Epic")]
		public SecretSetting EncryptionKey = new("EPIC_ENCRYPTION_KEY", false);
	}
}