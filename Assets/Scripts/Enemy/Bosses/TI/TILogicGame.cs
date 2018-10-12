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
	private int buttonDamage;

	public TILogicGame(float x_offset, float y_offset){
		x_offset_timer = x_offset;
		x_offset_timer = y_offset;
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
		GetComponent<TIBossBehavior>().FaceTextDeactivate();
		ani.Play("LogicInitialAnimation");
		StartCoroutine(Flow());
	}

	void MyDestroy(){
		Destroy(this);
	}

	public IEnumerator Flow(){
		while(true){
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
			this.buttonDamage = Random.Range(0,2);
			if(this.buttonDamage == 0) {
				GetComponent<TIBossBehavior>().ButtonActivate(1,"Eu recebo Dano");
				GetComponent<TIBossBehavior>().ButtonActivate(2,"Professor recebe Dano");
			} else {
				GetComponent<TIBossBehavior>().ButtonActivate(2,"Eu recebo Dano");
				GetComponent<TIBossBehavior>().ButtonActivate(1,"Professor recebe Dano");
			}
			this.buttonDamage ++; 
			yield return new WaitForSeconds(1f);
			timer.GetComponent<SpriteRenderer>().sprite = Resources.Load("timer2") as Sprite;
			yield return new WaitForSeconds(1f);
			timer.GetComponent<SpriteRenderer>().sprite = Resources.Load("timer1") as Sprite;
			yield return new WaitForSeconds(1f);
			timer.GetComponent<SpriteRenderer>().sprite = Resources.Load("timer0") as Sprite;
			yield return new WaitForSeconds(1f);
			
			if(QuestionNumber == 1 && isLampOn){
				if(Pressed1 && buttonDamage == 1){
					GetComponent<TIBossBehavior>().DealDMGPlayer(1);
				}
				if(Pressed2  && buttonDamage == 1){
					GetComponent<TIBossBehavior>().DealDMGBoss(1);
					GetComponent<TIBossBehavior>().FaceTextActivate();
					yield break;
				}
				if(Pressed2 && buttonDamage == 2){
					GetComponent<TIBossBehavior>().DealDMGPlayer(1);
				}
				if(Pressed1  && buttonDamage == 2){
					GetComponent<TIBossBehavior>().DealDMGBoss(1);
					GetComponent<TIBossBehavior>().FaceTextActivate();
					yield break;
				}
			}
			if(QuestionNumber == 2 && !isLampOn){
				if(Pressed1  && buttonDamage == 1){
					GetComponent<TIBossBehavior>().DealDMGPlayer(1);
				}
				if(Pressed2  && buttonDamage == 1){
					GetComponent<TIBossBehavior>().DealDMGBoss(1);
					GetComponent<TIBossBehavior>().FaceTextActivate();
					yield break;
				}
				if(Pressed2  && buttonDamage == 2){
					GetComponent<TIBossBehavior>().DealDMGPlayer(1);
				}
				if(Pressed1  && buttonDamage == 2){
					GetComponent<TIBossBehavior>().DealDMGBoss(1);
					GetComponent<TIBossBehavior>().FaceTextActivate();
					yield break;
				}
			}
			if( !Pressed1 && !Pressed2 && ((QuestionNumber == 1 && isLampOn)|| (QuestionNumber == 2 && !isLampOn))){
				GetComponent<TIBossBehavior>().DealDMGPlayer(1);
			}
			yield return new WaitForSeconds(1f);
		}
	}
}
