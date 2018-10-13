using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaliLevelManager : MonoBehaviour {

	public GameObject playerReference;
	public GameObject daliReference;
	public Dialogue[] endLevelDialogue;

	private Vector2 m_lastCheckpoint;
	private bool m_isPlayingDialogue = false;

	void Start() {
		m_lastCheckpoint = Vector2.zero;
	}

	public void PassedCheckpoint(Vector2 position) {
		m_lastCheckpoint = position;
	}

	public void EndOfLevel() {
		daliReference.GetComponent<BossArtes>().StopBoss();
		m_isPlayingDialogue = true;
		DialogueManager.instance.StartDialogue(endLevelDialogue);
	}

	public void ResetPlayer() {
		playerReference.transform.position = m_lastCheckpoint;
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
	
}
