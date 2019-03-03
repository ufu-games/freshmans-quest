using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour, IShowDialogue, IInteractableLeaveTrigger {

	public Dialogue[] dialogue;
	public bool destroySelfAfterTriggering = false;

	void Start() {
		if(DialogueManager.instance == null) {
			Debug.LogError("Não ha DialogueManager na cena mas tem um DialogueTrigger, colocar o DialogueManager!");
		}
	}

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
