using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTrigger : MonoBehaviour {
	public float damage;
	public List<int> targets;
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}
	void OnTriggerEnter2D(Collider2D other) {
		if(targets.Contains(other.gameObject.layer)){
			other.gameObject.GetComponent<HealthManager>().TakeDamage(this.damage);
			Debug.Log("pego");
		}
	}
}
