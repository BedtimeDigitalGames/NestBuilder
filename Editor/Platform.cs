using UnityEditor;
using System;
using UnityEditor.Build;

namespace BedtimeCore.BuildPipeline
{

	[Serializable]
	public struct Platform
	{
		public BuildTarget buildTarget;
		public BuildTargetGroup AsBuildTargetGroup => BuildTargetGroupFromTarget(buildTarget);
		public NamedBuildTarget AsNamedBuildTarget => NamedBuildTarget.FromBuildTargetGroup(AsBuildTargetGroup);

		public Platform(BuildTarget buildTarget) => this.buildTarget = buildTarget;

		public static NamedBuildTarget GetNamedBuildTarget(BuildTargetGroup group) => NamedBuildTarget.FromBuildTargetGroup(group);
		public static NamedBuildTarget GetNamedBuildTarget(BuildTarget activeBuildTarget) => GetNamedBuildTarget(BuildTargetGroupFromTarget(activeBuildTarget));

		public static BuildTargetGroup BuildTargetGroupFromTarget(BuildTarget buildTarget)
		{
			switch (buildTarget)
			{
				case BuildTarget.StandaloneOSX:
				case BuildTarget.StandaloneWindows:
				case BuildTarget.StandaloneWindows64:
				case BuildTarget.StandaloneLinux64:
					return BuildTargetGroup.Standalone;
				case BuildTarget.WebGL:
					return BuildTargetGroup.WebGL;
				case BuildTarget.iOS:
					return BuildTargetGroup.iOS;
				case BuildTarget.Android:
					return BuildTargetGroup.Android;
				case BuildTarget.WSAPlayer:
					return BuildTargetGroup.WSA;
				case BuildTarget.PS4:
					return BuildTargetGroup.PS4;
				case BuildTarget.XboxOne:
					return BuildTargetGroup.XboxOne;
				case BuildTarget.tvOS:
					return BuildTargetGroup.tvOS;
				case BuildTarget.Switch:
					return BuildTargetGroup.Switch;
				case BuildTarget.GameCoreXboxOne:
					return BuildTargetGroup.GameCoreXboxOne;
				case BuildTarget.GameCoreXboxSeries:
					return BuildTargetGroup.GameCoreXboxSeries;
				case BuildTarget.PS5:
					return BuildTargetGroup.PS5;
			}
			return BuildTargetGroup.Unknown;
		}

	}
}