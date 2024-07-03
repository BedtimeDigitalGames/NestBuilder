using UnityEditor;
using System;

namespace BedtimeCore.NestBuilder
{
	[Serializable]
	public class BuildAssetBundleOptionsSetting : BuildSetting<BuildAssetBundleOptions>
	{
		private const BuildAssetBundleOptions DEFAULT_BUILD_OPTIONS = BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DisableWriteTypeTree | BuildAssetBundleOptions.DisableLoadAssetByFileName | BuildAssetBundleOptions.DisableLoadAssetByFileNameWithExtension;

		public BuildAssetBundleOptionsSetting() : base(null)
		{
			value = DEFAULT_BUILD_OPTIONS;
		}

		public override void DrawValue(BuildConfiguration topLevel, SerializedProperty property)
		{
			value = (BuildAssetBundleOptions)EditorGUILayout.EnumFlagsField("AssetBundle Build Options", value);
		}
	}
}