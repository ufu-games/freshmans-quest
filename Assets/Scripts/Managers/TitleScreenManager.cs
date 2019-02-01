using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitleScreenManager : MonoBehaviour {

	public AudioClip pressClip;
	public AudioClip titleScreenMusic;

	[Header("Title Screen Object")]
	[Header("Press Start Screen")]
	public GameObject pressStartObject;
	public GameObject confirmButton;
	private int m_confirmButtonOffsetY = 100;
	
	[Header("Main Menu")]
	public GameObject optionsObject;
	public Color selectedColor = Color.red;
	public Color notSelectedColor = Color.white;
	public TextMeshProUGUI playText;
	public TextMeshProUGUI optionsText;
	public TextMeshProUGUI creditsText;
	public TextMeshProUGUI exitText;
	private MaskableGraphic[] m_optionsGraphics;

	[Header("Options")]
	public GameObject optionsGameObject;

	[Header("Credits")]
	public GameObject creditsObject;

	private float m_lastFrameVerticalInput;
	private bool m_canGetInput;
	private bool m_canOffset;

	private const int offsetOptionsCredits = -1255;
	

	public enum ECurrentState {
		OnPressStart,
		OnCredits,
		OnOptions,
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

		/* Verificando que os offsets nao vao ficar errados */
		switch(m_currentOptionState) {
			case EOptionState.Play:
				StartCoroutine(JuiceTextSelection(playText, -50, notSelectedColor));
			break;

			case EOptionState.Options:
				StartCoroutine(JuiceTextSelection(optionsText, -50, notSelectedColor));
			break;

			case EOptionState.Credits:
				StartCoroutine(JuiceTextSelection(creditsText, -50, notSelectedColor));
			break;

			case EOptionState.Exit:
				StartCoroutine(JuiceTextSelection(exitText, -50, notSelectedColor));
			break;
		}

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
		StartCoroutine(JuiceTextSelection(playText, 50, selectedColor));
		m_currentState = ECurrentState.OnMainMenu;
		m_currentOptionState = EOptionState.Play;
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

	/* JUICING da seleção de texto do Menu Principal */
	/* Faz um Offset no x e "balança" com seno */
	/* Offset a text a certain amount of x */
	private IEnumerator JuiceTextSelection(TextMeshProUGUI text, int xOffset, Color color) {
		m_canGetInput = false;

		text.color = color;
		RectTransform textTransform = text.gameObject.GetComponent<RectTransform>();
		
		Vector3 initialTextPosition = textTransform.position;
		Vector3 futureTextPosition = initialTextPosition;
		futureTextPosition.x += xOffset;

		float timeElapsed = 0f;

		while(timeElapsed < 0.1f) {
			timeElapsed += Time.deltaTime;
			float t = Interpolation.EaseOut(timeElapsed / 0.1f);
			/* Horizontal Offset with EaseOut and Linear Interpolation */
			Vector3 tempPosition = Vector3.Lerp(initialTextPosition, futureTextPosition, t);
			/* Vertical Offset with Sin */
			tempPosition.y += (Mathf.Sin((timeElapsed / 0.1f) * Mathf.PI) * 10);
			
			textTransform.position = tempPosition;
			yield return null;
		}
		
		textTransform.position = futureTextPosition;
		yield return null;

		m_canGetInput = true;
	}

	/* Função Que Processa o Estado Atual selecionado na Cena */
	private void ProcessMainMenuState() {
		if(!m_canGetInput || !m_canOffset) return;
		/* Get Input */
		float verticalValue = 0f;
		verticalValue = Mathf.Round(Input.GetAxisRaw("Vertical"));

		/* Impedindo que acontecçam mudancas indesejadas de opcoes */
		if(verticalValue == m_lastFrameVerticalInput) {
			verticalValue = 0;
		} else {
			m_lastFrameVerticalInput = verticalValue;
		}

		switch(m_currentOptionState) {
			case EOptionState.Play:

				/* Verifica se tem que mudar de opção */
				if(verticalValue == 1) {
					m_currentOptionState = EOptionState.Exit;
					/* Feedback Visual para o Player */
					StartCoroutine(JuiceTextSelection(playText, -50, notSelectedColor));		
					StartCoroutine(JuiceTextSelection(exitText, 50, selectedColor));
				} else if(verticalValue == -1) {
					m_currentOptionState = EOptionState.Options;
					/* Feedback Visual para o Player */
					StartCoroutine(JuiceTextSelection(playText, -50, notSelectedColor));		
					StartCoroutine(JuiceTextSelection(optionsText, 50, selectedColor));
				}

				/* Se o Jogador apertar CONFIRMAR... */
				if(InputManager.instance.PressedConfirm()) {
					Debug.Log("JOGAR!");
					LevelManagement.LevelManager.instance.LoadNextLevel();
				}
			break;

			case EOptionState.Options:
				if(verticalValue == 1) {
					m_currentOptionState = EOptionState.Play;
					/* Feedback Visual para o Player */
					StartCoroutine(JuiceTextSelection(playText, 50, selectedColor));
					StartCoroutine(JuiceTextSelection(optionsText, -50, notSelectedColor));		
				} else if(verticalValue == -1) {
					m_currentOptionState = EOptionState.Credits;
					/* Feedback Visual para o Player */
					StartCoroutine(JuiceTextSelection(creditsText, 50, selectedColor));
					StartCoroutine(JuiceTextSelection(optionsText, -50, notSelectedColor));		
				}

				if(InputManager.instance.PressedConfirm()) {
					StartCoroutine(OffsetGameObject(optionsGameObject, offsetOptionsCredits, 0, 1.0f));
					StartCoroutine(OffsetGameObject(optionsObject, -offsetOptionsCredits, 0, 1.0f));
					m_currentState = ECurrentState.OnOptions;
				}
			break;

			case EOptionState.Credits:
				if(verticalValue == 1) {
					m_currentOptionState = EOptionState.Options;
					/* Feedback Visual para o Player */
					StartCoroutine(JuiceTextSelection(optionsText, 50, selectedColor));
					StartCoroutine(JuiceTextSelection(creditsText, -50, notSelectedColor));		
				} else if(verticalValue == -1) {
					m_currentOptionState = EOptionState.Exit;
					/* Feedback Visual para o Player */
					StartCoroutine(JuiceTextSelection(exitText, 50, selectedColor));
					StartCoroutine(JuiceTextSelection(creditsText, -50, notSelectedColor));		
				}

				if(InputManager.instance.PressedConfirm()) {
					StartCoroutine(OffsetGameObject(creditsObject, offsetOptionsCredits, 0, 1.0f));
					StartCoroutine(OffsetGameObject(optionsObject, -offsetOptionsCredits, 0, 1.0f));
					m_currentState = ECurrentState.OnCredits;
				}
			break;

			case EOptionState.Exit:
				if(verticalValue == 1) {
					m_currentOptionState = EOptionState.Credits;
					/* Feedback Visual para o Player */
					StartCoroutine(JuiceTextSelection(creditsText, 50, selectedColor));
					StartCoroutine(JuiceTextSelection(exitText, -50, notSelectedColor));		
				} else if(verticalValue == -1) {
					m_currentOptionState = EOptionState.Play;
					/* Feedback Visual para o Player */
					StartCoroutine(JuiceTextSelection(playText, 50, selectedColor));
					StartCoroutine(JuiceTextSelection(exitText, -50, notSelectedColor));		
				}

				/* Se o Jogador apertar CONFIRMAR... */
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
					if(m_canOffset) {
						StartCoroutine(TransitionFromMainMenuRoutine());
					}
				break;
				case ECurrentState.OnCredits:
					if(m_canOffset) {
						StartCoroutine(OffsetGameObject(creditsObject, -offsetOptionsCredits, 0, 1.0f));StartCoroutine(OffsetGameObject(optionsObject, offsetOptionsCredits, 0, 1.0f));
						m_currentState = ECurrentState.OnMainMenu;
					}
				break;
				case ECurrentState.OnOptions:
					if(m_canOffset) {
						StartCoroutine(OffsetGameObject(optionsGameObject, -offsetOptionsCredits, 0, 1.0f));
						StartCoroutine(OffsetGameObject(optionsObject, offsetOptionsCredits, 0, 1.0f));
						m_currentState = ECurrentState.OnMainMenu;
					}
				break;
			}
		}
	}
}
