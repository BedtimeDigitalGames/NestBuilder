using System;
using System.Threading.Tasks;
using BedtimeCore.NestBuilder;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BedtimeCore.SteamUploader
{
	internal class SteamGuardWindow : EditorWindow
	{
		[SerializeField]
		private VisualTreeAsset _layout;

		private TaskCompletionSource<string> _submitTcs = new TaskCompletionSource<string>();
		
		public static Task<string> ShowWindow()
		{
			if (Application.isBatchMode)
			{
				Debug.Log($"[Steam Uploader] Missing Steam Guard authentication! Please authorize before building.");
				return Task.FromResult<string>(null);
			}
			
			var window = GetWindow<SteamGuardWindow>();
			window.titleContent = new GUIContent("Steam Guard");
			var size = new Vector2(300, 190);
			window.minSize = size;
			window.maxSize = size;
			window.Show();
			return window._submitTcs.Task;
		}

		private void TrySetResult(string result)
		{
			_submitTcs?.TrySetResult(result);
			_submitTcs = null;
			Close();
		}

		private void OnDestroy()
		{
			_submitTcs?.TrySetResult(null);
		}

		private void OnEnable()
		{
			_layout.CloneTree(rootVisualElement);

			var input = rootVisualElement.Q<TextField>("input-field");
			var cancel = rootVisualElement.Q<Button>("cancel-button");
			var submit = rootVisualElement.Q<Button>("submit-button");

			cancel.clicked += () => TrySetResult(string.Empty);
			submit.clicked += () => TrySetResult(input.text);
			submit.SetEnabled(false);
			
			input.RegisterValueChangedCallback(OnInputChange);
			input.value = string.Empty;
			
			void OnInputChange(ChangeEvent<string> evt)
			{
				var length = evt.newValue.Length;
				input.SetValueWithoutNotify(evt.newValue.ToUpper());
				submit.SetEnabled(length == 5);
			}
		}
	}
}