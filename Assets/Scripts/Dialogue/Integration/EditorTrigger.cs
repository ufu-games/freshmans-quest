using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorTrigger : MonoBehaviour {

	public bool dialogueOnStart = false;
	public Dialogue[] dialogues;

	public void TriggerDialog() {
		DialogueManager.instance.StartDialogue(dialogues);
	}

	void Start() {
		if(dialogueOnStart) {
			TriggerDialog();
		}
	}
}
