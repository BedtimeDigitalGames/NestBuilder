using UnityEditor;
using System;

namespace BedtimeCore.BuildPipeline
{

	[Serializable]
	public struct SerializableBuildPlayerOptions
	{
		public string assetBundleManifestPath;
		public string locationPathName;
		public BuildOptions options;
		public string[] scenes;
		public BuildTarget target;
		public BuildTargetGroup targetGroup;

		public SerializableBuildPlayerOptions(BuildPlayerOptions pOptions)
		{
			assetBundleManifestPath = pOptions.assetBundleManifestPath;
			locationPathName = pOptions.locationPathName;
			options = pOptions.options;
			scenes = pOptions.scenes;
			target = pOptions.target;
			targetGroup = pOptions.targetGroup;
		}

		public BuildPlayerOptions BuildPlayerOptions
		{
			get
			{
				var pOptions = new BuildPlayerOptions();

				pOptions.assetBundleManifestPath = assetBundleManifestPath;
				pOptions.locationPathName = locationPathName;
				pOptions.options = options;
				pOptions.scenes = scenes;
				pOptions.target = target;
				pOptions.targetGroup = targetGroup;

				return pOptions;
			}
		}
	}
}