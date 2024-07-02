using System;
using UnityEditor;

namespace BedtimeCore.BuildPipeline
{
	public interface IBuildSetting
	{
		string Name { get; set; }
		BuildConfiguration Owner { get; set; }
		bool IsSetSelf { get; set; }
		bool WillTriggerRecompile { get; }

		void Initialize(string name, BuildConfiguration owner, Attribute[] attributes);
		void OnGUI();
		void DrawValue(BuildConfiguration topLevel, SerializedProperty property);
		void ApplyToEditor(BuildConfiguration configuration);

		bool HasAttribute<T>() where T : Attribute;
		T GetAttribute<T>() where T : Attribute;
	}
}