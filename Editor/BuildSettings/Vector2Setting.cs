using UnityEngine;
using UnityEditor;
using System;

namespace BedtimeCore.NestBuilder
{
	[Serializable]
	public class Vector2Setting : BuildSetting<Vector2>
	{
		public Vector2Setting(Action<Vector2> applyAction) : base(applyAction)
		{
		}

		public override void DrawValue(BuildConfiguration topLevel, SerializedProperty property)
		{
			value = EditorGUILayout.Vector2Field("", value);
		}
	}
}
