#if BEDTIME_VERSIONING
using System.Threading.Tasks;
using BedtimeCore.NestBuilder;

namespace BedtimeCore.Versioning.Editor
{
	public class VersionUpdater : IBuildProcessor
	{
		public BuildStep ValidBuildSteps => BuildStep.PreBuild;

		public Task<bool> ProcessBuild(Build build) => Task.FromResult(UpdateVersion(build));

		public static bool UpdateVersion(Build build)
		{
			EditorVersionManager.UpdateVersion();
			return true;
		}
	}
}
#endif