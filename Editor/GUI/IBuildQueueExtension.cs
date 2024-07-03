namespace BedtimeCore.NestBuilder
{
	public interface IBuildQueueExtension
	{
		bool ShouldDisplay(Build build);
		void OnGUI(Build build);
	}
}