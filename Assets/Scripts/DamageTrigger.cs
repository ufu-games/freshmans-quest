using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTrigger : MonoBehaviour {
	public float damage;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnTriggerEnter(Collider other) {
		Debug.Log("hmm");
		if(other.gameObject.name == "Player"){
			other.gameObject.GetComponent<HealthManager>().TakeDamage(this.damage);
		}
	}
}
