using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace LevelManagement {
	public class LevelManager : MonoBehaviour {
		public static LevelManager instance;

		void Awake() {
			if(instance == null) {
				instance = this;
				DontDestroyOnLoad(gameObject);
			} else {
				Destroy(gameObject);
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
			if(levelIndex >= 0 && levelIndex < SceneManager.sceneCountInBuildSettings) {
				SceneManager.LoadScene(levelIndex);
			} else {
				Debug.LogError("Level Manager Error: invalid scene index specified (" + levelIndex + ")");
			}
		}


	}
}

