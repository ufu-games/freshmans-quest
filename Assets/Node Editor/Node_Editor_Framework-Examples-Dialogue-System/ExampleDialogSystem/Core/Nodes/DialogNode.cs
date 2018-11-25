using System;
using NodeEditorFramework;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// This node has one entry and one exit, it is just to display something, then move on
/// </summary>
[Node(false, "Dialog/Dialog Node", new Type[] { typeof(DialogNodeCanvas) })]
public class DialogNode : BaseDialogNode
{
	public override string Title {get { return "Dialog Node"; } }
	public override Vector2 MinSize { get { return new Vector2(350, 60); } }
	public override bool AutoLayout { get { return true; } }

	private const string Id = "dialogNode";
	public override string GetID { get { return Id; } }
	public override Type GetObjectType { get { return typeof(DialogNode); } }

	//Previous Node Connections
	[ValueConnectionKnob("From Previous", Direction.In, "DialogForward", NodeSide.Left, 30)]
	public ValueConnectionKnob fromPreviousIN;

	//Next Node to go to
	[ValueConnectionKnob("To Next", Direction.Out, "DialogForward", NodeSide.Right, 30)]
	public ValueConnectionKnob toNextOUT;

	private Vector2 scroll;

#if UNITY_EDITOR
	protected override void OnCreate ()
	{
		CharacterName = "Character Name";
		DialogLine = "Insert dialog text here";
		CharacterPotrait = null;
	}

	public override void NodeGUI()
	{
		EditorGUILayout.BeginVertical("Box");
		CharacterPotrait = (Sprite)EditorGUILayout.ObjectField(CharacterPotrait, typeof(Sprite), false, GUILayout.Width(330f), GUILayout.Height(310f));
		GUILayout.Space(10);
		CharacterName = EditorGUILayout.TextField("", CharacterName);
		GUILayout.EndVertical();

		GUILayout.Space(10);

		GUILayout.BeginHorizontal();

		scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(100));
		DialogLine = EditorGUILayout.TextArea(DialogLine, GUILayout.ExpandHeight(true));
		EditorGUILayout.EndScrollView();
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		EditorGUIUtility.labelWidth = 90;
		SoundDialog = EditorGUILayout.ObjectField("Dialog Audio:", SoundDialog, typeof(AudioClip), false) as AudioClip;
		if (GUILayout.Button("►", GUILayout.Width(20)))
		{
			if (SoundDialog)
				AudioUtils.PlayClip(SoundDialog);
		}
		GUILayout.EndHorizontal();
	}
#endif

	public override BaseDialogNode Input(int inputValue)
	{
		switch (inputValue)
		{
		case (int)EDialogInputValue.Next:
			if (IsNextAvailable ())
				return getTargetNode (toNextOUT);
			break;
		}
		return null;
	}

	public override bool IsNextAvailable()
	{
		return IsAvailable (toNextOUT);
	}
}
