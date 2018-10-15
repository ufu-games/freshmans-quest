using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaliCheckpoints : MonoBehaviour, IInteractable {

	void IInteractable.Interact() {
		DaliLevelManager.instance.PassedCheckpoint(transform.position);
	}
}
