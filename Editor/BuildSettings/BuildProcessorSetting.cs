using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditorInternal;
using System.Linq;
using BedtimeCore.SteamUploader;

namespace BedtimeCore.NestBuilder
{
	[Serializable]
	public class BuildProcessorSetting : BuildSetting<List<IBuildProcessor>>
	{
		[field:SerializeField]
		public BuildStep BuildStep { get; }

		public int selected;
		private ReorderableList reorderableList;
		private List<IBuildProcessor> validImplementations;
		private List<IBuildProcessor> allImplementations;


		[SerializeReference]
		private List<IBuildProcessor> selectedImplementations = new List<IBuildProcessor>();

		public override List<IBuildProcessor> ValueSelf => !IsSetSelf ? DefaultValue : selectedImplementations;


		public override void ApplyToEditor(BuildConfiguration configuration)
		{
			//This space intentionally left blank
		}

		public BuildProcessorSetting(BuildStep buildStep) : base(null) => this.BuildStep = buildStep;

		public override List<IBuildProcessor> DefaultValue => new List<IBuildProcessor>();
		

		public override void DrawValue(BuildConfiguration topLevel, SerializedProperty property)
		{
			Initialize();
			reorderableList.DoLayoutList();
		}

		private void Initialize()
		{
			if (!IsSetSelf && selectedImplementations.Any())
			{
				selectedImplementations = DefaultValue;	
			}

			if (allImplementations == null || validImplementations == null)
			{
				allImplementations = GetBuildProcessors();
				validImplementations = allImplementations.Where(i => (i.ValidBuildSteps & BuildStep) != 0).ToList();
			}

			if (reorderableList == null || reorderableList.list != ValueSelf)
			{
				reorderableList = new ReorderableList(ValueSelf, typeof(IBuildProcessor), true, true, false, true)
				{
					drawElementCallback = DrawListElement,
					drawHeaderCallback = DrawListHeader,
				};
			}
		}

		private void DrawListHeader(Rect rect)
		{
			const float BUTTON_WIDTH = 40;
			if (selected >= 0 && selected < validImplementations.Count)
			{
				using (new GUILayout.HorizontalScope())
				{
					var selectionRect = rect;
					selectionRect.xMax -= BUTTON_WIDTH * 2;
					selected = EditorGUI.Popup(selectionRect, selected, validImplementations.Select(i => i.GetType().Name).ToArray());

					var btnRect = rect;
					btnRect.xMin = rect.xMax - BUTTON_WIDTH;
					GUI.SetNextControlName("ClearButton");
					if (GUI.Button(btnRect, "Clear", EditorStyles.miniButton))
					{
						selectedImplementations.Clear();
					}
					btnRect.x -= BUTTON_WIDTH;
					GUI.SetNextControlName("AddButton");
					if (GUI.Button(btnRect, "Add", EditorStyles.miniButton))
					{
						selectedImplementations.Add(validImplementations[selected]);
					}
				}
			}
			else
			{
				selected = 0;
				EditorGUI.LabelField(rect, $"Create a class that implements {nameof(IBuildProcessor)}");
			}
		}

		private void DrawListElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			if (index < selectedImplementations.Count)
			{
				Color originalColor = GUI.contentColor;
				var text = selectedImplementations[index]?.GetType().Name ?? "";
				if (selectedImplementations[index] == null)
				{
					text = "[INVALID] " + text;
					GUI.contentColor = Color.red;
				}
				EditorGUI.LabelField(rect, text, MethodStyle);
				GUI.contentColor = originalColor;
			}
		}

		private List<IBuildProcessor> GetBuildProcessors()
		{
			var list = new List<IBuildProcessor>();
			var implementations = TypeCache.GetTypesDerivedFrom<IBuildProcessor>();
			foreach (var impl in implementations)
			{
				try
				{
					var instance = (IBuildProcessor) Activator.CreateInstance(impl);
					list.Add(instance);
				}
				catch (Exception)
				{
				}
			}

			return list;
		}

		private GUIStyle methodStyle = GUIStyle.none;

		private GUIStyle MethodStyle
		{
			get
			{
				if (methodStyle == GUIStyle.none)
				{
					methodStyle = (EditorStyles.helpBox);
					methodStyle.alignment = TextAnchor.MiddleLeft;
					methodStyle.fontStyle = FontStyle.Bold;
				}
				return methodStyle;
			}
		}
	}
}
