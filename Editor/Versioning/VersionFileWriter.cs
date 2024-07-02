#if BEDTIME_VERSIONING
using BedtimeCore.BuildPipeline;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace BedtimeCore.Versioning.Editor
{
	public class VersionFileWriter : IBuildProcessor
	{
		private const string SMALL_VERSION_FILE_NAME = "version.txt";
		private const string FULL_VERSION_FILE_NAME = "version-data.json";

		public BuildStep ValidBuildSteps => BuildStep.PostBuild;

		public Task<bool> ProcessBuild(Build build) => Task.FromResult(WriteVersionFile(build));

		private static bool WriteVersionFile(Build build)
		{
			BuildPlayerOptions buildPlayerOptions = build.PlayerOptions.BuildPlayerOptions;
			if (EditorUserBuildSettings.activeBuildTarget != buildPlayerOptions.target)
			{
				Debug.LogError("Cannot create versioning file as build targets does not match.");
				return false;
			}
			Version version = Version.GetVersion();
			if (!AssertVersionIsValid(version, build))
			{
				return false;
			}
			string directory = Path.GetDirectoryName(buildPlayerOptions.locationPathName);
			bool fileWriteSuccess = WriteFile(Path.Combine(directory, SMALL_VERSION_FILE_NAME), version.ToVersionString());
			fileWriteSuccess &= WriteFile(Path.Combine(directory, FULL_VERSION_FILE_NAME), JsonUtility.ToJson(version, true));
			return fileWriteSuccess;
		}

		private static bool WriteFile(string filePath, string content)
		{
			try
			{
				using (StreamWriter writer = new StreamWriter(new FileStream(filePath, FileMode.Create, FileAccess.Write), Encoding.UTF8))
				{
					writer.Write(content);
				}
				Debug.LogFormat("Wrote file '{0}'", filePath);
				return true;
			}
			catch (Exception exception)
			{
				Debug.LogErrorFormat("Could not write file '{0}': {1}", filePath ?? "NULL", exception);
			}
			return false;
		}

		private static bool AssertVersionIsValid(Version version, Build build)
		{
			if (version == null)
			{
				Debug.LogError("Cannot create versioning file as version is null.");
				return false;
			}
			return true;
		}

	}
}
#endif