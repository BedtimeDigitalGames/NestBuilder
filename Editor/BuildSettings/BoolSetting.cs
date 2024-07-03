using UnityEditor;
using System;

namespace BedtimeCore.NestBuilder
{
	[Serializable]
	public class BoolSetting : BuildSetting<bool>
	{
		public BoolSetting(Action<bool> applyAction = null) : base(applyAction)
		{
		}

		public override void DrawValue(BuildConfiguration topLevel, SerializedProperty property)
		{
			value = EditorGUILayout.Toggle("Active", value);
		}

		public static implicit operator bool(BoolSetting instance)
		{
			return instance?.Value ?? false;
		}
	}
}
