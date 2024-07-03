using UnityEditor;

namespace BedtimeCore.NestBuilder
{
	public class BuildQueueWindow : EditorWindow
	{
		private const string WINDOW_TITLE = "Build Queue";
		private const string NORMAL_ICON = "d_ToolHandleLocal";
		private const string WORKING_ICON = "d_WaitSpin{0}";
		private const double ANIMATION_TIME_WAIT = .02f;
		private double lastTime;

		private int animationFrame;

		private bool wasBuilding;
		private BuildQueueInspector inspector;

		[MenuItem("BedtimeCore/Build/Build Queue")]
		public static void GetWindow()
		{
			BuildQueueWindow window = CreateInstance<BuildQueueWindow>();
			window.Show();
		}

		private void OnEnable()
		{
			inspector = UnityEditor.Editor.CreateEditor(Builder.BuildQueue, typeof(BuildQueueInspector)) as BuildQueueInspector;
			SetTitle(NORMAL_ICON);
			EditorApplication.update += Update;
			Selection.selectionChanged += RepaintAll;
		}
		
		private void Update()
		{
			if (Builder.BuildQueue.Count > 0 && !Builder.BuildQueue.IsPaused)
			{
				HandleBuilding();
				RepaintAll();
				wasBuilding = true;
			}
			else if (wasBuilding)
			{
				RepaintAll();
				wasBuilding = false;
				SetTitle(NORMAL_ICON);
			}
		}

		private void RepaintAll()
		{
			inspector.Repaint();
			Repaint();
		}

		private void HandleBuilding()
		{
			var currentTime = EditorApplication.timeSinceStartup;
			if (currentTime - lastTime < ANIMATION_TIME_WAIT)
			{
				return;
			}

			const int MAX_FRAMES = 11;
			animationFrame = ++animationFrame > MAX_FRAMES ? 0 : animationFrame;
			SetTitle(string.Format(WORKING_ICON, animationFrame.ToString("00")));

			lastTime = currentTime;
		}

		private void OnDisable()
		{
			EditorApplication.update -= Update;
			Selection.selectionChanged -= Repaint;
		}

		private void SetTitle(string icon) => titleContent = EditorGUIUtility.TrTextContentWithIcon(WINDOW_TITLE, icon);

		private void OnGUI()
		{
			inspector.OnInspectorGUI();
		}
	}
}