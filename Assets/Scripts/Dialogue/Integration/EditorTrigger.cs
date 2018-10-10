using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorTrigger : MonoBehaviour {

	public int dialogIdToLoad;

	public void TriggerDialog() {
		DialogueManager.instance.StartDialogue(dialogIdToLoad);
	}
}
