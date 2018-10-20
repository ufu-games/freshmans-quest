using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class TIBossBehavior : MonoBehaviour {

	public GameObject[] Buttons;
	public TIBaseState MyState;
	public int HP = 3;
	[HideInInspector]
	public bool isDead = false;
	public float timeBetweenStages = 2f;
	private Camera m_cam;
	[Header("--Face Text--")]
	public Text FaceText;
	public float FaceTextXOffset = 1f;
	public float FaceTextYOffset = -1f;
	public int TextSize = 65;
	[HideInInspector]
	public bool isTextVisible = true;
	public Color TextColor = Color.white;
	[Range(0f,360f)]
	public float Rotation = 270f;
	private Vector3 m_text_offset;
	private Vector3 m_text_rotation;
	[Header("--Logic Game--")]
	public float TimerXOffset = -4f;
	public float TimerYOffset = 2f;
	public Text QuestionText;
	public float QuestionTextXOffset = -3f;
	public float QuestionTextYOffset = 2f;
	public int QuestionTextSize = 30;
	public Color QuestionColor = Color.white;

	void Start () {
		
		m_cam = Camera.main;
		SetFaceText(":D");
		FaceText.color = TextColor;
		m_text_offset = new Vector3(FaceTextXOffset,FaceTextYOffset,0);
		m_text_rotation = new Vector3(FaceText.rectTransform.rotation.x,FaceText.rectTransform.rotation.y,Rotation);
		for(int i = 0;i<5;i++){
			ButtonDeactivate(i);
		}
	}

	void Update () {
		Update_facetext_position();
		if(Input.GetKeyDown(KeyCode.T)){
			DealDMGBoss(1);
		}
		if(Input.GetKeyDown(KeyCode.E)) {
			MyState = gameObject.AddComponent<TILogicGame>() as TILogicGame;
			GetComponent<TILogicGame>().x_offset_timer = TimerXOffset;
			GetComponent<TILogicGame>().y_offset_timer = TimerYOffset;
			GetComponent<TILogicGame>().qtext = QuestionText;
			GetComponent<TILogicGame>().q_x_offset = QuestionTextXOffset;
			GetComponent<TILogicGame>().q_y_offset = QuestionTextYOffset;
			GetComponent<TILogicGame>().qsize = QuestionTextSize;
			GetComponent<TILogicGame>().qcolor = QuestionColor;
			MyState.Create();
		}
		if(Input.GetKeyDown(KeyCode.R)) {
			MyState = gameObject.AddComponent<TIGenius>() as TIGenius;
			MyState.Create();
		}

	}

	public void ButtonActivate(int i, Color color){
		this.Buttons[i].SetActive(true);
		this.Buttons[i].GetComponent<SpriteRenderer>().color = color;
	}

	public void ButtonDeactivate(int i){
		this.Buttons[i].GetComponent<SpriteRenderer>().color = Color.white;
		this.Buttons[i].SetActive(false);

	}
	public void ButtonPressed(int i){
		this.MyState.ButtonPressed(i);
	}
	public void FaceTextActivate(){
		isTextVisible = true;
		FaceText.enabled = true;
	}

	public void FaceTextDeactivate(){
		isTextVisible = false;
		FaceText.enabled = false;
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
		FaceTextXOffset -= 0.2f;
		Update_facetext_position();
		SetFaceText(">:(");
		yield return new WaitForSeconds(1f);
		FaceTextXOffset += 0.2f;
		Update_facetext_position();
		isDead = true;
		SetFaceText("XP");
	}

	public IEnumerator TakeDamage(){
		FaceTextXOffset -= 0.2f;
		Update_facetext_position();
		SetFaceText(">:(");
		yield return new WaitForSeconds(1f);
		FaceTextXOffset += 0.2f;
		Update_facetext_position();
		SetFaceText(":D");
		MyState.MyDestroy();
		yield return new WaitForSeconds(timeBetweenStages);
		switch (Random.Range(0,3)) {
			case 0:
				MyState = gameObject.AddComponent<TIGenius>() as TIGenius;
				break;
			case 1:
				MyState = gameObject.AddComponent<TIPong>() as TIPong;
				break;
			case 2:
				MyState = gameObject.AddComponent<TILogicGame>() as TILogicGame;
				GetComponent<TILogicGame>().x_offset_timer = TimerXOffset;
				GetComponent<TILogicGame>().y_offset_timer = TimerYOffset;
				GetComponent<TILogicGame>().qtext = QuestionText;
				GetComponent<TILogicGame>().q_x_offset = QuestionTextXOffset;
				GetComponent<TILogicGame>().q_y_offset = QuestionTextYOffset;
				GetComponent<TILogicGame>().qsize = QuestionTextSize;
				GetComponent<TILogicGame>().qcolor = QuestionColor;
				break;
			default:
				break;
		}
		MyState.Create();
	}

	public void SetFaceText(string text){
		FaceText.text = text;
	}

	public void SetFaceText(string line1, string line2){
		FaceText.text = line1 + "\n" + line2;
	}

	public void Update_facetext_position(){
		m_text_offset.x = FaceTextXOffset;
		m_text_offset.y = FaceTextYOffset;
		FaceText.rectTransform.position = m_cam.WorldToScreenPoint(this.transform.position + m_text_offset);
		FaceText.rectTransform.rotation = Quaternion.Euler(0,0,Rotation);
		FaceText.fontSize = TextSize;
	}
}
