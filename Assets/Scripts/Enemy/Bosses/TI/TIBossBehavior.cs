using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TIBossBehavior : MonoBehaviour {

	public GameObject[] Buttons;
	public TIBaseState MyState;
	public int HP = 3;
	public bool isDead = false;
	public float timeBetweenStages = 2f;
	[Header("LogicGame")]
	public float TimerXOffset = -2f;
	public float TimerYOffset = 2f;

	void Start () {
		MyState = new TIGenius();
		;
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
			if(!isDead){
				StartCoroutine(Death());
			}
		} else {
			HP -= dmg;
			StartCoroutine(TakeDamage());
		}
	}

	public void DealDMGPlayer(int dmg){
		GameObject.Find("Player").GetComponent<HealthManager>().TakeDamage((float) dmg);
	}

	public IEnumerator Death(){
		GetComponent<Animator>().Play("TIDeath");
		yield return new WaitForSeconds(1f);
		isDead = true;
		GetComponent<Animator>().Play("TIDead");
	}

	public IEnumerator TakeDamage(){
		GetComponent<Animator>().Play("TITakeDamage");
		yield return new WaitForSeconds(1f);
		GetComponent<Animator>().Play("TIIdle");
		yield return new WaitForSeconds(timeBetweenStages);
		MyState.MyDestroy();
		switch (Random.Range(0,3)) {
			case 0:
				MyState = new TIGenius();
				break;
			case 1:
				MyState = new TIPong();
				break;
			case 2:
				MyState = new TILogicGame(TimerXOffset,TimerYOffset);
				break;
			default:
				break;
		}
		MyState.Create();
	}
}
