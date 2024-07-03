using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using UnityEditorInternal;
using System.Linq;
#if BEDTIME_ASSET_SYSTEM
using BedtimeCore.Assets;
#endif

namespace BedtimeCore.NestBuilder
{
	[Serializable]
	public class SceneSetting : BuildSetting<string[]>
	{
		public List<SceneAsset> scenes = new List<SceneAsset>();
		public ReorderableList reorderableList;

		public override string[] ValueSelf
		{
			get
			{
				var output = new string[scenes.Count];
				for (int i = 0; i < scenes.Count; i++)
				{
					output[i] = AssetDatabase.GetAssetPath(scenes[i]);
				}
				return output;
			}
		}

		public SceneSetting(Action<string[]> applyAction = null) : base(applyAction)
		{
		}

		public override void DrawValue(BuildConfiguration topLevel, SerializedProperty property)
		{
			Initialize();
			
#if BEDTIME_ASSET_SYSTEM && BEDTIME_STORAGE
			if(topLevel.BuildSettings.assetBundlesCopy.Value.copyAssetBundles)
			{
				var messageType = reorderableList?.list?.Count > 1 ? MessageType.Warning : MessageType.Info;
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.HelpBox("You are using AssetBundles. You should only have an empty scene here.", messageType);
				if (GUILayout.Button("Get Empty", GUILayout.ExpandHeight(true)))
				{
					var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorAssetManager.EditorModeEmptyScene.Value);
					scenes.Clear();
					scenes.Add(scene);
				}
				EditorGUILayout.EndHorizontal();
			}
#endif
			reorderableList.DoLayoutList();
		} 

		public override string[] DefaultValue
		{
			get
			{
				return EditorBuildSettings.scenes.Select(s => s.path).ToArray();
			}
		}

		private void Initialize()
		{
			if (scenes == null)
			{
				scenes = new List<SceneAsset>();
			}
			if (reorderableList == null)
			{
				reorderableList = new ReorderableList(scenes, typeof(SceneAsset), true, true, true, true);
				reorderableList.drawElementCallback = DrawListElement;
				reorderableList.drawHeaderCallback = DrawListHeader;
				reorderableList.onAddCallback += OnAdd;
			}
		}

		private void OnAdd(ReorderableList list)
		{
			if (list.serializedProperty != null)
			{
				++list.serializedProperty.arraySize;
				list.index = list.serializedProperty.arraySize - 1;
			}
			else
			{
				list.list.Add(null);
			}
		}

		private void DrawListHeader(Rect rect)
		{
			Event e = Event.current;
			EditorGUI.LabelField(rect, "Drag here", EditorStyles.boldLabel);
			switch (e.type)
			{
				case EventType.DragUpdated:
				case EventType.DragPerform:
					if (!rect.Contains(e.mousePosition))
					{
						return;
					}

					DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

					if (e.type == EventType.DragPerform)
					{
						DragAndDrop.AcceptDrag();

						foreach (var item in DragAndDrop.objectReferences)
						{
							var scene = item as SceneAsset;
							if (scene != null)
							{
								scenes.Add(scene);
							}
						}
					}
					break;
			}

			var btnRect = rect;
			btnRect.xMin = rect.xMax - 40;
			GUI.SetNextControlName("ClearButton");
			if (GUI.Button(btnRect, "Clear", EditorStyles.miniButton))
			{
				scenes.Clear();
				GUI.FocusControl("ClearButton");
			}
		}

		private void DrawListElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			if (index < scenes.Count)
			{
				scenes[index] = (SceneAsset)EditorGUI.ObjectField(rect, scenes[index], typeof(SceneAsset), false);
			}
		}
	}
}
