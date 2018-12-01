using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropperBehavior : MonoBehaviour, IResettableProp {

	public float TimeBetweenDrops;
	public float DropFallSpeed;
	private GameObject dropReference;
	private List<GameObject> m_drops = new List<GameObject>();

	public void Reset() {
		foreach(GameObject drop in m_drops) {
			if(drop != null) {
				Destroy(drop);
			}
		}
		m_drops.Clear();
		StopAllCoroutines();
		StartCoroutine(Lifetime());
	}

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
				m_drops.Add(drop);
			} else {
				print("Falha ao criar Drop no objeto: " + this.name);
			}
		}
	}
}
