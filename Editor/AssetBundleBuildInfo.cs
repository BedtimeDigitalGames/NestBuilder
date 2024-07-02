#if BEDTIME_ASSET_SYSTEM
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using BedtimeCore.Assets;
using UnityEditor;

namespace BedtimeCore.BuildPipeline
{
	[Serializable]
	public class AssetBundleBuildInfo
	{
		public BuildAssetBundleOptions buildAssetBundleOptions;
		public bool buildAssetBundles;
		public bool copyAssetBundles;
		public BundleIncludeManifest includeManifest;
		public string AssetBundlesSource => $"{EditorAssetManager.AssetBundleBuildPathRoot}/{EditorUserBuildSettings.activeBuildTarget}";
		public string AssetBundlesDestination => $"{AssetManager.AssetBundlesRuntimePath}";
	}
}
#endif