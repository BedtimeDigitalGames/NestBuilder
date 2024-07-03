using System.IO;
using System.Threading.Tasks;

namespace BedtimeCore.NestBuilder
{
	public class AssetBundleBackup : IBuildProcessor
	{
		public BuildStep ValidBuildSteps => BuildStep.PostBuild;

		private const string BUNDLE_BACKUP_PATH_NAME = "AssetBundles";
		
		public Task<bool> ProcessBuild(Build build)
		{
#if BEDTIME_ASSET_SYSTEM
			var assetBundlesSetting = build.Configuration.BuildSettings.assetBundlesCopy;
			var settings = assetBundlesSetting.Value;

			if (!assetBundlesSetting.IsSet || !settings.copyAssetBundles)
			{
				return Task.FromResult(true);
			}

			var source = settings.AssetBundlesSource;
			var destination = $"{build.OutputDirectory}/{BUNDLE_BACKUP_PATH_NAME}";

			if (!string.IsNullOrEmpty(source))
			{
				try
				{
					CopyAssetBundles(source, destination);
				}
				catch (System.Exception e)
				{
					$"Failed to backup AssetBundles to {destination}!\n Error: {e}".LogError();
					return Task.FromResult(false);
				}
			}
#endif
			return Task.FromResult(true);
		}

		private void CopyAssetBundles(string source, string destination)
		{
			foreach (string dirPath in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
			{
				Directory.CreateDirectory(dirPath.Replace(source, destination));
			}

			foreach (string newPath in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
			{
				File.Copy(newPath, newPath.Replace(source, destination), true);
			}
		}
	}
}