using System;
using UnityEditor;

namespace BedtimeCore.BuildPipeline
{

	[AttributeUsage(AttributeTargets.Field)]
	public class ScriptingBackendRestrictionAttribute : Attribute
	{
		private ScriptingImplementation backEnd;

		public ScriptingBackendRestrictionAttribute(ScriptingImplementation backEnd)
		{
			this.backEnd = backEnd;
		}

		public bool IsAllowed(ScriptingImplementation backEnd)
		{
			return this.backEnd == backEnd;
		}
	}

}