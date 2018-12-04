using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*
	Level Manager
	Como utilizar: LevelManagement.LevelManager.instance.nomeDaFuncao
	               (namespace)  (classe)    (instancia) (função)
	Funções que podem ser utilizadas:

	ReloadLevel()
		- Recarrega o Level Atual
		Ex: LevelManagement.LevelManager.instance.ReloadLevel();
	LoadNextLevel()
		- Carrega a próxima cena na build, se a cena atual for a útlima, carrega a primeira cena.
		Ex: LevelManagement.LevelManager.instance.LoadNextLevel();
	LoadLevel(string)
		- Carrega a cena com o nome passado por parâmetro.
		Ex: LevelManagement.LevelManager.instance.LoadLevel("Hub");
	LoadLevel(int)
		- Carrega a cena com o indice passado por parâmetro
		Ex: LevelManagement.LevelManager.instance.LoadLevel(2);
 */


namespace LevelManagement {
	public class LevelManager : MonoBehaviour {
		public static LevelManager instance;
		public float fadeDuration = .1f;
		public int MaxFrameRate = 60;

		private MaskableGraphic blackScreenToFade;

		void Awake() {
			QualitySettings.vSyncCount = 0;
			StartCoroutine(changeFrameRate());

			if(instance == null) {
				instance = this;
				GameObject go = GameObject.FindGameObjectWithTag("BlackScreen");
				if(go) {
					blackScreenToFade = go.GetComponent<MaskableGraphic>();
					if(blackScreenToFade) {
						blackScreenToFade.gameObject.SetActive(true);
						blackScreenToFade.GetComponent<Image>().enabled = true;
						blackScreenToFade.color = new Color(blackScreenToFade.color.r,blackScreenToFade.color.g,blackScreenToFade.color.b,1);
					} else {
						print("Tela preta não encontrada, o Fading não funcionará");
					}
				} else {
					print("Tela preta não encontrada ou está desativada, o Fading não funcionará");
				}
			} else {
				Destroy(gameObject);
			}
		}

		void Start() {
			FadeOut(fadeDuration);
		}

		void Update() {
			if(Input.GetKeyDown(KeyCode.Escape)) {
				Application.Quit();
			}
			if(Input.GetKeyDown(KeyCode.H)) {
				LoadLevel("DebugHub");
			}
      	}

		public void ReloadLevel() { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }

		public void LoadNextLevel() {
			int sceneCount = SceneManager.sceneCountInBuildSettings;
			int sceneToLoad = (SceneManager.GetActiveScene().buildIndex + 1) % sceneCount;
			SceneManager.LoadScene(sceneToLoad);
		}

		public void LoadLevel(string levelName) {
			if(Application.CanStreamedLevelBeLoaded(levelName)) {
				SceneManager.LoadScene(levelName);
			} else {
				Debug.LogError("LevelManager Error: invalid scene specified (" + levelName + " )");
			}
		}

		public void LoadLevel(int levelIndex) {
			Debug.Log("Load Level: Int");
			if(levelIndex >= 0 && levelIndex < SceneManager.sceneCountInBuildSettings) {
				SceneManager.LoadScene(levelIndex);
			} else {
				Debug.LogError("Level Manager Error: invalid scene index specified (" + levelIndex + ")");
			}
		}

		// =============================================================================
		// =============================================================================
		// 							FADE IN / FADE OUT
		// =============================================================================
		// =============================================================================

		private void Fade(float targetAlpha, float duration) {
			blackScreenToFade.CrossFadeAlpha(targetAlpha, duration, true);
		}

		public void FadeOut(float duration) {
			if(blackScreenToFade == null) return;
			
			blackScreenToFade.canvasRenderer.SetAlpha(1f);
			Fade(0f, duration);
		}

		public void FadeIn(float duration) {
			if(blackScreenToFade == null) return;

			blackScreenToFade.canvasRenderer.SetAlpha(0f);
			Fade(1f, duration);
		}

		public IEnumerator changeFrameRate() {
			Application.targetFrameRate = MaxFrameRate;
			yield return new WaitForSeconds(1f);
		}

	}
}

