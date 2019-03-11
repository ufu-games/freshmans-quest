using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehindEffect : MonoBehaviour, INonHarmfulInteraction {

	// [NOT USED]
	void INonHarmfulInteraction.InteractWithPlayer(Collider2D player) {
		// player.transform.GetComponent<PlayerController>().ToggleSillouette();
	}
}
