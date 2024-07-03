using System;
using System.ComponentModel;
using UnityEditor;

namespace BedtimeCore.NestBuilder
{
	[Serializable]
	public class PS4Module : ISettingsModule
	{
		[Category("PS4")]
		public StringSetting ContentID = new(x => PlayerSettings.PS4.contentID = x);

		[Category("PS4")]
		public StringSetting AppVersion = new(x => PlayerSettings.PS4.appVersion = x);

		[Category("PS4")]
		public StringSetting MasterVersion = new(x => PlayerSettings.PS4.masterVersion = x);

		[Category("PS4")]
		public StringSetting NpTitlePath = new(x => PlayerSettings.PS4.NPtitleDatPath = x);

		[Category("PS4")]
		public StringSetting NpTrophyPackPath = new(x => PlayerSettings.PS4.npTrophyPackPath = x);

		[Category("PS4")]
		public IntSetting ParentalLockLevel = new(x => PlayerSettings.PS4.parentalLevel = x);
		
	}
}