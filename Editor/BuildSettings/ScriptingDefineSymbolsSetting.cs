using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;
#if BEDTIME_LOGGING
using BedtimeCore.ProjectSettings;
#endif

namespace BedtimeCore.NestBuilder
{
	[Serializable]
	public class ScriptingDefineSymbolsSetting : BuildSetting<List<string>>
	{
		[SerializeField]
		private List<string> _internalValue = new List<string>();

		private ReorderableList _reorderableList;

		public ScriptingDefineSymbolsSetting(Action<List<string>> applyAction = null) : base(applyAction)
		{
		}

		public override List<string> ValueSelf => _internalValue.ToList();
		public override List<string> DefaultValue => new List<string>();

		private void Initialize()
		{
			if (_reorderableList == null)
			{
				_reorderableList = new ReorderableList(_internalValue, typeof(string), true, false, true, true);
				_reorderableList.drawElementCallback = DrawListElement;
				_reorderableList.onAddCallback += OnAdd;
			}
		}

		private void DrawListElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			if (index < _internalValue.Count)
			{
				_internalValue[index] = EditorGUI.TextField(rect, _internalValue[index]);
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
				list.list.Add(string.Empty);
			}
		}
		
		public override void ApplyToEditor(BuildConfiguration configuration)
		{
			Platform platform = configuration.BuildSettings.Main.Platform.Value;
			var newSymbols = ValueSelf;
			
			#if BEDTIME_LOGGING
			var loggingEnabledSetting = configuration.BuildSettings.BedtimeLogging.BedLogEnabled;
			var loggingEnabled = (loggingEnabledSetting?.IsSet ?? false) ? loggingEnabledSetting.Value : LogSettings.LoggingEnabled;
			var symbols = string.Join(";", newSymbols);
			newSymbols = LogSettingsUtility.AddOrRemoveLoggingSymbol(symbols, loggingEnabled)
			                               .Split(';')
			                               .ToList();
			#endif
			
			PlayerSettings.SetScriptingDefineSymbols(platform.AsNamedBuildTarget, newSymbols.ToArray());
		}

		public override void DrawValue(BuildConfiguration topLevel, SerializedProperty property)
		{
			Initialize();
			_reorderableList.DoLayoutList();
		}

		public override bool WillTriggerRecompile
		{
			get
			{
				var platform = Owner.BuildSettings.Main.Platform.Value;
				var current = PlayerSettings.GetScriptingDefineSymbols(platform.AsNamedBuildTarget).Split(';');
				
				return ValueSelf.Except(current).Any();
			}
		}
	}
}
