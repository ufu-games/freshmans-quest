using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour {
	public float Hp;
	private bool invulnerable = false;
	public float invulnerabilityTime;
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}
	public void TakeDamage(float damage){
		if(this.invulnerable == false){
			this.invulnerable = true;
			if(Hp - damage <= 0){
				Hp = 0;
				Destroy(this.gameObject);
			} else {
				this.Hp -= damage;
			}
			StartCoroutine(InvulnerabilityTimer());

		}
	}
	private IEnumerator InvulnerabilityTimer(){
		yield return new WaitForSeconds (this.invulnerabilityTime);
		this.invulnerable = false;
	}
	void OnTriggerStay2D(Collider2D other) {
		
		if(other.gameObject.tag == "DamageSource"){
			TakeDamage(other.gameObject.GetComponent<DamageTrigger>().damage);
		}
	}
}
