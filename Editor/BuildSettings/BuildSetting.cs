using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

namespace BedtimeCore.BuildPipeline
{
	[Serializable]
	public abstract class BuildSetting<T> : IBuildSetting, IEquatable<BuildSetting<T>>
	{
		public BuildConfiguration Owner { get; set; }
		public string Name { get; set; }
		[SerializeField]
		protected bool isSet;
		[SerializeField]
		protected T value;
		protected Attribute[] Attributes { get; set; }
		public Action<T> ApplyAction { get; set; }
		
		public void Initialize(string name, BuildConfiguration owner, Attribute[] attributes)
		{
			this.Name = name;
			this.Owner = owner;
			this.Attributes = attributes;
		}

		public virtual T ValueSelf => !IsSetSelf ? DefaultValue : value;

		public virtual T Value => Owner.GetCascadedSetting<BuildSetting<T>>(Name).ValueSelf;

		public virtual T DefaultValue => default;

		public bool IsSet => Owner.GetCascadedSetting<BuildSetting<T>>(Name).IsSetSelf;
		public bool IsSetSelf
		{
			get => isSet;
			set => isSet = value;
		}

		public virtual void ApplyToEditor(BuildConfiguration configuration)
		{
			if (ApplyAction != null)
			{
				ApplyAction.Invoke(ValueSelf);
			}
		}

		public BuildSetting<T> GetCascaded() => Owner.GetCascadedSetting<BuildSetting<T>>(Name);

		protected virtual T TryCopy(T from) => @from is ValueType ? @from : DefaultValue;

		public void OnGUI()
		{
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			EditorGUILayout.BeginHorizontal();

			var cascadedSetting = Owner.GetCascadedSetting<BuildSetting<T>>(Name);
			var parentOwner = cascadedSetting != null && cascadedSetting != this;

			var willSet = (GUILayout.Toggle(IsSetSelf, "", GUILayout.MaxWidth(12)));
			if (willSet ^ isSet)
			{
				var obj = new SerializedObject(Owner);
				obj.Update();
				value = willSet && cascadedSetting.IsSetSelf ? TryCopy(cascadedSetting.ValueSelf) : DefaultValue;
				obj.ApplyModifiedProperties();
			}

			IsSetSelf = willSet;
			
			var tooltipAttribute = GetAttribute<TooltipAttribute>();
			string tooltip = tooltipAttribute != null ? tooltipAttribute.tooltip : "";

			var label = new GUIContent(ObjectNames.NicifyVariableName(Name), tooltip);

			EditorGUILayout.LabelField(label, EditorStyles.boldLabel);

			if (cascadedSetting != null && cascadedSetting != this && cascadedSetting.isSet)
			{
				EditorGUI.BeginDisabledGroup(!parentOwner);

				if (GUILayout.Button($"Defined in \"{cascadedSetting.Owner.name}\"", EditorStyles.miniButton, GUILayout.Height(15)))
				{
					Selection.activeObject = cascadedSetting.Owner;
				}

				EditorGUI.EndDisabledGroup();
			}

			if (GUILayout.Button("x", EditorStyles.miniButton, GUILayout.MaxWidth(20), GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight)))
			{
				value = GetEditorValue();
			}
			
			EditorGUILayout.EndHorizontal();

			EditorGUI.BeginDisabledGroup(parentOwner || !IsSetSelf);
			if (cascadedSetting != null)
			{
				var obj = new SerializedObject(cascadedSetting.Owner);
				obj.Update();
				cascadedSetting.DrawValue(Owner, GetProperty(obj));
				obj.ApplyModifiedProperties();
			}
			EditorGUI.EndDisabledGroup();

			EditorGUILayout.EndVertical();
		}

		private SerializedProperty GetProperty(SerializedObject configuration)
		{
			try
			{
				var container = configuration.FindProperty("buildSettings");
				var prop = container.FindPropertyRelative(Name);
				return prop.FindPropertyRelative(nameof(value)).Copy();
			}
			catch (Exception)
			{
				return null;
			}
		}

		public abstract void DrawValue(BuildConfiguration topLevel, SerializedProperty property);

		public TAttribute GetAttribute<TAttribute>() where TAttribute : Attribute
		{
			return Attributes.Where(m => typeof(TAttribute).IsAssignableFrom(m.GetType())).FirstOrDefault() as TAttribute;
		}
		public bool HasAttribute<TAttribute>() where TAttribute : Attribute
		{
			return GetAttribute<TAttribute>() != null;
		}

		bool IEquatable<BuildSetting<T>>.Equals(BuildSetting<T> other)
		{
			return other != null && this.Name == other.Name;
		}

		public override bool Equals(object obj)
		{
			var other = obj as IBuildSetting;
			return other != null && this.Name == other.Name && this.GetType() == other.GetType();
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public BuildSetting(Action<T> applyAction = null)
		{
			this.ApplyAction = applyAction;
		}

		private static GUIStyle toggleButtonActive;
		private static GUIStyle ToggleButtonActive
		{
			get
			{
				if (toggleButtonActive == null)
				{
					toggleButtonActive = new GUIStyle(EditorStyles.miniButton);
					toggleButtonActive.normal.background = toggleButtonActive.active.background;
				}
				return toggleButtonActive;
			}
		}
		private static GUIStyle toggleButtonInactive;
		private static GUIStyle ToggleButtonInactive
		{
			get
			{
				if (toggleButtonInactive == null)
				{
					toggleButtonInactive = new GUIStyle(EditorStyles.miniButton);
				}
				return toggleButtonInactive;
			}
		}

		public virtual bool WillTriggerRecompile
		{
			get
			{
				return false;
			}
		}

		public virtual T GetEditorValue() => default;
	}
}