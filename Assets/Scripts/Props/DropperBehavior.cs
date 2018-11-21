using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropperBehavior : MonoBehaviour {

	public float TimeBetweenDrops;
	public float DropFallSpeed;
	private GameObject dropReference;

	void Start () {
		dropReference = (GameObject) Resources.Load("Drop");	
		StartCoroutine(Lifetime());
	}
	
	private IEnumerator Lifetime(){
		while(this.enabled == true) {
			yield return new WaitForSeconds(TimeBetweenDrops);
			GameObject drop = Instantiate(dropReference,transform.position,Quaternion.identity);
			if(drop != null) {
				drop.GetComponent<DropBehavior>().Velocity = DropFallSpeed;
			} else {
				print("Falha ao criar Drop no objeto: " + this.name);
			}
		}
	}
}
