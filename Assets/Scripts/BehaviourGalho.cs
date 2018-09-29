using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourGalho : MonoBehaviour {
	public GameObject Tree;
	private Vector3 RotationCenter;
	public float normalSpeed;
	public float angrySpeed;
	private float speed;
	public float damage;
	public int orientation = 1;
	public float fastSpeedTime;
	public float offsetX;
	public float offsetY;
	private Vector3 offset;

    // Use this for initialization
    void Start () {
		this.offset = new Vector3(GetComponent<BoxCollider2D>().bounds.size.x*offsetX,GetComponent<BoxCollider2D>().bounds.size.y*offsetY,0);
		this.RotationCenter = this.transform.position - this.offset;
		this.speed = normalSpeed;
	}
	
	// Update is called once per frame
	void Update () {
		transform.RotateAround(this.RotationCenter, new Vector3(0,0,1), this.orientation*this.speed*Time.deltaTime);
		//Debug.Log("speed: " + this.speed);
	}

	public void SetHighSpeed(){
		this.speed = angrySpeed;
		StartCoroutine(RotationTime());
	}

	private IEnumerator RotationTime(){
		yield return new WaitForSeconds (this.fastSpeedTime);
		this.speed = normalSpeed;
	}
}
