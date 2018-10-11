using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class TIBossBehavior : MonoBehaviour {

	public GameObject[] Buttons;
	public TIBaseState MyState;
	public int HP = 3;
	public bool isDead = false;
	public float timeBetweenStages = 2f;
	private Camera m_cam;
	[Header("--Face Text--")]
	public Text FaceText;
	public float FaceTextXOffset = 1f;
	public float FaceTextYOffset = 1f;
	public int TextSize = 65;
	public bool isTextVisible = true;
	public Color TextColor = Color.white;
	private Vector3 m_text_offset;
	[Header("--Logic Game--")]
	public float TimerXOffset = -4f;
	public float TimerYOffset = 2f;

	void Start () {
		MyState = new TIGenius();
		m_cam = Camera.main;
		SetFaceText(":D");
		FaceText.color = TextColor;
		FaceText.fontSize = TextSize;
		m_text_offset = new Vector3(FaceTextXOffset,FaceTextYOffset,0);
	}

	void Update () {
		m_text_offset.x = FaceTextXOffset;
		m_text_offset.y = FaceTextYOffset;
		FaceText.rectTransform.position = m_cam.WorldToScreenPoint(this.transform.position + m_text_offset);
	}

	public void ButtonActivate(int i, string buttontext){

	}

	public void ButtonDeactivate(int i){

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
		SetFaceText("°O°");
		yield return new WaitForSeconds(1f);
		FaceTextXOffset += 0.2f;
		isDead = true;
		SetFaceText("XP");
	}

	public IEnumerator TakeDamage(){
		FaceTextXOffset -= 0.2f;
		SetFaceText("°O°");
		yield return new WaitForSeconds(1f);
		FaceTextXOffset += 0.2f;
		SetFaceText(":D");
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

	public void SetFaceText(string text){
		FaceText.text = text;
	}

	public void SetFaceText(string line1, string line2){
		FaceText.text = line1 + "\n" + line2;
	}
}
