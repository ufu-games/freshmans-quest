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
	
	[Header("Selectable Objects")]
	public GameObject playButton;
	public GameObject firstSelectableOptionsMenu;
	public GameObject firstSelectableProfile;
	public Selectable[] mainMenuSelectables;
	public Selectable[] optionsMenuSelectables;
	public Selectable[] profileSelectables;
	
	[Header("Menus")]
	public GameObject optionsObject;
	public GameObject optionsGameObject;
	public GameObject creditsObject;
	public GameObject selectProfileObject;
	public ProfileUI[] profileUIs;

	[Header("Options Menu Object")]
	public Slider musicSlider;
	public Slider sfxSlider;
	
	private MaskableGraphic[] m_optionsGraphics;
	private bool m_canOffset;
	private const int m_confirmButtonOffsetY = 150;
	private Vector2 anchoredPointCenter = new Vector2(0,0);
	private Vector2 rightAnchoredPoint = new Vector2(1600, 0);
	private Vector2 leftAnchoredPoint = new Vector2(-1600, 0);
	private Vector2 upAnchoredPoint = new Vector2(0, 900);
	private Vector2 downAnchoredPoint = new Vector2(0, -900);
	private GameObject m_lastSelectedObjectByInputSystem;

	public enum ECurrentState {
		OnPressStart,
		OnCredits,
		OnOptions,
		OnMainMenu,
		OnSelectProfile,
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

		musicSlider.value = SoundManager.instance.musicVolume;
		sfxSlider.value = SoundManager.instance.sfxVolume;

		for(int i = 0; i < 3; i++) {
			profileUIs[i].UpdateProfileUI(SaveSystem.instance.UISlotIsInUse(i), SaveSystem.instance.UIExtractInfo(i));
		}
	}

	#region Options Menu Function
	public void ChangedMusicVolume() {
		SoundManager.instance.musicVolume = musicSlider.value;
		SoundManager.instance.UpdateAudioSources();
	}

	public void ChangedSoundEffectsVolume() {
		SoundManager.instance.sfxVolume = sfxSlider.value;
		SoundManager.instance.UpdateAudioSources();
	}
	#endregion

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

	
	private IEnumerator OffsetRectTransformToAnchoredPoint(RectTransform rectTransform, Vector2 anchoredPoint, float transitionTime) {
		m_canOffset = false;
		float timeElapsed = 0f;
		Vector2 initialAnchorPoint = rectTransform.anchoredPosition;

		while(timeElapsed < transitionTime) {
			timeElapsed += Time.deltaTime;
			float t = Interpolation.EaseOut(timeElapsed / transitionTime);
			rectTransform.anchoredPosition = Vector3.Lerp(initialAnchorPoint, anchoredPoint, t);
			yield return null;
		}
		
		rectTransform.anchoredPosition = anchoredPoint;
		yield return null;
		m_canOffset = true;
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


	private void ChangeSelectableState(Selectable[] selectableArray, bool state) {
		foreach(Selectable selectable in selectableArray) {
			selectable.interactable = state;
		}
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

	public void ShowSelectProfile() {
		if(!m_canOffset) return;
		StartCoroutine(OffsetRectTransformToAnchoredPoint(selectProfileObject.GetComponent<RectTransform>(), anchoredPointCenter, 1.0f));
		StartCoroutine(OffsetRectTransformToAnchoredPoint(optionsObject.GetComponent<RectTransform>(), downAnchoredPoint, 1.0f));
		m_currentState = ECurrentState.OnSelectProfile;
		
		DisselectCurrent();
		ChangeSelectableState(mainMenuSelectables, false);
		ChangeSelectableState(optionsMenuSelectables, false);
		ChangeSelectableState(profileSelectables, true);
		SelectGameObjectOnEventSystem(firstSelectableProfile);
	}

	public void ShowCredits() {
		if(!m_canOffset) return;

		StartCoroutine(OffsetRectTransformToAnchoredPoint(creditsObject.GetComponent<RectTransform>(), anchoredPointCenter, 1.0f));
		StartCoroutine(OffsetRectTransformToAnchoredPoint(optionsObject.GetComponent<RectTransform>(), leftAnchoredPoint, 1.0f));
		m_currentState = ECurrentState.OnCredits;

		DisselectCurrent();
		ChangeSelectableState(mainMenuSelectables, false);
		ChangeSelectableState(profileSelectables, false);
		ChangeSelectableState(optionsMenuSelectables, false);
	}

	public void ShowOptions() {
		if(!m_canOffset) return;
		StartCoroutine(OffsetRectTransformToAnchoredPoint(optionsGameObject.GetComponent<RectTransform>(), anchoredPointCenter, 1.0f));
		StartCoroutine(OffsetRectTransformToAnchoredPoint(optionsObject.GetComponent<RectTransform>(), leftAnchoredPoint, 1.0f));
		m_currentState = ECurrentState.OnOptions;

		DisselectCurrent();

		ChangeSelectableState(mainMenuSelectables, false);
		ChangeSelectableState(profileSelectables, false);
		ChangeSelectableState(optionsMenuSelectables, true);
		SelectGameObjectOnEventSystem(firstSelectableOptionsMenu);
	}

	public void StartGame(int profileNumber) {
		Debug.LogWarningFormat("Starting game on Profile {0}", profileNumber);

		if(SaveSystem.instance.UISlotIsInUse(profileNumber)) {
			SaveSystem.instance.UILoadGame(profileNumber);
		} else {
			SaveSystem.instance.UICreateSaveSlot(profileNumber);
			LevelManagement.LevelManager.instance.LoadNextLevel();
		}
	}

	public void QuitGame() {
		Application.Quit();
	}
	
	void Update () {
		if(InputManager.instance.PressedConfirm()) {
			switch(m_currentState) {
				case ECurrentState.OnPressStart:
					StartCoroutine(TransitionToMainMenuRoutine());
					ChangeSelectableState(mainMenuSelectables, true);
					ChangeSelectableState(profileSelectables, false);
					ChangeSelectableState(optionsMenuSelectables, false);
				break;
			}
		}

		if(InputManager.instance.PressedCancel()) {
			switch(m_currentState) {
				case ECurrentState.OnMainMenu:
					if(m_canOffset) {
						DisselectCurrent();
						StartCoroutine(TransitionFromMainMenuRoutine());
						ChangeSelectableState(mainMenuSelectables, false);
						ChangeSelectableState(profileSelectables, false);
						ChangeSelectableState(optionsMenuSelectables, false);
					}
				break;
				case ECurrentState.OnCredits:
					if(m_canOffset) {
						StartCoroutine(OffsetRectTransformToAnchoredPoint(creditsObject.GetComponent<RectTransform>(), rightAnchoredPoint, 1.0f));
						StartCoroutine(OffsetRectTransformToAnchoredPoint(optionsObject.GetComponent<RectTransform>(), anchoredPointCenter, 1.0f));
						m_currentState = ECurrentState.OnMainMenu;

						ChangeSelectableState(mainMenuSelectables, true);
						ChangeSelectableState(profileSelectables, false);
						ChangeSelectableState(optionsMenuSelectables, false);
						SelectLastSelected();
					}
				break;
				case ECurrentState.OnOptions:
					if(m_canOffset) {
						StartCoroutine(OffsetRectTransformToAnchoredPoint(optionsGameObject.GetComponent<RectTransform>(), rightAnchoredPoint, 1.0f));
						StartCoroutine(OffsetRectTransformToAnchoredPoint(optionsObject.GetComponent<RectTransform>(), anchoredPointCenter, 1.0f));
						m_currentState = ECurrentState.OnMainMenu;

						ChangeSelectableState(mainMenuSelectables, true);
						ChangeSelectableState(profileSelectables, false);
						ChangeSelectableState(optionsMenuSelectables, false);
						SelectLastSelected();
					}
				break;
				case ECurrentState.OnSelectProfile:
					if(m_canOffset) {
						StartCoroutine(OffsetRectTransformToAnchoredPoint(optionsObject.GetComponent<RectTransform>(), anchoredPointCenter, 1.0f));
						StartCoroutine(OffsetRectTransformToAnchoredPoint(selectProfileObject.GetComponent<RectTransform>(), upAnchoredPoint, 1.0f));
						m_currentState = ECurrentState.OnMainMenu;
						ChangeSelectableState(mainMenuSelectables, true);
						ChangeSelectableState(profileSelectables, false);
						ChangeSelectableState(optionsMenuSelectables, false);
						SelectLastSelected();
					}
				break;
			}
		}
	}
}
