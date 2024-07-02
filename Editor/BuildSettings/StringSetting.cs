using UnityEditor;
using System;

namespace BedtimeCore.BuildPipeline
{
	[Serializable]
	public class StringSetting : BuildSetting<string>
	{
		public StringSetting(Action<string> applyAction  = null) : base(applyAction)
		{
		}

		public override void DrawValue(BuildConfiguration topLevel, SerializedProperty property)
		{
			value = EditorGUILayout.TextField(value);
		}
	}
}
