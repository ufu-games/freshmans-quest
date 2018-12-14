using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitleScreenManager : MonoBehaviour {

	public AudioClip pressClip;
	public AudioClip titleScreenMusic;
	[Header("Title Screen Object")]
	public GameObject pressStartObject;
	// public TextMeshProUGUI pressStartText;
	public GameObject optionsObject;
	public Color selectedColor = Color.red;
	public Color notSelectedColor = Color.white;
	public TextMeshProUGUI playText;
	public TextMeshProUGUI exitText;
	private MaskableGraphic[] m_optionsGraphics;


	// odeio isso
	private float m_lastFrameVerticalInput;
	

	public enum ECurrentState {
		OnPressStart,
		OnMainMenu
	}

	public enum EOptionState {
		Play,
		Options,
		Credits,
		Exit
	}

	private ECurrentState m_currentState = ECurrentState.OnPressStart;
	private EOptionState m_currentOptionState = EOptionState.Play;

	void Start() {
		if(SoundManager.instance) {
			SoundManager.instance.ChangeMusic(titleScreenMusic);
		}

		// if(pressStartObject) pressStartObject.SetActive(false);
		
		if(optionsObject) {
			// optionsObject.SetActive(false);
			m_optionsGraphics = optionsObject.transform.GetComponentsInChildren<MaskableGraphic>();
			foreach(MaskableGraphic m in m_optionsGraphics) {
				m.canvasRenderer.SetAlpha(0);
			}
		}

		StartCoroutine(FadePressStartRoutine(0, 1, 1.0f));
	}

	private IEnumerator FadePressStartRoutine(int fadeFrom, int fadeTo, float fadeTime) {
		pressStartObject.SetActive(true);
		Vector3 fadeFromScale = new Vector3(fadeFrom, fadeFrom, fadeFrom);
		pressStartObject.transform.localScale = fadeFromScale;
		Vector3 fadeToScale = new Vector3(fadeTo, fadeTo, fadeTo);
		float timeElapsed = 0f;

		while(timeElapsed < fadeTime) {
			float t = timeElapsed / fadeTime;
			pressStartObject.transform.localScale = Vector3.Lerp(fadeFromScale, fadeToScale, t);
			timeElapsed += Time.deltaTime;
			yield return null;
		}

		pressStartObject.transform.localScale = fadeToScale;
	}

	/* MainMenu => Press Start */
	private IEnumerator TransitionFromMainMenuRoutine() {
		foreach(MaskableGraphic m in m_optionsGraphics) {
			m.CrossFadeAlpha(0f, 0.5f, true);
		}

		yield return StartCoroutine(FadePressStartRoutine(0, 1, 1.0f));
		m_currentState = ECurrentState.OnPressStart;
	}

	/* Press Start => Main Menu */
	private IEnumerator TransitionToMainMenuRoutine() {
		yield return StartCoroutine(FadePressStartRoutine(1, 0, 1.0f));
		
		foreach(MaskableGraphic m in m_optionsGraphics) {
			m.CrossFadeAlpha(1f, 1.0f, true);
		}

		m_currentState = ECurrentState.OnMainMenu;
	}

	private IEnumerator LoadNext() {
		yield return new WaitForSeconds(1.0f);
		LevelManagement.LevelManager.instance.LoadNextLevel();
	}

	/*
		================================================================
		================================================================
			PROCESSING STATES
		================================================================
		================================================================
	 */

	private void ProcessMainMenuState() {
		/* Get Input */
		float verticalValue = 0f;
		verticalValue = Mathf.Round(Input.GetAxisRaw("Vertical"));
		
		// Debug.Log("Vertical: " + verticalValue);
		// Debug.Log("Last Frame: " + m_lastFrameVerticalInput);

		if(verticalValue == m_lastFrameVerticalInput) {
			verticalValue = 0;
		} else {
			m_lastFrameVerticalInput = verticalValue;
		}

		switch(m_currentOptionState) {
			case EOptionState.Play:
				if((verticalValue == 1 || verticalValue == -1)) {
					m_currentOptionState = EOptionState.Exit;		
				}

				// Feedback visual para saber em qual opcao o player esta
				playText.color = selectedColor;
				exitText.color = notSelectedColor;

				if(Input.GetButtonDown("Submit")) {
					Debug.Log("JOGAR!");
				}
			break;
			case EOptionState.Exit:
				if((verticalValue == 1 || verticalValue == -1)) {
					m_currentOptionState = EOptionState.Play;
				}

				// Feedback visual para saber em qual opcao o player esta
				playText.color = notSelectedColor;
				exitText.color = selectedColor;

				if(Input.GetButtonDown("Submit")) {
					Debug.Log("SAIR!");
				}
			break;
		}
	}

	private void ProcessCurrentState() {
		switch(m_currentState) {
			case ECurrentState.OnMainMenu:
				ProcessMainMenuState();
			break;
		}
	}
	

	/*
		- Trocar Submit e Cancel.
		- Incorporar eles ao InputManager.
	*/
	void Update () {
		ProcessCurrentState();

		if(Input.GetButtonDown("Submit")) {
			switch(m_currentState) {
				case ECurrentState.OnPressStart:
					StartCoroutine(TransitionToMainMenuRoutine());
				break;
				case ECurrentState.OnMainMenu:
				break;
			}
		}

		if(Input.GetButtonDown("Cancel")) {
			switch(m_currentState) {
				case ECurrentState.OnMainMenu:
					StartCoroutine(TransitionFromMainMenuRoutine());
				break;
			}
		}
	}
}
