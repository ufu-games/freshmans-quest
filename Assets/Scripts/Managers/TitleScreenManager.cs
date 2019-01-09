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
		
		if(optionsObject) {
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

		StartCoroutine(JuiceTextSelection(playText, 50, selectedColor));
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

	private IEnumerator JuiceTextSelection(TextMeshProUGUI text, int xOffset, Color color) {
		text.color = color;
		RectTransform textTransform = text.gameObject.GetComponent<RectTransform>();
		
		Vector3 initialTextPosition = textTransform.position;
		Vector3 futureTextPosition = initialTextPosition;
		futureTextPosition.x += xOffset;

		float timeElapsed = 0f;

		while(timeElapsed < 0.1f) {
			timeElapsed += Time.deltaTime;

			float t = Interpolation.EaseOut(timeElapsed / 0.1f);
			textTransform.position = Vector3.Lerp(initialTextPosition, futureTextPosition, t);
			yield return null;
		}
		
		textTransform.position = futureTextPosition;
		yield return null;

	}

	private void ProcessMainMenuState() {
		/* Get Input */
		float verticalValue = 0f;
		verticalValue = Mathf.Round(Input.GetAxisRaw("Vertical"));

		if(verticalValue == m_lastFrameVerticalInput) {
			verticalValue = 0;
		} else {
			m_lastFrameVerticalInput = verticalValue;
		}

		switch(m_currentOptionState) {
			case EOptionState.Play:
				if((verticalValue == 1 || verticalValue == -1)) {
					m_currentOptionState = EOptionState.Exit;

					/* Feedback Visual para o Player */
					StartCoroutine(JuiceTextSelection(playText, -50, notSelectedColor));		
					StartCoroutine(JuiceTextSelection(exitText, 50, selectedColor));
				}

				if(InputManager.instance.PressedConfirm()) {
					Debug.Log("JOGAR!");
					LevelManagement.LevelManager.instance.LoadNextLevel();
				}
			break;
			case EOptionState.Exit:
				if((verticalValue == 1 || verticalValue == -1)) {
					m_currentOptionState = EOptionState.Play;

					/* Feedback Visual para o Player */
					StartCoroutine(JuiceTextSelection(playText, 50, selectedColor));		
					StartCoroutine(JuiceTextSelection(exitText, -50, notSelectedColor));
				}

				if(InputManager.instance.PressedConfirm()) {
					Debug.Log("SAIR!");
					Application.Quit();
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
	
	void Update () {
		ProcessCurrentState();

		/* If Press the confirm button... */
		if(InputManager.instance.PressedConfirm()) {
			switch(m_currentState) {
				case ECurrentState.OnPressStart:
					StartCoroutine(TransitionToMainMenuRoutine());
				break;
			}
		}

		/* When pressing the cancel button */
		if(InputManager.instance.PressedCancel()) {
			switch(m_currentState) {
				case ECurrentState.OnMainMenu:
					StartCoroutine(TransitionFromMainMenuRoutine());
				break;
			}
		}
	}
}
