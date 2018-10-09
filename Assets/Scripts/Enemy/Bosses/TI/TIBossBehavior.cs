using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TIBossBehavior : MonoBehaviour {

	public GameObject[] Buttons;
	public TIBaseState MyState;
	public int HP = 3;

	void Start () {
		MyState = new TIGenius();
	}

	void Update () {

	}

	public void ButtonActivate(int i, string buttonlabel){

	}

	public void ButtonDeactivate(int i){

	}

	public void DealDMGBoss(int dmg){
		if(HP - dmg <= 0){
			HP = 0;
		} else {
			HP -= dmg;
		}
		//GetComponent<Animator>().Play("TITakeDamage");
	}

	public void DealDMGPlayer(int dmg){
		GameObject.Find("Player").GetComponent<HealthManager>().TakeDamage((float) dmg);
	}
}
