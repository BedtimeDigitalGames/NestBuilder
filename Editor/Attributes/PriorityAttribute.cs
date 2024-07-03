using System;

namespace BedtimeCore.NestBuilder
{
	[AttributeUsage(AttributeTargets.Field)]
	public class PriorityAttribute : Attribute
	{
		public float Priority { get; set; }
		public PriorityAttribute(float priority)
		{
			this.Priority = priority;
		}
	}
}