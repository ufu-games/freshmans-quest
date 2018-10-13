using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HubManager : MonoBehaviour {

	public WorldPoint[] challengesPositions;
	public GameObject playerReference;
	public float transitionTime = 0.2f;
	public Text pressToPlayText;
	public AudioClip hubMusic;
	private int m_playerPositionOnChallenges;
	private bool m_isMoving;
	private bool m_isShowingDialogue;
	

	void Start() {
		playerReference.transform.position = challengesPositions[0].positionOnMap;
		m_playerPositionOnChallenges = 0;
		pressToPlayText.text = "";
		SoundManager.instance.ChangeMusic(hubMusic);
		m_isMoving = false;
	}

	private IEnumerator MovePlayerRoutine(float timeToMove) {
		m_isMoving = true;
		Vector2 initialPosition = playerReference.transform.position;
		Vector2 destination = challengesPositions[m_playerPositionOnChallenges].positionOnMap;
		float elapsedTime = 0.0f;
		bool reached = false;

		while(!reached) {
			if(Vector2.Distance(playerReference.transform.position, destination) < 0.05f) {
				reached = true;
				break;
			}

			elapsedTime += Time.deltaTime;
			float t = Mathf.Clamp(elapsedTime / timeToMove, 0f, 1f);
			t = t*t*t*(t*(t*6 - 15) + 10);
			playerReference.transform.position = Vector3.Lerp(initialPosition, destination, t);
			yield return null;
		}

		playerReference.transform.position = destination;
		m_isMoving = false;
	}

	private void MovePlayer() {
		StartCoroutine(MovePlayerRoutine(transitionTime));
	}

	void Update() {

		if(m_isMoving || DialogueManager.instance.isShowingDialogue) return;

		if(Input.GetKeyDown(KeyCode.RightArrow)) {
			pressToPlayText.text = "";
			m_playerPositionOnChallenges = Mathf.Clamp((m_playerPositionOnChallenges + 1), 0, challengesPositions.Length - 1);
			MovePlayer();
		} else if(Input.GetKeyDown(KeyCode.LeftArrow)) {
			pressToPlayText.text = "";
			m_playerPositionOnChallenges = Mathf.Clamp((m_playerPositionOnChallenges - 1), 0, challengesPositions.Length - 1);
			MovePlayer();
		}

		if(Input.GetKeyDown(KeyCode.K)) {
			if(challengesPositions[m_playerPositionOnChallenges].isAvailable) {
				LevelManagement.LevelManager.instance.LoadLevel(challengesPositions[m_playerPositionOnChallenges].levelName);
			}
		}

		if(Input.GetButtonDown("Submit") && !m_isShowingDialogue) {
			pressToPlayText.text = "";
			m_isShowingDialogue = true;
			DialogueManager.instance.StartDialogue(challengesPositions[m_playerPositionOnChallenges].dialogueText);
		}

		if(!DialogueManager.instance.isShowingDialogue && m_isShowingDialogue) {
			if(challengesPositions[m_playerPositionOnChallenges].isAvailable) {
				pressToPlayText.text = "Pressione K para entrar";
			}

			m_isShowingDialogue = false;
		}
		
	}


}
