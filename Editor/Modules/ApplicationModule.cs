using System;
using System.ComponentModel;
using UnityEditor;

namespace BedtimeCore.NestBuilder
{
	[Serializable]
	public class ApplicationModule : ISettingsModule
	{
		[Category("Application")]
		public StringSetting CompanyName = new(x => PlayerSettings.companyName = x);

		[Category("Application")]
		public StringSetting ProductName = new(x => PlayerSettings.productName = x);

		[Category("Application")]
		public StringSetting Version = new(x => BuildSettingMapper.Version = x);
		
		[Category("Application")]
		public BoolSetting ShowSplashScreen = new(x => PlayerSettings.SplashScreen.show = x);
		
		[Category("Application")]
		public StringSetting ApplicationIdentifier = new(x => PlayerSettings.SetApplicationIdentifier(Platform.GetNamedBuildTarget(EditorUserBuildSettings.activeBuildTarget), x));

#if BEDTIME_METADATA
		[Category("Application")]
		public MetadataSetting Metadata = new(BedtimeCore.Persistence.EditorMetadata.ClearAndApply);
#endif

	}
}