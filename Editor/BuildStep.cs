using System;

namespace BedtimeCore.NestBuilder
{
	[Flags]
	public enum BuildStep
	{
		Unavailable = 1 << 0,
		Ready = 1 << 1,
		Waiting = 1 << 2,
		PreConfiguration = 1 << 3,
		PostConfiguration = 1 << 4,
		AddAssetBundles = 1 << 5,
		PreBuild = 1 << 6,
		Building = 1 << 7,
		PostBuild = 1 << 8,
		Failed = 1 << 9,
		Completed = 1 << 10,
	}
}