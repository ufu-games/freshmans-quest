using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class NotAffectChildScale : MonoBehaviour {

	public float FixeScale = 1;
	public GameObject parent;
	
	private Vector3 vect;

	void Start() {
		vect = new Vector3(0,0,0);
	}
	// Update is called once per frame
	void Update () {
		vect.x = FixeScale/parent.transform.localScale.x;
		vect.y = FixeScale/parent.transform.localScale.y;
		vect.z = FixeScale/parent.transform.localScale.z;
		transform.localScale = vect;
	}
}
 