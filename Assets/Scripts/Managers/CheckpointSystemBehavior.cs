using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSystemBehavior : MonoBehaviour {

	public AudioClip HurtClip;
	[ReadOnly]
	public Vector2 LastCheckpoint = Vector2.zero;
	[HideInInspector]
	public bool JustSpawned = false;
	private GameObject playerReference;
	private LevelManagement.LevelManager levelManager;
	private List<GameObject> all_gameObjects = new List<GameObject>();
	private LevelTransition lv;

	void Start () {
		playerReference = GameObject.FindGameObjectWithTag("Player");
		levelManager = LevelManagement.LevelManager.instance;
		if(playerReference == null || levelManager == null) {
			print("Player ou LevelManager não encontrado, o Sistema de checkpoint não funcionará");
		}
		foreach (GameObject go in GameObject.FindGameObjectsWithTag("Prop")) {
			all_gameObjects.Add(go);
		}
		foreach (GameObject go in GameObject.FindGameObjectsWithTag("BreakableWall")) {
			all_gameObjects.Add(go);
		}
		foreach (GameObject go in GameObject.FindGameObjectsWithTag("Enemy")) {
			all_gameObjects.Add(go);
		}
		foreach(GameObject go in GameObject.FindGameObjectsWithTag("Resetable")) {
			all_gameObjects.Add(go);
		}
		
		lv = GameObject.FindGameObjectWithTag("Transitioner").GetComponent<LevelTransition>();
		if(lv == null) {
			print("LevelTransition não encontrado, o sistema de reset de props não funcionará");
			all_gameObjects.Clear();
		}
	}

	public void ResetPlayer(){
		StartCoroutine(ResetCoroutine());
	}

	private IEnumerator ResetCoroutine(){
		// SoundManager.instance.PlaySfxWithTimeOffset(HurtClip,0.9f);
		JustSpawned = true;
		levelManager.FadeIn(.1f);
		playerReference.GetComponent<PlayerController>().StopMovement();
		playerReference.GetComponent<PlayerController>().enabled = false;
		foreach(SpriteRenderer spr in playerReference.GetComponentsInChildren<SpriteRenderer>()) {
			spr.enabled = false;
		}
		yield return new WaitForSeconds(.1f);
		playerReference.transform.position = LastCheckpoint;
		ScreenTransition sc = lv.m_nowCollider.GetComponent<ScreenTransition>();
		foreach(GameObject go in all_gameObjects) {
			if(sc.m_resettables.Contains(go)) {
				IResettableProp ir = go.GetComponent<IResettableProp>();
				if(ir != null && go.activeInHierarchy) {
					Debug.Log("1");
					ir.Reset();
				}
			}
		}
		yield return new WaitForSeconds(.5f);
		playerReference.GetComponent<PlayerController>().enabled = true;
		foreach(SpriteRenderer spr in playerReference.GetComponentsInChildren<SpriteRenderer>()) {
			spr.enabled = true;
		}
		levelManager.FadeOut(.25f);
		yield return new WaitForSeconds(.1f);
		JustSpawned = false;
		SaveSystem.instance.Died();
	}

	public void RemovePizzaCounters() {
		foreach(GameObject go in all_gameObjects) {
			CollectableBehavior cb = go.GetComponent<CollectableBehavior>();
			if(cb != null && go.activeInHierarchy) {
				cb.Reset();
			}
		}
	}
}
