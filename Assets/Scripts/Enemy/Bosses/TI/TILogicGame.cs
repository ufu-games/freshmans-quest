using System.Collections;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine;

public class TILogicGame : TIBaseState {

	private Animator ani;
	private int QuestionNumber;
	private bool Pressed1 = false;
	private bool Pressed3 = false;
	public float x_offset_timer;
	public float y_offset_timer;
	public TextMeshProUGUI qtext;
	public float q_x_offset;
	public float q_y_offset;
	public int qsize;
	public Color qcolor;
	private bool isLampOn;
	private int buttonDamage;
	private GameObject timer;
	private GameObject light;
	private Vector2 m_text_offset;

	public override void ButtonPressed(int i){
		if(i == 1){
			Pressed1 = true;
			GetComponent<TIBossBehavior>().ButtonDeactivate(3);
		}
		if(i == 3){
			Pressed3 = true;
			GetComponent<TIBossBehavior>().ButtonDeactivate(1);
		}
	}

	public override void Create () {
		Debug.Log("jjjjjj");
		ani = GetComponent<Animator>();
		GetComponent<TIBossBehavior>().FaceTextDeactivate();
		timer = (GameObject) Instantiate(Resources.Load("TILogicTimer"));
		timer.transform.position = this.transform.position;
		Vector2 offset = new Vector2(x_offset_timer,y_offset_timer);
		timer.transform.position += (Vector3) offset;
		light = GameObject.Find("TILamp");
		UpdateText();
		ani.Play("LogicInitialAnimation");
		StartCoroutine(Flow());
	}

	public override void MyDestroy(){
		GetComponent<TIBossBehavior>().ButtonDeactivate(1);
		GetComponent<TIBossBehavior>().ButtonDeactivate(3);
		light.GetComponent<SpriteRenderer>().enabled = false;
		qtext.text = "";
		Destroy(timer.gameObject);
		Destroy(timer);
		Destroy(this);
	}

	public void UpdateText(){
		m_text_offset.x = q_x_offset;
		m_text_offset.y = q_y_offset;
		qtext.fontSize = qsize;
		qtext.color = qcolor;
		qtext.rectTransform.position = Camera.main.WorldToScreenPoint(this.transform.position + (Vector3)m_text_offset);
	}

	public IEnumerator Flow(){
		while(true){
			yield return new WaitForSeconds(1f);
			Pressed1 = false;
			Pressed3 = false;
			if(Random.Range(0,2) == 0) {
				qtext.text = "Se a luz\nestiver acessa,\nfaça";//questão 1, se a luz estiver acessa, faça
				QuestionNumber = 1;
			} else {
				qtext.text = "Se a luz\nestiver apagada,\nfaça"; //questão 2, se a luz estiver apagada, faça
				QuestionNumber = 2;
			}
			GameObject light = GameObject.Find("TILamp");
			if(Random.Range(0,2) == 0) {
				isLampOn = true;
				light.GetComponent<SpriteRenderer>().enabled = true;
			} else {
				isLampOn = false;
				light.GetComponent<SpriteRenderer>().enabled = false;
			}
			this.buttonDamage = Random.Range(0,2);
			if(this.buttonDamage == 0) {
				GetComponent<TIBossBehavior>().ButtonActivate(1,Color.red);
				GetComponent<TIBossBehavior>().ButtonActivate(3,Color.blue);
			} else {
				GetComponent<TIBossBehavior>().ButtonActivate(3,Color.red);
				GetComponent<TIBossBehavior>().ButtonActivate(1,Color.blue);
			}
			this.buttonDamage ++;
			timer.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("timer5");
			yield return new WaitForSeconds(1f);
			timer.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("timer4");
			yield return new WaitForSeconds(1f);
			timer.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("timer3");
			yield return new WaitForSeconds(1f);
			timer.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("timer2");
			yield return new WaitForSeconds(1f);
			timer.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("timer1");
			yield return new WaitForSeconds(1f);
			timer.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("timer0");
			yield return new WaitForSeconds(1f);
			qtext.text = "";
			
			if(QuestionNumber == 1 && isLampOn){
				if(Pressed1 && buttonDamage == 1){
					GetComponent<TIBossBehavior>().DealDMGPlayer(1);
				}
				if(Pressed3  && buttonDamage == 1){
					GetComponent<TIBossBehavior>().DealDMGBoss(1);
					GetComponent<TIBossBehavior>().FaceTextActivate();
					yield break;
				}
				if(Pressed3 && buttonDamage == 2){
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
				if(Pressed3  && buttonDamage == 1){
					GetComponent<TIBossBehavior>().DealDMGBoss(1);
					GetComponent<TIBossBehavior>().FaceTextActivate();
					yield break;
				}
				if(Pressed3  && buttonDamage == 2){
					GetComponent<TIBossBehavior>().DealDMGPlayer(1);
				}
				if(Pressed1  && buttonDamage == 2){
					GetComponent<TIBossBehavior>().DealDMGBoss(1);
					GetComponent<TIBossBehavior>().FaceTextActivate();
					yield break;
				}
			}
			if( !Pressed1 && !Pressed3 && ((QuestionNumber == 1 && isLampOn)|| (QuestionNumber == 2 && !isLampOn))){
				GetComponent<TIBossBehavior>().DealDMGPlayer(1);
			}
		}
	}
}
