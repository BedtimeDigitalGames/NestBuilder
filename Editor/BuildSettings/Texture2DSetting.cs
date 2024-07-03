using UnityEngine;
using UnityEditor;
using System;

namespace BedtimeCore.NestBuilder
{
	[Serializable]
	public class Texture2DSetting : BuildSetting<Texture2D>
	{
		public Texture2DSetting(Action<Texture2D> applyAction) : base(applyAction)
		{
		}

		public override void DrawValue(BuildConfiguration topLevel, SerializedProperty property)
		{
			value = EditorGUILayout.ObjectField(value, typeof(Texture2D), false) as Texture2D;
		}
	}
}
