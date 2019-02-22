using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TitleScreenManager : MonoBehaviour {

	public AudioClip pressClip;
	public AudioClip titleScreenMusic;
	

	[Header("Freshmans Quest (Press X To Play...) Screen")]
	public GameObject pressStartObject;
	public GameObject confirmButton;
	private int m_confirmButtonOffsetY = 100;
	
	[Header("Selectable Objects")]
	public GameObject playButton;
	// primeiro das opcoes aqui
	
	[Header("Menus")]
	public GameObject optionsObject;
	public GameObject optionsGameObject;
	public GameObject creditsObject;
	
	private MaskableGraphic[] m_optionsGraphics;
	private bool m_canOffset;
	private const int offsetOptionsCredits = -1205;
	private GameObject m_lastSelectedObjectByInputSystem;

	public enum ECurrentState {
		OnPressStart,
		OnCredits,
		OnOptions,
		OnMainMenu
	}

	private ECurrentState m_currentState = ECurrentState.OnPressStart;

	void Start() {
		if(SoundManager.instance) {
			SoundManager.instance.ChangeMusic(titleScreenMusic);
		}
		

		/* Escondendo Todos os Objetos no Options Menu */
		if(optionsObject) {
			m_optionsGraphics = optionsObject.transform.GetComponentsInChildren<MaskableGraphic>();
			foreach(MaskableGraphic m in m_optionsGraphics) {
				m.canvasRenderer.SetAlpha(0);
			}
		}

		/* Mostrando na Tela o Logo do Jogo e o botãozinho mostrando qual botão apertar */
		StartCoroutine(OffsetGameObject(confirmButton, 0, m_confirmButtonOffsetY, 1.0f));
		StartCoroutine(FadePressStartRoutine(0, 1, 1.0f));
	}

	/* Corrotina responsavel por exibir ou esconder o logo na tela */
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

	/* Transição */
	/* MainMenu => Press Start */
	private IEnumerator TransitionFromMainMenuRoutine() {
		SoundManager.instance.SetParameterFMOD("MainThemeTransition", -1.0f);

		/* Fazendo o Fade Out de todas as imagens do menu principal */
		foreach(MaskableGraphic m in m_optionsGraphics) {
			m.CrossFadeAlpha(0f, 0.5f, true);
		}

		/* Mostrando na Tela o Logo do Jogo e o botãozinho mostrando qual botão apertar */
		StartCoroutine(OffsetGameObject(confirmButton, 0, m_confirmButtonOffsetY, 1.0f));
		yield return StartCoroutine(FadePressStartRoutine(0, 1, 1.0f));
		m_currentState = ECurrentState.OnPressStart;
	}

	/* Transição */
	/* Press Start => Main Menu */
	private IEnumerator TransitionToMainMenuRoutine() {
		SoundManager.instance.SetParameterFMOD("MainThemeTransition", 1.0f);

		/* Tirando da tela o Logo do Jogo e o botãozinho mostrando qual botão apertar */
		StartCoroutine(OffsetGameObject(confirmButton, 0, -m_confirmButtonOffsetY, 1.0f));
		yield return StartCoroutine(FadePressStartRoutine(1, 0, 1.0f));
		
		/* Fazendo o Fade In de todas as imagens do menu Principal */
		foreach(MaskableGraphic m in m_optionsGraphics) {
			m.CrossFadeAlpha(1f, 1.0f, true);
		}

		/* Definindo os Estados Inicias do Menu Principal */
		m_currentState = ECurrentState.OnMainMenu;

		if(m_lastSelectedObjectByInputSystem == null) {
			SelectGameObjectOnEventSystem(playButton);
		} else {
			SelectLastSelected();
		}
		
	}

	/* Translada um GameObject por uma certa quantidade em um certo tempo */
	 private IEnumerator OffsetGameObject(GameObject obj, int xOffset, int yOffset, float transitionTime) {
		 m_canOffset = false;

		 RectTransform gameobjectTransform = obj.gameObject.GetComponent<RectTransform>();
		 
		 Vector3 initialPosition = gameobjectTransform.position;
		 Vector3 futurePosition = initialPosition;
		 futurePosition.x += xOffset;
		 futurePosition.y += yOffset;

		 float timeElapsed = 0;

		 while(timeElapsed < transitionTime) {
			 timeElapsed += Time.deltaTime;

			 float t = Interpolation.EaseOut(timeElapsed / transitionTime);
			 Vector3 tempPosition = Vector3.Lerp(initialPosition, futurePosition, t);
			 gameobjectTransform.position = tempPosition;
			 yield return null;
		 }

		 gameobjectTransform.position = futurePosition;
		 yield return null;

		 m_canOffset = true;
	 }

	private void SelectGameObjectOnEventSystem(GameObject obj) {
		EventSystem.current.SetSelectedGameObject(obj);
	}
	private void DisselectCurrent() {
		m_lastSelectedObjectByInputSystem = EventSystem.current.currentSelectedGameObject;
		EventSystem.current.SetSelectedGameObject(null);
	}

	private void SelectLastSelected() {
		EventSystem.current.SetSelectedGameObject(m_lastSelectedObjectByInputSystem);
	}

	public void ShowCredits() {
		if(!m_canOffset) return;

		StartCoroutine(OffsetGameObject(creditsObject, offsetOptionsCredits, 0, 1.0f));
		StartCoroutine(OffsetGameObject(optionsObject, -offsetOptionsCredits, 0, 1.0f));
		m_currentState = ECurrentState.OnCredits;

		DisselectCurrent();
	}

	public void ShowOptions() {
		if(!m_canOffset) return;

		StartCoroutine(OffsetGameObject(optionsGameObject, offsetOptionsCredits, 0, 1.0f));
		StartCoroutine(OffsetGameObject(optionsObject, -offsetOptionsCredits, 0, 1.0f));
		m_currentState = ECurrentState.OnOptions;

		DisselectCurrent();
	}
	
	void Update () {
		if(InputManager.instance.PressedConfirm()) {
			switch(m_currentState) {
				case ECurrentState.OnPressStart:
					StartCoroutine(TransitionToMainMenuRoutine());
				break;
			}
		}

		if(InputManager.instance.PressedCancel()) {
			switch(m_currentState) {
				case ECurrentState.OnMainMenu:
					if(m_canOffset) {
						DisselectCurrent();
						StartCoroutine(TransitionFromMainMenuRoutine());
					}
				break;
				case ECurrentState.OnCredits:
					if(m_canOffset) {
						StartCoroutine(OffsetGameObject(creditsObject, -offsetOptionsCredits, 0, 1.0f));StartCoroutine(OffsetGameObject(optionsObject, offsetOptionsCredits, 0, 1.0f));
						m_currentState = ECurrentState.OnMainMenu;

						SelectLastSelected();
					}
				break;
				case ECurrentState.OnOptions:
					if(m_canOffset) {
						StartCoroutine(OffsetGameObject(optionsGameObject, -offsetOptionsCredits, 0, 1.0f));
						StartCoroutine(OffsetGameObject(optionsObject, offsetOptionsCredits, 0, 1.0f));
						m_currentState = ECurrentState.OnMainMenu;
						SelectLastSelected();
					}
				break;
			}
		}
	}
}
