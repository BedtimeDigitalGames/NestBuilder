using System.Threading.Tasks;

namespace BedtimeCore.NestBuilder
{
	public interface IBuildProcessor
	{
		BuildStep ValidBuildSteps { get; }
		Task<bool> ProcessBuild(Build build);
	}
}