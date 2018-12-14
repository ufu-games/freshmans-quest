using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSpawn : MonoBehaviour, IInteractable {

	public enum Type {OneTime, MultipleTimes}
	public Type Modo = Type.MultipleTimes;
	// Use this for initialization

	private bool used = false;
	private Transform child;

	void Start () {
		foreach(Transform transf in GetComponentsInChildren<Transform>()) {
			if(transf != this.transform) {
				child = transf;
			}
		}
		child.gameObject.GetComponent<SpriteRenderer>().enabled = false;
		GetComponent<SpriteRenderer>().enabled = false;
	}
	
	void Update () {
		
	}

	public void Interact() {
		LevelTransition Leveltrans = GameObject.FindGameObjectWithTag("Transitioner").GetComponent<LevelTransition>();
		if(Leveltrans == null) {
			print("Falta o LevelTransitioner na Cena, a SetSpawn não vai funcionar");
			return;
		}
		if(Modo == Type.OneTime) {
			if(!used && Leveltrans.InColliders.Count == 1) {
				Leveltrans.InColliders[0].GetComponent<ScreenTransition>().spawnpoint = child.position;
				Leveltrans.SetSpawnPoint();
				used = true;
			} else {
				return;
			}
		} else {
			Leveltrans.InColliders[0].GetComponent<ScreenTransition>().spawnpoint = child.position;
			Leveltrans.SetSpawnPoint();
		}

		foreach(GameObject go in Leveltrans.m_nowCollider.GetComponent<ScreenTransition>().m_resettables) { 
			if(go.GetComponent<CollectableBehavior>() != null) {
				CollectableBehavior collect = go.GetComponent<CollectableBehavior>();
				if(collect.Collected) {
					Leveltrans.m_nowCollider.GetComponent<ScreenTransition>().m_resettables.Remove(go);
				}
			}
			if(go.tag == "BreakableWall") {
				if(go.GetComponent<SpriteRenderer>().enabled == false) {
					Leveltrans.m_nowCollider.GetComponent<ScreenTransition>().m_resettables.Remove(go);
				}
			}
		}
	}
}
