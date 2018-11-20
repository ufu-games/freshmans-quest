using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardBehavior : MonoBehaviour, IDangerous {
	void IDangerous.InteractWithPlayer(Collider2D player) {
		print(GameObject.FindGameObjectWithTag("Checkpoint System").name);
		GameObject.FindGameObjectWithTag("Checkpoint System").GetComponent<CheckpointSystemBehavior>().ResetPlayer();
	}
}
