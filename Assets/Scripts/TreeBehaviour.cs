using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBehaviour : MonoBehaviour {
	public GameObject galho;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void changeOrientation(){
		this.galho.GetComponent<BehaviourGalho>().orientation *= -1;
	}

	public void fastRotation(){
		this.galho.GetComponent<BehaviourGalho>().SetHighSpeed();
	}
}
