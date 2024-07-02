using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.Linq;
using System.ComponentModel;

namespace BedtimeCore.BuildPipeline
{
	[CustomEditor(typeof(BuildConfiguration), true), CanEditMultipleObjects]
	public class BuildConfigurationInspector : UnityEditor.Editor
	{
		const string MISCELLANEOUS_CATEGORY = "Miscellaneous";
		private static readonly SettingCategory[] DEFAULT_CATEGORIES =
		{
			new SettingCategory("Main", 0, true),
			new SettingCategory(MISCELLANEOUS_CATEGORY, 10000),
		};

		private BuildConfiguration configuration;
		private bool buildWasClicked;
		private List<BuildIllegalReason> buildIllegalReasons = new List<BuildIllegalReason>();

		[SerializeField]
		private static List<SettingCategory> categorizedSettings;
		private static bool showHiddenSettings = true;

		private void ClearCategoryContent()
		{
			foreach (var category in categorizedSettings)
			{
				category.settings.Clear();
			}
		}

		private SettingCategory GetOrCreateCategory(string category, int priority = 10)
		{
			var result = categorizedSettings.Where(c => c.name == category).FirstOrDefault();

			if (result == null)
			{
				result = new SettingCategory(category, priority);
				categorizedSettings.Add(result);
				SortCategories();
			}
			return result;
		}

		private void SortCategories()
		{
			categorizedSettings = categorizedSettings.OrderBy(c => c.priority).ToList();
		}

		private void OnEnable()
		{
			if (categorizedSettings == null)
			{
				categorizedSettings = new List<SettingCategory>();
				categorizedSettings.AddRange(DEFAULT_CATEGORIES);
				SortCategories();
			}
			
			ClearCategoryContent();

			configuration = target as BuildConfiguration;
			CheckForRequiredSettings();
			EditorApplication.update += Update;
			Undo.undoRedoPerformed += OnDirtied;
		}

		private void OnDisable()
		{
			EditorApplication.update -= Update;
		}

		private void Update()
		{
			if (Builder.BuildQueue.GetBuild(configuration) != null)
			{
				Repaint();
			}
		}

		private void CreateCategories()
		{
			foreach (var setting in configuration.Settings)
			{
				if (setting == null)
				{
					continue;
				}
				SettingCategory category = null;
				var attribute = setting.GetAttribute<CategoryAttribute>();
				if (attribute != null)
				{
					category = GetOrCreateCategory(attribute.Category);
				}
				else
				{
					category = GetOrCreateCategory(MISCELLANEOUS_CATEGORY);
				}

				category.Add(setting);
			}
		}

		private void OnDirtied()
		{
			if (configuration != null)
			{
				EditorUtility.SetDirty(configuration);
				EditorApplication.delayCall += CheckForRequiredSettings;
			}
		}

		public override void OnInspectorGUI()
		{
			if (target == null || configuration.Settings == null)
			{
				return;
			}

			serializedObject.UpdateIfRequiredOrScript();

			CreateCategories();

			EditorGUI.BeginChangeCheck();

			Undo.RecordObject(target, "Modified Build Configuration");

			DrawProgressBar();
			DrawInspectorHeader();
			DrawSettings();
			DrawMessages();

			if (EditorGUI.EndChangeCheck())
			{
				OnDirtied();
			}

		}

		private void DrawMessages()
		{
			if (buildIllegalReasons.Count > 0)
			{
				EditorGUILayout.BeginVertical();
				foreach (var buildIllegalReason in buildIllegalReasons)
				{
					EditorGUILayout.HelpBox(buildIllegalReason.reason, buildIllegalReason.messageType);
				}
				EditorGUILayout.EndVertical();
			}
		}

		private void DrawSettings()
		{
			EditorGUI.BeginDisabledGroup(targets.Length > 1 || Builder.BuildQueue.Count > 1 || EditorApplication.isCompiling || buildWasClicked);
			foreach (var item in categorizedSettings)
			{
				bool hasValues = false;
				foreach (var setting in item.settings)
				{
					if (showHiddenSettings || configuration.IsSettingValidForConfiguration(setting))
					{
						hasValues = true;
						break;
					}
				}

				if (!hasValues)
				{
					continue;
				}


				item.isExpanded = EditorGUILayout.Foldout(item.isExpanded, item.name, true);

				if (item.isExpanded)
				{
					EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins);
					foreach (var setting in item.settings)
					{
						if (!showHiddenSettings && !configuration.IsSettingValidForConfiguration(setting))
						{
							continue;
						}
						setting.OnGUI();
					}

					EditorGUILayout.EndVertical();
				}
			}
			EditorGUI.EndDisabledGroup();
		}
		
		private void DrawProgressBar()
		{
			Build configuredBuild = Builder.BuildQueue.GetBuild(configuration);

			if (configuredBuild != null)
			{
				float progress = Build.BuildStepToProgress(configuredBuild.BuildStep);
				Progressbar(progress, "Build Step: {0}", ObjectNames.NicifyVariableName(configuredBuild.BuildStep.ToString()));
			}
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
		
		private void DrawInspectorHeader()
		{
			var originalLabelWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 60;
			EditorGUILayout.BeginHorizontal();

			var lastConfiguration = configuration.fallbackConfiguration;
			configuration.fallbackConfiguration = (BuildConfiguration)EditorGUILayout.ObjectField("Parent",
				configuration.fallbackConfiguration, configuration.GetType(), false);
			if (lastConfiguration != configuration.fallbackConfiguration)
			{
				foreach (var target in targets)
				{
					var buildConf = target as BuildConfiguration;
					if (buildConf != null)
					{
						buildConf.fallbackConfiguration = configuration.fallbackConfiguration;
						EditorUtility.SetDirty(buildConf);
					}
				}
			}

			EditorGUIUtility.labelWidth = originalLabelWidth;


			
			if (GUILayout.Button("Apply to editor",GUILayout.Width(100)))
			{
				configuration.ApplyToEditor(true);
			}
			
			var originalBackgroundColor = GUI.backgroundColor;
			GUI.backgroundColor = configuration.CanBuild ? Color.green : Color.red;
			EditorGUI.BeginDisabledGroup(!configuration.CanBuild || Builder.BuildQueue.Contains(configuration));
			if (GUILayout.Button("Build & Run",GUILayout.Width(100)))
			{
				BuildPlayer(BuildOptions.AutoRunPlayer);
			}
			if (GUILayout.Button("Build",GUILayout.Width(70)))
			{
				BuildPlayer();
			}

			EditorGUI.EndDisabledGroup();

			GUI.backgroundColor = originalBackgroundColor;

			EditorGUILayout.EndHorizontal();
			if (configuration.fallbackConfiguration == configuration)
			{
				configuration.fallbackConfiguration = null;
			}
		}

		private void BuildPlayer(BuildOptions modifiers = BuildOptions.None)
		{
			buildWasClicked = true;

			foreach (var target in targets.OrderBy(b => b.name))
			{
				var buildConf = target as BuildConfiguration;
				if (buildConf == null || !buildConf.CanBuild)
				{
					continue;
				}

				EditorApplication.delayCall += () =>
				{
					buildConf.Build(modifiers);
					buildWasClicked = false;
				};
			}
		}

		private void CheckForRequiredSettings()
		{
			buildIllegalReasons.Clear();
			foreach (var setting in configuration.GetIllegalSettings())
			{
				string reason = string.Format("[{0}] must be set", ObjectNames.NicifyVariableName(setting.Name));
				buildIllegalReasons.Add(new BuildIllegalReason(reason, MessageType.Error));
			}
		}

		private struct BuildIllegalReason
		{
			public MessageType messageType;
			public string reason;

			public BuildIllegalReason(string reason, MessageType messageType)
			{
				this.reason = reason;
				this.messageType = messageType;
			}

			public override string ToString()
			{
				return reason;
			}
		}

		[Serializable]
		private class SettingCategory : IEquatable<SettingCategory>, IComparable<SettingCategory>
		{
			public string name;
			public bool isExpanded = false;
			public int priority = 10;

			public HashSet<IBuildSetting> settings = new HashSet<IBuildSetting>();

			public bool Contains(IBuildSetting setting)
			{
				return settings.Contains(setting);
			}

			public bool Add(IBuildSetting setting)
			{
				return settings.Add(setting);
			}

			public int Count
			{
				get
				{
					return settings.Count;
				}
			}

			public SettingCategory(string name, int priority = 0, bool isExpanded = false)
			{
				this.name = name;
				this.priority = priority;
				this.isExpanded = isExpanded;
			}

			public bool Equals(SettingCategory other)
			{
				return other != null && this.name == other.name;
			}

			public int CompareTo(SettingCategory other)
			{
				if (other == null)
				{
					return 1;
				}

				return Mathf.Clamp(other.priority - this.priority, -1, 1);
			}
		}
	}
}