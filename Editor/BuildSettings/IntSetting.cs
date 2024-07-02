using UnityEditor;
using System;

namespace BedtimeCore.BuildPipeline
{

	[Serializable]
	public class IntSetting : BuildSetting<int>
	{
		public IntSetting(Action<int> applyAction = null) : base(applyAction)
		{
		}

		public override void DrawValue(BuildConfiguration topLevel, SerializedProperty property)
		{
			value = EditorGUILayout.IntField("", value);
		}
	}
}
