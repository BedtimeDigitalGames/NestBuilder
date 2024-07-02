using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace BedtimeCore.BuildPipeline
{
	[CustomEditor(typeof(BuildQueue))]
	public class BuildQueueInspector : UnityEditor.Editor
	{
		private const string PAUSE_ICON = "d_PauseButton";
		private const string PLAY_ICON = "IN foldout act";
		private const string TIME_FORMAT = "yyyy/MM/dd - HH:mm:ss";
		private GUIContent playIcon;
		private GUIContent pauseIcon;
		private Build selected;

		[SerializeField]
		private Vector2 scroll;
		[SerializeField]
		private Vector2 scrollConfigurations;
		
		private List<Build> queue;
		private List<Build> history;
		
		[SerializeField]
		private List<IBuildQueueExtension> buildQueueExtensions = new List<IBuildQueueExtension>();
		private List<BuildConfiguration> configurations;
		
		private void OnEnable()
		{
			buildQueueExtensions.Clear();
			foreach (Type type in TypeCache.GetTypesDerivedFrom<IBuildQueueExtension>())
			{
				try
				{
					var instance = (IBuildQueueExtension) Activator.CreateInstance(type);
					buildQueueExtensions.Add(instance);
				}
				catch (Exception)
				{
					// ignored
				}
			}
			
			playIcon = EditorGUIUtility.IconContent(PLAY_ICON);
			pauseIcon = EditorGUIUtility.IconContent(PAUSE_ICON);

			if(target is BuildQueue buildQueue)
			{
				queue = buildQueue.Queue;
				history = buildQueue.History;
			}

			ReloadConfigurations();
			EditorApplication.projectChanged += ReloadConfigurations;
		}

		private void OnDisable()
		{
			EditorApplication.projectChanged -= ReloadConfigurations;
		}

		private GUIContent PlayPauseIcon => Builder.BuildQueue.IsPaused ? playIcon : pauseIcon;

		private void ReloadConfigurations()
		{
			configurations = AssetDatabase.FindAssets("t:BuildConfiguration")
				.Select(AssetDatabase.GUIDToAssetPath)
				.Select(AssetDatabase.LoadAssetAtPath<BuildConfiguration>)
				.ToList();
		}
		
		private IEnumerable<IBuildQueueExtension> GetValidExtensions(Build build)
		{
			foreach (IBuildQueueExtension buildQueueExtension in buildQueueExtensions)
			{
				if (!(buildQueueExtension?.ShouldDisplay(build) ?? false))
				{
					continue;
				}

				yield return buildQueueExtension;
			}
		}
		
		public override void OnInspectorGUI()
		{
			EditorGUILayout.BeginHorizontal();
			DrawQueue();
			DrawBuildConfigurations();
			EditorGUILayout.EndHorizontal();
		}

		private void DrawBuildConfigurations()
		{
			var selectedColor = new Color(.75f, .75f, .75f, 1f);
			
			var width = Mathf.Clamp(EditorGUIUtility.currentViewWidth / 4, 110, 400);
			var originalBackgroundColor = GUI.backgroundColor;
			EditorGUILayout.BeginVertical(GUILayout.Width(width));

			GUI.backgroundColor = GUI.backgroundColor * .85f; 
			if (GUILayout.Button("New Configuration", EditorStyles.toolbarButton, GUILayout.ExpandWidth(true)))
			{
				BuildConfiguration.CreateBuildConfiguration();
			}
			if(BuildConfiguration.ConfigurationCount == 0 && GUILayout.Button("Create Default Configurations", EditorStyles.toolbarButton, GUILayout.ExpandWidth(true)))
			{
				BuildConfiguration.CreateDefaultBuildConfigurations();
			}
			GUI.backgroundColor = originalBackgroundColor;

			scrollConfigurations = GUILayout.BeginScrollView(scrollConfigurations, GUIStyle.none, GUI.skin.GetStyle("VerticalScrollbar"));
			
			foreach (var configuration in configurations)
			{
				if (configuration == null)
				{
					EditorApplication.delayCall += ReloadConfigurations;
					break;
				}
				
				GUI.backgroundColor = Selection.activeObject == configuration ? selectedColor : originalBackgroundColor;
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button(configuration.name, ListStyle, GUILayout.MinWidth(width * .70f)))
				{
					Selection.activeObject = configuration;
				}
				GUI.backgroundColor = originalBackgroundColor;
				EditorGUI.BeginDisabledGroup(!configuration.CanBuild || Builder.BuildQueue.Contains(configuration));
				if (GUILayout.Button("Build", ListStyleCenter))
				{
					configuration.Build();
				}
				EditorGUI.EndDisabledGroup();
				EditorGUILayout.EndHorizontal();
				
			}
			
			EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
			EditorGUILayout.EndVertical();
			
			var rect = GUILayoutUtility.GetLastRect();
			GUI.Box(rect,"");
			
			GUILayout.EndScrollView();
			EditorGUILayout.EndVertical();
		}

		public void DrawQueue()
		{
			if (queue == null)
			{
				return;
			}
			EditorGUILayout.BeginVertical();
			scroll = GUILayout.BeginScrollView(scroll, GUIStyle.none, GUI.skin.GetStyle("VerticalScrollbar"));
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button(PlayPauseIcon, EditorStyles.toolbarButton, GUILayout.MaxWidth(40)))
			{
				Builder.BuildQueue.IsPaused = !Builder.BuildQueue.IsPaused;
			}
			EditorGUI.BeginDisabledGroup(Builder.BuildQueue.Count == 0);
			if (GUILayout.Button("Clear Queue", EditorStyles.toolbarButton))
			{
				EditorApplication.delayCall += Builder.BuildQueue.Clear;
				SlowRepaint();
			}
			EditorGUI.EndDisabledGroup();

			EditorGUI.BeginDisabledGroup(Builder.BuildQueue.History.Count == 0);
			if (GUILayout.Button("Clear History", EditorStyles.toolbarButton))
			{
				EditorApplication.delayCall += Builder.BuildQueue.ClearHistory;
				SlowRepaint();
			}
			
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginVertical("Box");
			if (!queue.Any())
			{
				EditorGUILayout.LabelField("The queue is empty! (´；ω；｀)");
			}

			for (int i = 0; i < queue.Count; i++)
			{
				DrawBuild(queue[i], i);
			}
			EditorGUILayout.EndVertical();

			if (history.Any())
			{
				EditorGUILayout.Space();
				EditorGUILayout.LabelField("History");

				EditorGUILayout.BeginVertical("HelpBox");

				for (int i = history.Count - 1; i >= 0; i--)
				{
					DrawBuild(history[i]);
				}

				EditorGUILayout.EndVertical();
			}

			GUILayout.EndScrollView();
			EditorGUILayout.EndVertical();
		}

		private void SlowRepaint()
		{
			Repaint();
			EditorApplication.delayCall += Repaint;
		}

		private void DrawBuild(Build build, int index = -1)
		{
			if (build == null || build.Configuration == null)
			{
				return;
			}
			float progress = Build.BuildStepToProgress(build.BuildStep);
			Color currentColor = Build.BuildStepToColor(build.BuildStep);
			Color selectedColor = GUI.backgroundColor - Color.white * 0.4f;
			Color originalContentColor = GUI.contentColor;
			Color originalBGolor = GUI.backgroundColor;

			bool isSelected = selected == build && build.Configuration == Selection.activeObject;

			if (isSelected)
			{
				GUI.backgroundColor = selectedColor;
			}

			using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
			{
				var bg = GUI.backgroundColor;
				using (new EditorGUILayout.HorizontalScope())
				{
					if (index != -1)
					{
						EditorGUILayout.LabelField(index.ToString(), EditorStyles.boldLabel, GUILayout.MaxWidth(18));
					}
					using (new EditorGUILayout.VerticalScope())
					{
						EditorGUILayout.LabelField(build.Configuration.name, EditorStyles.boldLabel);

						GUI.contentColor = currentColor;
						EditorGUILayout.LabelField(ObjectNames.NicifyVariableName(build.BuildStep.ToString()), EditorStyles.whiteLabel);

						if (index != -1)
						{
							bg = GUI.backgroundColor;
							GUI.backgroundColor = originalBGolor;
							Progressbar(progress, "{0}%", progress * 100f);
							GUI.backgroundColor = bg;
						}

						GUI.contentColor = originalContentColor;

						if (build.StartTime.HasValue)
						{
							EditorGUILayout.LabelField($"{build.StartTime.Value.ToLocalTime().ToString(TIME_FORMAT)}", GreyMiniLabel);
						}

						if (build.EndTime.HasValue)
						{
							EditorGUILayout.LabelField($"Elapsed: {build.EndTime - build.StartTime:hh\\:mm\\:ss}", GreyMiniLabel);
						}

						if (build.BuildStep == BuildStep.Failed)
						{
							EditorGUILayout.LabelField("Failed at: " + ObjectNames.NicifyVariableName(build.LastStep.ToString()), GreyMiniLabel);
						}
					}
				}
				
				GUI.backgroundColor = originalBGolor;
				foreach (IBuildQueueExtension extension in GetValidExtensions(build))
				{
					extension?.OnGUI(build);
				}
				GUI.backgroundColor = bg;
			}
			
			var rect = GUILayoutUtility.GetLastRect();
			
			using (new EditorGUILayout.VerticalScope())
			{

			}
			
			var mouseClicks = IsMouseState(rect, EventType.MouseDown);
			if (mouseClicks == 1)
			{
				Event.current.Use();
				selected = build;
				Selection.activeObject = build.Configuration;
				EditorGUIUtility.PingObject(build.Configuration);
			}
			else if (mouseClicks == 2)
			{
				Event.current.Use();
				if (selected != null)
				{
					var path = selected.PlayerOptions.locationPathName;
					if (Directory.Exists(path) || File.Exists(path))
					{
						EditorUtility.RevealInFinder(selected.PlayerOptions.locationPathName);
					}	
				}
			}

			GUI.backgroundColor = originalBGolor;
		}
		
		private int IsMouseState(Rect rect, EventType type, int button = -1)
		{
			Event e = Event.current;
			if (!rect.Contains(e.mousePosition))
			{
				return 0;
			}
			if (e.isMouse && e.type == type && (e.button == button || button == -1))
			{
				return e.clickCount;
			}
			return 0;
		}

		private const float PROGRESS_BAR_MIN_WIDTH = 32;
		private const float PROGRESS_BAR_MAX_WIDTH = float.MaxValue;
		private const float PROGRESS_BAR_MIN_HEIGHT = 16;
		private const float PROGRESS_BAR_MAX_HEIGHT = 18;
		private void Progressbar(float value, string format = "", params object[] args)
		{
			Rect rectangle = GUILayoutUtility.GetRect(PROGRESS_BAR_MIN_WIDTH, PROGRESS_BAR_MAX_WIDTH, PROGRESS_BAR_MIN_HEIGHT, PROGRESS_BAR_MAX_HEIGHT);
			EditorGUI.ProgressBar(rectangle, value, string.Format(format, args));
		}
		
		private static GUIStyle greyMiniLabel;
		private static GUIStyle listStyle;
		private static GUIStyle listStyleCenter;

		private static GUIStyle GreyMiniLabel
		{
			get
			{
				if (greyMiniLabel == null)
				{
					greyMiniLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
					greyMiniLabel.alignment = TextAnchor.MiddleLeft;
				}
				return greyMiniLabel;
			}
		}
		
		private static GUIStyle ListStyle
		{
			get
			{
				if (listStyle == null || !listStyle.name.Equals("toolbarbutton"))
				{
					listStyle = new GUIStyle(EditorStyles.toolbarButton);
					listStyle.alignment = TextAnchor.MiddleLeft;
					listStyle.fixedHeight = EditorGUIUtility.singleLineHeight;
				}
				return listStyle;
			}
		}
		
		private static GUIStyle ListStyleCenter
		{
			get
			{
				if (listStyleCenter == null || !listStyleCenter.name.Equals("toolbarbutton"))
				{
					listStyleCenter = new GUIStyle(EditorStyles.toolbarButton);
					listStyleCenter.alignment = TextAnchor.MiddleCenter;
					listStyleCenter.fixedHeight = EditorGUIUtility.singleLineHeight;
				}
				return listStyleCenter;
			}
		}
	}
}