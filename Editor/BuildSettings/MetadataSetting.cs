#if BEDTIME_METADATA
using System;
using BedtimeCore.Persistence;
using UnityEditor;

namespace BedtimeCore.BuildPipeline
{
	[Serializable]
	public class MetadataSetting : BuildSetting<Metadata>
	{
		public MetadataSetting(Action<Metadata> applyAction) : base(applyAction)
		{
			
		}

		public override void DrawValue(BuildConfiguration topLevel, SerializedProperty property)
		{
			if (property != null)
			{
				EditorGUILayout.PropertyField(property);
				EditorGUILayout.Space();
			}
		}
	}
}
#endif