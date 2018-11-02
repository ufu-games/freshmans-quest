using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaliLevelManager : MonoBehaviour {

	public static DaliLevelManager instance;
	public GameObject playerReference;
	public GameObject daliReference;
	public Dialogue[] endLevelDialogue;
	public AudioClip daliBossMusic;

	private Vector2 m_lastCheckpoint;
	private bool m_isPlayingDialogue = false;

	void Awake() {
		if(instance == null) {
			instance = this;
		}
	}

	void Start() {
		SoundManager.instance.ChangeMusic(daliBossMusic);
		m_lastCheckpoint = playerReference.transform.position;
	}

	public void PassedCheckpoint(Vector2 position) {
		m_lastCheckpoint = position;
	}

	public void EndOfLevel() {
		daliReference.GetComponent<BossArtes>().StopBoss();
	}

	public void ShowFinalDialogue() {
		m_isPlayingDialogue = true;
		DialogueManager.instance.StartDialogue(endLevelDialogue);
	}

	private IEnumerator WaitFadeOut() {
		yield return new WaitForSeconds(0.5f);
		playerReference.SetActive(true);
		LevelManagement.LevelManager.instance.FadeOut(.25f);
	}

	private IEnumerator GoToHub() {
		yield return new WaitForSeconds(1.0f);
		LevelManagement.LevelManager.instance.LoadLevel("Hub");
	}

	void Update() {
		if(m_isPlayingDialogue) {
			if(!DialogueManager.instance.isShowingDialogue) {
				m_isPlayingDialogue = false;
				StartCoroutine(GoToHub());
			}
		}
	}
	
	public IEnumerator ResetPlayer(){
		LevelManagement.LevelManager.instance.FadeIn(.1f);
		playerReference.SetActive(false);
		yield return new WaitForSeconds(0.1f);
		playerReference.transform.position = m_lastCheckpoint;
		Camera.main.transform.position = m_lastCheckpoint;
		if(daliReference.activeSelf) daliReference.GetComponent<BossArtes>().ResetBossPosition();
		StartCoroutine(WaitFadeOut());
		yield return null;
	}
}
