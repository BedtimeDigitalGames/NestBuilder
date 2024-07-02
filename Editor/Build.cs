using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace BedtimeCore.BuildPipeline
{
	[Serializable]
	public class Build : IEquatable<Build>
	{
		[SerializeField]
		private AssetBundlesStatus _assetBundlesStatus;

		[SerializeField]
		private BuildStep _buildStep;

		[SerializeField]
		private double _endTime;

		[SerializeField]
		private BuildStep _lastStep;

		[SerializeField]
		private double _startTime;

		[field:SerializeField]
		public SerializableBuildPlayerOptions PlayerOptions { get; private set; }

		[field:SerializeField]
		public BuildConfiguration Configuration { get; private set; }

		[field:SerializeField]
		public List<string> Metadata {get; private set;} = new List<string>();
		
		public Build(BuildConfiguration configuration, BuildPlayerOptions playerOptions)
		{
			this.Configuration = configuration;
			this.PlayerOptions = new SerializableBuildPlayerOptions(playerOptions);
			BuildStep = BuildStep.Ready;
		}

		public AssetBundlesStatus AssetBundlesStatus
		{
			get => _assetBundlesStatus;
			set => _assetBundlesStatus = value;
		}

		public string OutputFilePath => PlayerOptions.locationPathName;

		public string OutputDirectory
		{
			get
			{
				var path = PlayerOptions.locationPathName;
				if (Path.HasExtension(path))
				{
					return Path.GetDirectoryName(path);
				}

				return path;
			}
		}

		public DateTime? StartTime
		{
			get
			{
				if (_startTime == 0)
				{
					return null;
				}

				return DateTime.UtcNow.AddSeconds(_startTime - EditorApplication.timeSinceStartup);
			}
		}

		public DateTime? EndTime
		{
			get
			{
				if (_endTime == 0)
				{
					return null;
				}

				return DateTime.UtcNow.AddSeconds(_endTime - EditorApplication.timeSinceStartup);
			}
		}

		public BuildStep LastStep => _lastStep;

		public BuildStep BuildStep
		{
			get => _buildStep;
			set
			{
				_lastStep = _buildStep;
				if (_lastStep == BuildStep.Ready)
				{
					_startTime = EditorApplication.timeSinceStartup;
				}

				switch (value)
				{
					case BuildStep.Failed:
					case BuildStep.Unavailable:
					case BuildStep.Completed:
						_endTime = EditorApplication.timeSinceStartup;
						break;
				}

				_buildStep = value;
				Log(string.Format("State: {0}", ObjectNames.NicifyVariableName(_buildStep.ToString())));
			}
		}

		public bool Equals(Build other) => other != null && Configuration.name == other.Configuration.name;

		private void Log(string log, params object[] objects)
		{
			var format = "[Build] ({0}) {1}";
			Debug.Log(string.Format(format, Configuration.name, string.Format(log, objects)));
		}

		public static float BuildStepToProgress(BuildStep step)
		{
			switch (step)
			{
				case BuildStep.PreConfiguration:
					return .1f;
				case BuildStep.PostConfiguration:
					return .4f;
				case BuildStep.AddAssetBundles:
					return .5f;
				case BuildStep.PreBuild:
					return .6f;
				case BuildStep.Building:
					return .7f;
				case BuildStep.PostBuild:
					return .9f;
				case BuildStep.Completed:
					return 1f;
				default:
					return 0;
			}
		}

		public static Color BuildStepToColor(BuildStep step)
		{
			switch (step)
			{
				case BuildStep.Failed:
				case BuildStep.Unavailable:
					return Color.red;
				case BuildStep.Completed:
					return Color.green;
				case BuildStep.Ready:
					return Color.green * .7f;
				default:
					return EditorGUIUtility.isProSkin ? Color.white : Color.black;
			}
		}
	}
}