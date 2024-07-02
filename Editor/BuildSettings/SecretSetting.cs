using System;
using UnityEditor;
using UnityEngine;

namespace BedtimeCore.BuildPipeline
{
	[Serializable]
	public class SecretSetting : BuildSetting<string>
	{
		private bool _hideSecret;
		private string _defaultValue;
		public SecretSetting(string defaultValue, bool hideSecret = true)
		{
			_hideSecret = hideSecret;
			_defaultValue = defaultValue ?? string.Empty;
		}
        
        public SecretSetting(string defaultValue, bool hideSecret, Action<string> applyAction) : this(defaultValue, hideSecret)
        {
            void Action(string _)
            {
                TryGetValue(out var output);
                applyAction(output);
            }

            ApplyAction = Action;
        }

        public SecretSetting(string defaultValue, Action<string> applyAction) : this(defaultValue, true, applyAction)
        {
        }

        public bool TryGetValue(out string output) => Secrets.TryGetValue(Value, out output);
        

        public override void DrawValue(BuildConfiguration topLevel, SerializedProperty property)
		{
			if (!isSet)
			{
				return;
			}
			
			EditorGUILayout.BeginVertical();
			value = EditorGUILayout.DelayedTextField("Key name",value);
			if(string.IsNullOrEmpty(value))
			{
				value = _defaultValue;
			}
			
			Secrets.TryGetLocal(value, out var secret);

			string newSecret;
			if (_hideSecret)
			{
				newSecret = EditorGUILayout.PasswordField("Secret", secret);
			}
			else
			{
				newSecret = EditorGUILayout.TextField("Secret", secret);
			}
			
			if (newSecret != secret && !string.IsNullOrEmpty(newSecret))
			{
				Secrets.SetLocal(value, newSecret);
			}

			EditorGUILayout.EndVertical();
		}
	}
}