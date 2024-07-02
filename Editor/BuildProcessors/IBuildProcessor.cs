using System.Threading.Tasks;

namespace BedtimeCore.BuildPipeline
{
	public interface IBuildProcessor
	{
		BuildStep ValidBuildSteps { get; }
		Task<bool> ProcessBuild(Build build);
	}
}