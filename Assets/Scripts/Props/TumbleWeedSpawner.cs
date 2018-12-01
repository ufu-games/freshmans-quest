using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TumbleWeedSpawner : MonoBehaviour, IResettableProp, IInteractable {

	private GameObject tWeed;
	private bool Spawned = false;

	public void Reset() {
		if(tWeed != null) {
			Destroy(tWeed);
		}
		Spawned = false;
	}

	public void Interact() {
		if(!Spawned) {
			Spawned = true;
			tWeed = (GameObject) GameObject.Instantiate(Resources.Load("Tumble Weed"));
			tWeed.transform.position = this.transform.position;
		}
	}
}
