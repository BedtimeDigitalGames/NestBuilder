using System;
using System.ComponentModel;
using UnityEditor;

namespace BedtimeCore.BuildPipeline
{
	[Serializable]
	public class WebGlContainer : ISettingsContainer
	{
		[Category("WebGL")]
		public BoolSetting ThreadsSupport = new(x => PlayerSettings.WebGL.threadsSupport = x);

		[Category("WebGL")]
		public BoolSetting DataCaching = new(x => PlayerSettings.WebGL.dataCaching = x);

		[Category("WebGL")]
		public EnumSetting<WebGLDebugSymbolMode> DebugSymbols = new(x => PlayerSettings.WebGL.debugSymbolMode = x);

		[Category("WebGL")]
		public BoolSetting AnalyzeBuildSize = new(x => PlayerSettings.WebGL.analyzeBuildSize = x);

		[Category("WebGL")]
		public BoolSetting UseEmbeddedResources = new(x => PlayerSettings.WebGL.useEmbeddedResources = x);

		[Category("WebGL")]
		public BoolSetting NameFilesAsHashes = new(x => PlayerSettings.WebGL.nameFilesAsHashes = x);

		[Category("WebGL")]
		public StringSetting EmscriptenArgs = new(x => PlayerSettings.WebGL.emscriptenArgs = x);

		[Category("WebGL")]
		public EnumSetting<WebGLExceptionSupport> ExceptionSupport = new(x => PlayerSettings.WebGL.exceptionSupport = x);

		[Category("WebGL")]
		public EnumSetting<WebGLLinkerTarget> LinkerTarget = new(x => PlayerSettings.WebGL.linkerTarget = x);

		[Category("WebGL")]
		public EnumSetting<WebGLCompressionFormat> CompressionFormat = new(x => PlayerSettings.WebGL.compressionFormat = x);


	}
}