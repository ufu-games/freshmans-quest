using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonBehaviour : MonoBehaviour {
	public float throwMultiplier;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public float getAngle(){
		return this.gameObject.transform.eulerAngles.z;
	}
	public float getThrowMultiplier(){
		return this.throwMultiplier;
	}
}
