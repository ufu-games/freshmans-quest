using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBehaviour : MonoBehaviour {
	public float throwMultiplier;
	public float zoomOutMultiplier = 1.2f;
	public bool active = false;
	public bool UseAnalogic;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(active && (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)){
			if(!UseAnalogic){
				transform.Rotate(Vector3.forward * -Input.GetAxisRaw("Horizontal"));
			}
			else{
				float heading = Mathf.Atan2(-Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
				Debug.Log(heading);
				transform.rotation=Quaternion.Euler(0f,0f,heading*Mathf.Rad2Deg);
			}
		}
	}

	public float getAngle(){
		return this.gameObject.transform.eulerAngles.z;
	}
	public float getThrowMultiplier(){
		return this.throwMultiplier;
	}
	public void setActive(bool a){
		this.active = a;
	}

}
