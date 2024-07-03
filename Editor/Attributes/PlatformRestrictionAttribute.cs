using System;
using System.Collections;
using UnityEditor;

namespace BedtimeCore.NestBuilder
{

	[AttributeUsage(AttributeTargets.Field)]
	public class PlatformRestrictionAttribute : Attribute
	{
		private BuildTargetGroup[] targetGroup;
		private BuildTarget[] target;

		public PlatformRestrictionAttribute(params BuildTargetGroup[] targetGroup)
		{
			this.targetGroup = targetGroup;
		}

		public PlatformRestrictionAttribute(params BuildTarget[] target)
		{
			this.target = target;
		}

		public bool IsAllowed(Platform platform)
		{
			if (targetGroup != null)
			{
				return ((IList) targetGroup).Contains(platform.AsBuildTargetGroup);
			}
			if (target != null)
			{
				return ((IList) target).Contains(platform.buildTarget);
			}
			return false;
		}
	}

}