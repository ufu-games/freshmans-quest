using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewHubManager : MonoBehaviour {

	float m_transitionTime = 0.5f;
	private bool m_isMoving;

	public enum ELevel {
		Artes = 0,
		Medicina = 1
	}

	public ELevel currentLevel;

	void Start () {
		currentLevel = ELevel.Artes;
		m_isMoving = false;
	}

	IEnumerator MoveRoutine(float timeToMove) {
		
		m_isMoving = true;

		float elapsedTime = 0.0f;
		Vector3 startPosition = Camera.main.transform.position;
		Vector3 destination = new Vector3(17.5f * (int)currentLevel, Camera.main.transform.position.y, Camera.main.transform.position.z);
		bool reachedDestination = false;

		while(!reachedDestination) {
			if(Vector3.Distance(Camera.main.transform.position, destination) < 0.1f) {
				Camera.main.transform.position = destination;
				reachedDestination = true;
				break;
			}

			elapsedTime += Time.deltaTime;
			float t = Mathf.Clamp(elapsedTime / timeToMove, 0f, 1f);
			t = Interpolation.SmootherStep(t);

			Camera.main.transform.position = Vector3.Lerp(startPosition, destination, t);
			yield return null;
		}

		m_isMoving = false;
	}
	
	public void GoLeft() {
		if(m_isMoving) return; 

		StopCoroutine("MoveRoutine");
		currentLevel = (ELevel) Mathf.Max(((int)currentLevel)-1, (int) ELevel.Artes);
		StartCoroutine(MoveRoutine(m_transitionTime));
	}

	public void GoRight() {
		if(m_isMoving) return; 

		StopCoroutine("MoveRoutine");
		currentLevel = (ELevel) Mathf.Min(((int)currentLevel)+1, (int) ELevel.Medicina);
		StartCoroutine(MoveRoutine(m_transitionTime));
	}

	public void ChooseUniversityLevel() {
		Debug.LogWarning("Starting University Level...");
		LevelManagement.LevelManager.instance.LoadLevel(3);
	}

	public void ChooseArtsLevel() {
		Debug.LogWarning("Starting Arts Level...");
	}

	public void ChooseChemistryLevel() {
		Debug.LogWarning("Starting Chesmitry Level...");
		LevelManagement.LevelManager.instance.LoadLevel(4);
	}

	public void ChooseMedicineLevel() {
		Debug.LogWarning("Starting Medicine Level...");
	}

	public void ChoosePhilosophyLevel() {
		Debug.LogWarning("Starting Philosophy Level...");
	}
}
