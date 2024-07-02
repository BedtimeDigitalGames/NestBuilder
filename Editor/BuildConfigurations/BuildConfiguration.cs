using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;
using System.Collections.ObjectModel;

namespace BedtimeCore.BuildPipeline
{
	[Serializable]
	public sealed class BuildConfiguration : ScriptableObject, IEquatable<BuildConfiguration>
	{
		public BuildConfiguration fallbackConfiguration;

		public ReadOnlyCollection<IBuildSetting> Settings
		{
			get
			{
				if (settings == null)
				{
					Initialize();
				}
				return settings;
			}
			private set => settings = value;
		}

		private void Initialize()
		{
			var tempList = new List<IBuildSetting>();
			const BindingFlags FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;
			Type configurationType = BuildSettings.GetType();
			foreach (FieldInfo fInfo in configurationType.GetFields(FLAGS))
			{
				if (typeof(IBuildSetting).IsAssignableFrom(fInfo.FieldType))
				{
					var setting = configurationType.GetField(fInfo.Name, FLAGS)?.GetValue(BuildSettings) as IBuildSetting;
					if(setting == null)
					{
						continue;
					}
					setting.Initialize(fInfo.Name, this, fInfo.GetCustomAttributes(typeof(Attribute), true) as Attribute[]);
					tempList.Add(setting);
				}
			}

			settings = tempList.AsReadOnly();
		}

		[SerializeField]
		private BuildSettingContainer buildSettings = new BuildSettingContainer();

		private ReadOnlyCollection<IBuildSetting> settings;

		public BuildSettingContainer BuildSettings => buildSettings;

		public bool CanBuild => !GetIllegalSettings().Any();

		public bool Build(BuildOptions modifiers = BuildOptions.None) => CanBuild && Builder.EnqueueBuild(this, modifiers);

		public T GetCascadedSetting<T>(string settingName) where T : class, IBuildSetting
		{
			var owner = this;
			BuildConfiguration lastValid = this;
			do
			{
				var tempSetting = owner.GetInternalSetting<T>(settingName);
				if (tempSetting is { IsSetSelf: true })
				{
					return tempSetting;
				}
				owner = owner.fallbackConfiguration;
				if (owner != null)
				{
					lastValid = owner;
				}
			}
			while (owner != null);

			// Value was never set through the hierarchy if we get down here
			return lastValid.GetInternalSetting<T>(settingName);
		}

		public T GetInternalSetting<T>(string settingName) where T : class, IBuildSetting => Settings?.FirstOrDefault(s => s.Name == settingName) as T;

		private void OnEnable()
		{
			Initialize();
		}

		public bool ApplyToEditor(bool forceApply = false)
		{
			if (!forceApply && !CanBuild)
			{
				return false;
			}

			foreach (var setting in OrderByPriority(Settings))
			{
				var cascadedSetting = GetCascadedSetting<IBuildSetting>(setting.Name);
				if (cascadedSetting != null && IsSettingValidForConfiguration(cascadedSetting) && cascadedSetting.IsSetSelf)
				{
					cascadedSetting.ApplyToEditor(this);
				}
			}
			return true;
		}

		public static void CreateAsset(BuildConfiguration configuration)
		{
			const string ROOT_FOLDER = "Assets/";
			const string NEW_NAME = "New Configuration";
			const string FILE_NAME = ".asset";
			const int MAX_DUPLICATES = 100;

			string directory = Path.Combine(ROOT_FOLDER, Builder.BuildPresetDefaultPath);
			var finalPath = Path.Combine(directory, NEW_NAME + FILE_NAME);
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}
			if (File.Exists(finalPath))
			{
				for (int i = 1; i < MAX_DUPLICATES; i++)
				{
					finalPath = Path.Combine(directory, $"{NEW_NAME} {i}{FILE_NAME}");
					if (!File.Exists(finalPath))
					{
						break;
					}
				}
			}
			AssetDatabase.CreateAsset(configuration, finalPath);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			Selection.activeObject = configuration;
		}

		private static List<IBuildSetting> OrderByPriority(IEnumerable<IBuildSetting> settings)
		{
			Func<IBuildSetting, bool> match = s => s.GetAttribute<PriorityAttribute>() != null;
			var hasAttribute = settings.Where(s => match(s)).ToList();
			var rest = settings.Where(s => !match(s));
			var sorted = hasAttribute.OrderBy(m => m.GetAttribute<PriorityAttribute>().Priority).ToList();

			sorted.AddRange(rest);
			return sorted;
		}

		public bool IsSettingValidForConfiguration(IBuildSetting setting)
		{
			if (setting.HasAttribute<RequiredAttribute>())
			{
				return true;
			}

			var platform = BuildSettings.platform.GetCascaded();
			if (platform == null)
			{
				return false;
			}

			var platformRestriction = setting.GetAttribute<PlatformRestrictionAttribute>();
			if (platformRestriction != null)
			{
				return platformRestriction.IsAllowed(platform.Value);
			}



			var backendRestriction = setting.GetAttribute<ScriptingBackendRestrictionAttribute>();
			if (backendRestriction != null)
			{
				var backend = BuildSettings.scriptingBackend.Value;
				return backendRestriction.IsAllowed(backend);
			}

			return true;
		}

		public IEnumerable<IBuildSetting> GetIllegalSettings()
		{
			foreach (var setting in Settings)
			{
				if (setting.GetAttribute<RequiredAttribute>() != null)
				{
					var cascadedSetting = GetCascadedSetting<IBuildSetting>(setting.Name);
					if (cascadedSetting == null || !cascadedSetting.IsSetSelf)
					{
						yield return setting;
						continue;
					}
				}
			}
		}

		public bool Equals(BuildConfiguration other)
		{
			return this.name == other.name;
		}

		public static void CreateBuildConfiguration()
		{
			CreateAsset(CreateInstance<BuildConfiguration>());
		}

		public static void CreateDefaultBuildConfigurations()
		{
			var dir = new DirectoryInfo("Packages/com.bedtime.build-pipeline/.DefaultBuildConfigurations");
			if(!dir.Exists)
			{
				Debug.LogError("Default build configurations not found");
				return;
			}
			
			Directory.CreateDirectory(Path.Combine("Assets", Builder.BuildPresetDefaultPath));
			var files = dir.GetFiles("*.*");
			foreach (var file in files)
			{
				var path = Path.Combine("Assets", Builder.BuildPresetDefaultPath, file.Name);
				if (File.Exists(path))
				{
					continue;
				}
				File.Copy(file.FullName, path);
			}
			
			AssetDatabase.Refresh();
		}
		
		public static int ConfigurationCount => AssetDatabase.FindAssets($"t:{nameof(BuildConfiguration)}").Length;
	}
}