using System;
using System.ComponentModel;

namespace BedtimeCore.NestBuilder
{
	[Serializable]
	public class BuildStepsModule : ISettingsModule
	{
		[Category("BuildStep Methods")]
		public BuildProcessorSetting PreConfigurationStep = new(BuildStep.PreConfiguration);

		[Category("BuildStep Methods")]
		public BuildProcessorSetting PostConfigurationStep = new(BuildStep.PostConfiguration);

		[Category("BuildStep Methods")]
		public BuildProcessorSetting PreBuildStep = new(BuildStep.PreBuild);

		[Category("BuildStep Methods")]
		public BuildProcessorSetting PostBuildStep = new(BuildStep.PostBuild);
	}
}