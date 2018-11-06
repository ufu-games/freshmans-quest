using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaliEndLevel : MonoBehaviour, IInteractable, IShowDialogue {

	void IInteractable.Interact() {
		DaliLevelManager.instance.PassedCheckpoint(transform.position);
		DaliLevelManager.instance.EndOfLevel();
	}

	void IShowDialogue.ShowDialogue() {
		DaliLevelManager.instance.ShowFinalDialogue();
	}
}
