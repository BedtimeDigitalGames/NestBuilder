using System;
using UnityEditor;

namespace BedtimeCore.NestBuilder
{
    [Serializable]
    public class ObjectFieldSetting<T> : BuildSetting<T> where T : UnityEngine.Object
    {
        public ObjectFieldSetting(Action<T> applyAction  = null) : base(applyAction)
        {
        }

        public override void DrawValue(BuildConfiguration topLevel, SerializedProperty property)
        {
            value = (T) EditorGUILayout.ObjectField("", value, typeof(T), false);
        }
    }
}