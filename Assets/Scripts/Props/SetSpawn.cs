using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSpawn : MonoBehaviour, IInteractable {

	public enum Type {OneTime, MultipleTimes}
	public Type Modo = Type.OneTime;
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
	
	// Update is called once per frame
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
			}
		} else {
			Leveltrans.InColliders[0].GetComponent<ScreenTransition>().spawnpoint = child.position;
			Leveltrans.SetSpawnPoint();
		}
	}
}
