using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehindEffect : MonoBehaviour, IDangerous {

	void IDangerous.InteractWithPlayer(Collider2D player) {
		player.transform.GetComponent<PlayerController>().ActivateSillouette();
	}
}
