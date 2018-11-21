using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSystemBehavior : MonoBehaviour {

	public Vector2 LastCheckpoint = Vector2.zero;
	private PlayerController playerReference;
	private LevelManagement.LevelManager levelManager;

	void Start () {
		playerReference = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
		levelManager = LevelManagement.LevelManager.instance;
		if(playerReference == null || levelManager == null) {
			print("Player ou LevelManager não encontrado, o Sistema de checkpoint não funcionará");
		}
		LastCheckpoint = playerReference.transform.position;
	}

	public void ResetPlayer(){
		print("oe");
		StartCoroutine(ResetCoroutine());
	}

	private IEnumerator ResetCoroutine(){
		levelManager.FadeIn(.1f);
		playerReference.gameObject.SetActive(false);
		yield return new WaitForSeconds(.1f);
		playerReference.StopMovement();
		playerReference.transform.position = LastCheckpoint;
		yield return new WaitForSeconds(.5f);
		playerReference.gameObject.SetActive(true);
		levelManager.FadeOut(.25f);
	}
}
