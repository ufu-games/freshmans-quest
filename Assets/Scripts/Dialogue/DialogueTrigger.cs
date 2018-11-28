using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour, IInteractable, IInteractableLeaveTrigger {

	public Dialogue[] dialogue;

	public void TriggerDialogue() {
		DialogueManager.instance.StartDialogue(dialogue);
	}

	void IInteractable.Interact() {
		TriggerDialogue();
	}

	void IInteractableLeaveTrigger.Interact() {
		DialogueManager.instance.EndDialog();
	}
}
