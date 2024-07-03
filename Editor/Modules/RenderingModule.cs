using System;
using System.ComponentModel;
using UnityEditor;

namespace BedtimeCore.NestBuilder
{
	[Serializable]
	public class RenderingModule : ISettingsModule
	{
		[Category("Rendering")]
		public BoolSetting MultiThreadedRendering = new(x => PlayerSettings.MTRendering = x);

		[Category("Rendering")]
		public BoolSetting Use32BitDisplayBuffer = new(x => PlayerSettings.use32BitDisplayBuffer = x);

		[Category("Rendering")]
		public BoolSetting UseHDRDisplay = new(x => PlayerSettings.useHDRDisplay = x);
		
		[Category("Rendering")]
		public BoolSetting GPUSkinning = new(x => PlayerSettings.gpuSkinning = x);

		[Category("Rendering")]
		public BoolSetting GraphicsJobs = new(x => PlayerSettings.graphicsJobs = x);
	}
}