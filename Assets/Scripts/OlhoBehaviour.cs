using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OlhoBehaviour : MonoBehaviour {
	private bool closed = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void TakeDamage(float damage){
		if(closed == false){
			closed = true;
			transform.parent.gameObject.GetComponent<TreeBehaviour>().changeOrientation();
		} else{
			closed = false;
			transform.parent.gameObject.GetComponent<TreeBehaviour>().fastRotation();
		}
	}

	void OnTriggerStay2D(Collider2D other) {
		
		if(other.gameObject.tag == "AtaquePlayer"){
			TakeDamage(other.gameObject.GetComponent<DamageTrigger>().damage);
		}
	}
}
