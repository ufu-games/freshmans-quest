using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporPlayer : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
   public var Camera1 : Camera;

   public var Camera2 : Camera;

   public var destination : Transform;

function OnTriggerEnter(other : collider)
    {
        if (other.tag == "Player")
        {
            other.transform.position = destination.position;
        }
        Camera1.enabled = false;

        Camera2.enabled = true;

    }


}
