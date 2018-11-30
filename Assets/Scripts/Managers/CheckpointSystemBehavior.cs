using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSystemBehavior : MonoBehaviour {

	[ReadOnly]
	public Vector2 LastCheckpoint = Vector2.zero;
	[HideInInspector]
	public bool JustSpawned = false;
	private GameObject playerReference;
	private LevelManagement.LevelManager levelManager;

	void Start () {
		playerReference = GameObject.FindGameObjectWithTag("Player");
		levelManager = LevelManagement.LevelManager.instance;
		if(playerReference == null || levelManager == null) {
			print("Player ou LevelManager não encontrado, o Sistema de checkpoint não funcionará");
		}
	}

	public void ResetPlayer(){
		StartCoroutine(ResetCoroutine());
	}

	private IEnumerator ResetCoroutine(){
		JustSpawned = true;
		levelManager.FadeIn(.1f);
		playerReference.GetComponent<PlayerController>().StopMovement();
		playerReference.GetComponent<PlayerController>().enabled = false;
		foreach(SpriteRenderer spr in playerReference.GetComponentsInChildren<SpriteRenderer>()) {
			spr.enabled = false;
		}
		yield return new WaitForSeconds(.1f);
		playerReference.transform.position = LastCheckpoint;
		yield return new WaitForSeconds(.5f);
		playerReference.GetComponent<PlayerController>().enabled = true;
		foreach(SpriteRenderer spr in playerReference.GetComponentsInChildren<SpriteRenderer>()) {
			spr.enabled = true;
		}
		levelManager.FadeOut(.25f);
		yield return new WaitForSeconds(.1f);
		JustSpawned = false;
	}
}
