using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour, IShowDialogue, IInteractableLeaveTrigger {

	public Dialogue[] dialogue;
	public bool destroySelfAfterTriggering = false;

	public void TriggerDialogue() {
		DialogueManager.instance.StartDialogue(dialogue);
	}

	void IShowDialogue.ShowDialogue() {
		TriggerDialogue();
		FindObjectOfType<PlayerController>().StartDialogue();

		if(destroySelfAfterTriggering) {
			Destroy(gameObject);
		}
	}

	void IInteractableLeaveTrigger.Interact() {
		FindObjectOfType<PlayerController>().EndDialogue();
		DialogueManager.instance.EndDialog();
	}
}
