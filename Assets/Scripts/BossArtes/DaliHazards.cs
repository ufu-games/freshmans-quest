using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaliHazards : MonoBehaviour, IDangerous {
	void IDangerous.InteractWithPlayer(Collider2D player) {
		DaliLevelManager.instance.ResetPlayer();
	}
}
