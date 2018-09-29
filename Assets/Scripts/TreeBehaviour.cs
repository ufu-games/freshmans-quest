using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBehaviour : MonoBehaviour {
	private GameObject galho;
	public int hp = 1;
	// Use this for initialization
	void Start () {
		for(int i = 0;i < transform.childCount;i++){
			if(this.transform.GetChild(i).gameObject.name == "galho"){
				this.galho = this.transform.GetChild(i).gameObject;
				break;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void changeOrientation(){
		this.takeDamage();
		this.galho.GetComponent<BehaviourGalho>().orientation *= -1;
	}

	public void fastRotation(){
		this.takeDamage();
		this.galho.GetComponent<BehaviourGalho>().SetHighSpeed();
	}
	private void takeDamage(){
		Debug.Log("ai");
		if(this.hp-- <= 0){
			Debug.Log("morri");
			Destroy(gameObject);
		}
	}
}
