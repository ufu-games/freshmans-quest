using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSystemBehavior : MonoBehaviour {

	[ReadOnly]
	public Vector2 LastCheckpoint = Vector2.zero;
	[HideInInspector]
	public bool JustSpawned = false;
	private PlayerController playerReference;
	private LevelManagement.LevelManager levelManager;

	void Start () {
		playerReference = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
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
		playerReference.gameObject.SetActive(false);
		yield return new WaitForSeconds(.1f);
		playerReference.StopMovement();
		playerReference.transform.position = LastCheckpoint;
		yield return new WaitForSeconds(.5f);
		playerReference.gameObject.SetActive(true);
		levelManager.FadeOut(.25f);
		yield return new WaitForSeconds(.1f);
		JustSpawned = false;
	}
}
