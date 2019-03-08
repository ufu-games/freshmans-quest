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
		public GameObject loadingScreenPrefab;

		private MaskableGraphic blackScreenToFade;

		void Awake() {
			QualitySettings.vSyncCount = 0;
			StartCoroutine(changeFrameRate());

			if(instance == null) {
				instance = this;

				// is this bad?
				DontDestroyOnLoad(gameObject);
			} else {
				Destroy(gameObject);
			}

			
		}

		void Start() {
			GameObject go = GameObject.FindGameObjectWithTag("BlackScreen");
			if(go) {
				blackScreenToFade = go.GetComponent<MaskableGraphic>();
				if(blackScreenToFade) {
					blackScreenToFade.GetComponent<Image>().enabled = true;
					blackScreenToFade.color = new Color(blackScreenToFade.color.r,blackScreenToFade.color.g,blackScreenToFade.color.b,1);
				} else {
					print("Tela preta não encontrada, o Fading não funcionará");
				}
			} else {
				print("Tela preta não encontrada, o Fading não funcionará");
			}
			FadeOut(fadeDuration);
		}

		public void ReloadLevel() { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }

		public void LoadNextLevel() {
			int sceneCount = SceneManager.sceneCountInBuildSettings;
			int sceneToLoad = (SceneManager.GetActiveScene().buildIndex + 1) % sceneCount;
			if(sceneToLoad > 2) {
				SaveSystem.instance.OnLevelEnter(sceneToLoad);
			}
			SceneManager.LoadScene(sceneToLoad);
		}

		private IEnumerator LoadSceneWithLoadingScreenRoutine(int levelIndex) {
			Debug.LogWarningFormat("Loading Scene with Level Index: {0}", levelIndex);
			
			// Instantiating Loading Screen Canvas;
			GameObject loadingScreen = Instantiate(loadingScreenPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			DontDestroyOnLoad(loadingScreen);
			LoadingScreen loadingScreenScript = loadingScreen.GetComponent<LoadingScreen>();
			AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(levelIndex);

			// Make this Fade out
			float previousMusicVolume = SoundManager.instance.musicVolume;
			SoundManager.instance.musicVolume = 0.0f;
			SoundManager.instance.UpdateAudioSources();

			while(!loadingOperation.isDone) {
				// Debug.LogWarning("Loading Progress: " + loadingOperation.progress);
				loadingScreenScript.UpdateProgressText((loadingOperation.progress / 0.9f));
				yield return null;
			}
			// Debug.LogWarning("Destroying Loading Screen 1");

			// Make this Fade In
			SoundManager.instance.musicVolume = previousMusicVolume;
			SoundManager.instance.UpdateAudioSources();

			// Debug.LogWarning("Destroying Loading Screen 2");
			Destroy(loadingScreen);
		}

		public void LoadSceneWithLoadingScreen(int levelIndex) {
			StartCoroutine(LoadSceneWithLoadingScreenRoutine(levelIndex));
		}

		public void LoadLevel(string levelName) {
			Debug.LogWarningFormat("Loading Scene with Level Name: {0}", levelName);
			Debug.LogWarningFormat("Level Index from Name: {0}", SceneManager.GetSceneByName(levelName).buildIndex);
			if(Application.CanStreamedLevelBeLoaded(levelName)) {
				if(levelName != "Hub" && levelName != "MenuInicial" && levelName != "IntroducaoHistoria") {
					SaveSystem.instance.OnLevelEnter(SceneManager.GetSceneByName(levelName).buildIndex);
				}
				LoadSceneWithLoadingScreen(SceneManager.GetSceneByName(levelName).buildIndex);
			} else {
				Debug.LogError("LevelManager Error: invalid scene specified (" + levelName + " )");
			}
		}

		public void LoadLevel(int levelIndex) {
			Debug.Log("Load Level: Int");
			if(levelIndex >= 0 && levelIndex < SceneManager.sceneCountInBuildSettings) {
				if(levelIndex > 2) {
					SaveSystem.instance.OnLevelEnter(levelIndex);
				}
				LoadSceneWithLoadingScreen(levelIndex);
				// SceneManager.LoadScene(levelIndex);
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

