using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorTrigger : MonoBehaviour {

	public int dialogIdToLoad;
	public bool dialogueOnStart = false;

	public void TriggerDialog() {
		// DialogueManager.instance.StartDialogue(dialogIdToLoad);
	}

	void Start() {
		if(dialogueOnStart) {
			TriggerDialog();
		}
	}
}
