using UnityEditor;
using System;

namespace BedtimeCore.NestBuilder
{
	[Serializable]
	public class PlatformSetting : BuildSetting<Platform>
	{
		public PlatformSetting(Action<Platform> applyAction) : base(applyAction)
		{
		}

		public override void DrawValue(BuildConfiguration topLevel, SerializedProperty property)
		{
			value.buildTarget = (BuildTarget)EditorGUILayout.EnumPopup("Platform", value.buildTarget);
			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.EnumPopup("Platform Group", value.AsBuildTargetGroup);
			EditorGUI.EndDisabledGroup();
		}
	}
}
