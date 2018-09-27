using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTrigger : MonoBehaviour {
	public float damage;
	public List<int> targets;
	
	void OnTriggerEnter2D(Collider2D other) {
		if(targets.Contains(other.gameObject.layer)){
			other.gameObject.GetComponent<HealthManager>().TakeDamage(this.damage);
			Debug.Log("pego");
		} else if(other.tag == "Enemy") {
			other.gameObject.GetComponent<HealthManager>().Knockback();
			other.gameObject.GetComponent<HealthManager>().TakeDamage(1);
		}
	}
}
