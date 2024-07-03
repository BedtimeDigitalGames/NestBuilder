#if BEDTIME_ASSET_SYSTEM
using UnityEditor;
using System;
using BedtimeCore.Assets;

namespace BedtimeCore.NestBuilder
{
	[Serializable]
	public sealed class AssetBundleBuildInfoSetting : BuildSetting<AssetBundleBuildInfo>
	{
		public AssetBundleBuildInfoSetting(Action<AssetBundleBuildInfo> applyAction = null) : base(applyAction)
		{
			value = new AssetBundleBuildInfo();
		}

		public override AssetBundleBuildInfo DefaultValue => new AssetBundleBuildInfo();

		public override void DrawValue(BuildConfiguration topLevel, SerializedProperty property)
		{
			ValueSelf.buildAssetBundles = EditorGUILayout.Toggle("Build dirty AssetBundles", ValueSelf.buildAssetBundles);

			ValueSelf.buildAssetBundleOptions = (BuildAssetBundleOptions)EditorGUILayout.EnumFlagsField("AssetBundle Build Options", ValueSelf.buildAssetBundleOptions);

			ValueSelf.copyAssetBundles = EditorGUILayout.Toggle("Copy AssetBundles from source into destination", ValueSelf.copyAssetBundles);

			ValueSelf.includeManifest = EditorGUILayout.ObjectField("Include Manifest", ValueSelf.includeManifest, typeof(BundleIncludeManifest), false) as BundleIncludeManifest;
		}
	}
}
#endif