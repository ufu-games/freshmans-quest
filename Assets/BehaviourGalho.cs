using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourGalho : MonoBehaviour {
	public GameObject Tree;
	private Vector3 RotationCenter;
	public float speed;
	public float angrySpeed;
	public float damage;
	public Vector3 offset;

    // Use this for initialization
    void Start () {
		this.RotationCenter = this.Tree.transform.position + this.offset;
	}
	
	// Update is called once per frame
	void Update () {
		transform.RotateAround(this.RotationCenter, new Vector3(0,0,1), this.speed*Time.deltaTime);
	}
}
