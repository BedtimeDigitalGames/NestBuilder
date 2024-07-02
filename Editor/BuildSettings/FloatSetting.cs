using UnityEditor;
using System;

namespace BedtimeCore.BuildPipeline
{

	[Serializable]
	public class FloatSetting : BuildSetting<float>
	{
		public FloatSetting(Action<float> applyAction) : base(applyAction)
		{
		}

		public override void DrawValue(BuildConfiguration topLevel, SerializedProperty property)
		{
			value = EditorGUILayout.FloatField("", value);
		}
	}
}
