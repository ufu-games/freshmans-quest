using System;
using UnityEngine;
using GenericMenu = NodeEditorFramework.Utilities.GenericMenu;

namespace NodeEditorFramework.Standard
{
	public class NodeEditorInterface
	{
		public NodeEditorUserCache canvasCache;
		public Action<GUIContent> ShowNotificationAction;

		// GUI
		public string sceneCanvasName = "";
		public float toolbarHeight = 17;

		// Modal Panel
		public bool showModalPanel;
		public Rect modalPanelRect = new Rect(20, 50, 250, 70);
		public Action modalPanelContent;

		public void ShowNotification(GUIContent message)
		{
			if (ShowNotificationAction != null)
				ShowNotificationAction(message);
		}

#region GUI

		public void DrawToolbarGUI(Rect rect)
		{
			rect.height = toolbarHeight;
			GUILayout.BeginArea (rect, NodeEditorGUI.toolbar);
			GUILayout.BeginHorizontal();
			float curToolbarHeight = 0;

			if (GUILayout.Button("File", NodeEditorGUI.toolbarDropdown, GUILayout.Width(50)))
			{
				GenericMenu menu = new GenericMenu(!Application.isPlaying);

				// Load / Save
#if UNITY_EDITOR
				menu.AddItem(new GUIContent("Load Canvas"), false, LoadCanvas);
				menu.AddItem(new GUIContent("Reload Canvas"), false, ReloadCanvas);
				menu.AddSeparator("");
				if (canvasCache.nodeCanvas.allowSceneSaveOnly)
				{
					menu.AddDisabledItem(new GUIContent("Save Canvas"));
					menu.AddDisabledItem(new GUIContent("Save Canvas As"));
				}
				else
				{
					menu.AddItem(new GUIContent("Save Canvas"), false, SaveCanvas);
					menu.AddItem(new GUIContent("Save Canvas As"), false, SaveCanvasAs);
				}
				menu.AddSeparator("");
#endif

				// Show dropdown
				menu.Show(new Vector2(5, toolbarHeight));
			}

			curToolbarHeight = Mathf.Max(curToolbarHeight, GUILayoutUtility.GetLastRect().yMax);

			GUILayout.Space(10);
			GUILayout.FlexibleSpace();

			GUILayout.Label(new GUIContent("" + canvasCache.nodeCanvas.saveName + " (" + (canvasCache.nodeCanvas.livesInScene ? "Scene Save" : "Asset Save") + ")", 
											"Opened Canvas path: " + canvasCache.nodeCanvas.savePath), NodeEditorGUI.toolbarLabel);
			GUILayout.Label("Type: " + canvasCache.typeData.DisplayString, NodeEditorGUI.toolbarLabel);
			curToolbarHeight = Mathf.Max(curToolbarHeight, GUILayoutUtility.GetLastRect().yMax);

			GUI.backgroundColor = new Color(1, 0.3f, 0.3f, 1);
			curToolbarHeight = Mathf.Max(curToolbarHeight, GUILayoutUtility.GetLastRect().yMax);
			GUI.backgroundColor = Color.white;

			GUILayout.EndHorizontal();
			GUILayout.EndArea();

			if (Event.current.type == EventType.Repaint)
				toolbarHeight = curToolbarHeight;
		}

		public void DrawModalPanel()
		{
			if (showModalPanel)
			{
				if (modalPanelContent == null)
					return;
				GUILayout.BeginArea(modalPanelRect, NodeEditorGUI.nodeBox);
				modalPanelContent.Invoke();
				GUILayout.EndArea();
			}
		}

#endregion

#region Menu Callbacks

#if UNITY_EDITOR
		private void LoadCanvas()
		{
			string path = UnityEditor.EditorUtility.OpenFilePanel("Load Node Canvas", NodeEditor.editorPath + "Resources/Saves/", "asset");
			if (!path.Contains(Application.dataPath))
			{
				if (!string.IsNullOrEmpty(path))
					ShowNotification(new GUIContent("You should select an asset inside your project folder!"));
			}
			else
				canvasCache.LoadNodeCanvas(path);
		}

		private void ReloadCanvas()
		{
			string path = canvasCache.nodeCanvas.savePath;
			if (!string.IsNullOrEmpty(path))
			{
				if (path.StartsWith("SCENE/"))
					canvasCache.LoadSceneNodeCanvas(path.Substring(6));
				else
					canvasCache.LoadNodeCanvas(path);
				ShowNotification(new GUIContent("Canvas Reloaded!"));
			}
			else
				ShowNotification(new GUIContent("Cannot reload canvas as it has not been saved yet!"));
		}

		private void SaveCanvas()
		{
			string path = canvasCache.nodeCanvas.savePath;

			if (!string.IsNullOrEmpty(path)) {
				canvasCache.SaveNodeCanvas(path);
				ShowNotification(new GUIContent("Canvas Saved!"));
			}
			else {
				ShowNotification(new GUIContent("No save location found. Use 'Save As'!"));
			}
		}

		private void SaveCanvasAs()
		{
			string panelPath = NodeEditor.editorPath + "Resources/Saves/";
			if (canvasCache.nodeCanvas != null && !string.IsNullOrEmpty(canvasCache.nodeCanvas.savePath))
				panelPath = canvasCache.nodeCanvas.savePath;

			string path = UnityEditor.EditorUtility.SaveFilePanelInProject("Save Node Canvas", "Node Canvas", "asset", "", panelPath);
			if (!string.IsNullOrEmpty(path))
				canvasCache.SaveNodeCanvas(path);
		}
#endif
#endregion
	}
}
