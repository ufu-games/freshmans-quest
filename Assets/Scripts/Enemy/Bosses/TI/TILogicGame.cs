using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TILogicGame : TIBaseState {

	private Animator ani;
	private int QuestionNumber;
	private bool Pressed1 = false;
	private bool Pressed2 = false;
	private float x_offset_timer;
	private float y_offset_timer;
	private bool isLampOn;

	public TILogicGame(float x_offset, float y_offset){
		x_offset_timer = x_offset;
		y_offset_timer = y_offset;
	}

	void ButtonPressed(int i){
		if(i == 1){
			Pressed1 = true;
			GetComponent<TIBossBehavior>().ButtonDeactivate(2);
		}
		if(i == 2){
			Pressed2 = true;
			GetComponent<TIBossBehavior>().ButtonDeactivate(1);
		}
	}

	void Create () {
		ani = GetComponent<Animator>();
		ani.Play("LogicInitialAnimation");
		StartCoroutine(Flow());
	}

	void MyDestroy(){
		Destroy(this);
	}

	public IEnumerator Flow(){
		yield return new WaitForSeconds(1f);
		if(Random.Range(0,2) == 0) {
			ani.Play("LogicQuestion1"); //questão 1, se a luz estiver acessa, faça
			QuestionNumber = 1;
		} else {
			ani.Play("LogicQuestion2"); //questão 2, se a luz estiver apagada, faça
			QuestionNumber = 2;
		}
		GameObject timer = (GameObject) Instantiate(Resources.Load("TILogicTimer"),this.transform);
		GameObject light = GameObject.Find("TILamp");
		if(Random.Range(0,2) == 0) {
			isLampOn = true;
			light.GetComponent<SpriteRenderer>().sprite = Resources.Load("lampOn") as Sprite;
		} else {
			isLampOn = false;
			light.GetComponent<SpriteRenderer>().sprite = Resources.Load("lampOff") as Sprite;
		}
		Vector2 offset = new Vector2(x_offset_timer,y_offset_timer);
		timer.transform.position += (Vector3) offset;
		if(Random.Range(0,2) == 0) {
			GetComponent<TIBossBehavior>().ButtonActivate(1,"Eu recebo Dano");
			GetComponent<TIBossBehavior>().ButtonActivate(2,"Professor recebe Dano");
		} else {
			GetComponent<TIBossBehavior>().ButtonActivate(2,"Eu recebo Dano");
			GetComponent<TIBossBehavior>().ButtonActivate(1,"Professor recebe Dano");
		}
		yield return new WaitForSeconds(1f);
		timer.GetComponent<SpriteRenderer>().sprite = Resources.Load("timer2") as Sprite;
		yield return new WaitForSeconds(1f);
		timer.GetComponent<SpriteRenderer>().sprite = Resources.Load("timer1") as Sprite;
		yield return new WaitForSeconds(1f);
		timer.GetComponent<SpriteRenderer>().sprite = Resources.Load("timer0") as Sprite;
		yield return new WaitForSeconds(1f);
		if(QuestionNumber == 1 && isLampOn){
			if(Pressed1){
				GetComponent<TIBossBehavior>().DealDMGPlayer(1);
			}
			if(Pressed2){
				GetComponent<TIBossBehavior>().DealDMGBoss(1);
			}
		}
		if(QuestionNumber == 2 && !isLampOn){
			if(Pressed1){
				GetComponent<TIBossBehavior>().DealDMGPlayer(1);
			}
			if(Pressed2){
				GetComponent<TIBossBehavior>().DealDMGBoss(1);
			}
		}
		yield return new WaitForSeconds(1f);
	}
}
