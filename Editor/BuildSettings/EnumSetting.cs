using System;
using System.Reflection;
using UnityEditor;

namespace BedtimeCore.BuildPipeline
{
	[Serializable]
	public class EnumSetting<T>  : BuildSetting<T> where T : unmanaged, Enum
	{
		public EnumSetting(Action<T> applyAction = null) : base(applyAction)
		{
		}

		public override void DrawValue(BuildConfiguration topLevel, SerializedProperty property)
		{
			if (typeof(T).GetCustomAttribute<FlagsAttribute>() != null)
			{
				value = (T)EditorGUILayout.EnumFlagsField(value);
			}
			else
			{
				value = (T)EditorGUILayout.EnumPopup(value);
			}
		}
	}
}