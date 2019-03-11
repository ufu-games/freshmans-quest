using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueArray {
	public string descriptionName;
	public Dialogue[] dialogues;
}

public class ArrayDialogueTrigger : MonoBehaviour, IShowDialogue, IInteractableLeaveTrigger {

	public DialogueArray[] dialogues;

	public bool destroySelfAfterTriggering = false;

	private int m_currentDialogueIndex;

	void Start() {
		if(DialogueManager.instance == null) {
			Debug.LogError("Não ha DialogueManager na cena mas tem um ArrayDialogueTrigger, colocar o DialogueManager!");
		}

		m_currentDialogueIndex = 0;
	}

	public void TriggerDialogue() {
		DialogueManager.instance.StartDialogue(dialogues[m_currentDialogueIndex].dialogues );

		m_currentDialogueIndex = Mathf.RoundToInt(Mathf.Clamp(m_currentDialogueIndex + 1, 0, (dialogues.Length - 1)));
	}

	void IShowDialogue.ShowDialogue() {
		TriggerDialogue();

		if(destroySelfAfterTriggering) {
			Destroy(gameObject);
		}
	}

	void IInteractableLeaveTrigger.Interact() {
		FindObjectOfType<PlayerController>().EndDialogue();
		DialogueManager.instance.EndDialog();
	}
}
