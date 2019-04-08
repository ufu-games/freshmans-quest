using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour {

	public static SoundManager instance;
	[Header("Music")]
	public bool musicEnabled = true;
	[Range(0,1)]
	public float musicVolume = 1f;
	
	[Header("Sound Effects")]
	public bool sfxEnabled = true;
	[Range(0,1)]
	public float sfxVolume = 1f;
	StudioEventEmitter fmodEventEmitter;
	FMOD.Studio.Bus masterBus;
	[Space(10)]
	public SoundSettings Settings;

	void Awake() {
		if(instance == null) {
			instance = this;
			fmodEventEmitter = GetComponent<StudioEventEmitter>();
			masterBus = FMODUnity.RuntimeManager.GetBus("Bus:/");
			// is this bad?
			DontDestroyOnLoad(gameObject);
		} else if(instance != this) {
			// instance.fmodEventEmitter.ChangeEvent(GetComponent<StudioEventEmitter>().Event);
			// instance.fmodEventEmitter.Play();
			switch (SceneManager.GetActiveScene().buildIndex) {
				case 0:
					if(Settings.TitleScreen != "")
						instance.fmodEventEmitter.ChangeEvent(Settings.TitleScreen);		
					break;
				case 1:
					if(Settings.Hub != "")
						instance.fmodEventEmitter.ChangeEvent(Settings.Hub);
					break;
				case 2:
					if(Settings.Hub != "")
						instance.fmodEventEmitter.ChangeEvent(Settings.Hub);
					break;
				case 3:
					if(Settings.University != "")
						instance.fmodEventEmitter.ChangeEvent(Settings.University);
					break;
				case 4:
					if(Settings.Arts != "")
						instance.fmodEventEmitter.ChangeEvent(Settings.Arts);
					break;
				case 5:
					if(Settings.Chemistry != "")
						instance.fmodEventEmitter.ChangeEvent(Settings.Chemistry);
					break;
				case 6:
					instance.fmodEventEmitter.ChangeEvent(null);
					break;
				default:
					Destroy(gameObject);
					break;
				
			}
			instance.fmodEventEmitter.Play();
			Destroy(gameObject);
		}
	}

	public void UpdateAudioSources() {
		if(masterBus.isValid()) masterBus.setVolume(musicVolume);
	}

	public void SetParameterFMOD(string parameter, float value) {
		if(fmodEventEmitter) {
			fmodEventEmitter.SetParameter(parameter, value);
		}
	}

	public void ChangeMusic(string clip) {
		fmodEventEmitter.ChangeEvent(clip);
		fmodEventEmitter.Play();
	}

	public void PlaySfx(string clip) {
		FMODUnity.RuntimeManager.PlayOneShot(clip, transform.position);
	}

	// public void StopSfx() {
	// 	sfxSource.Stop();
	// }

	// public void PlaySfxWithTimeOffset(AudioClip clip, float time) {
	// 	StartCoroutine(TimeOffsetSfx(clip,time));
	// }

	public void PlayContinuousSfx(string clip, GameObject audioPlayer) {
		StartCoroutine(ContinuousSfxCoroutine(clip,audioPlayer));
	}

	private IEnumerator ContinuousSfxCoroutine(string clip, GameObject audioPlayer) {
		StudioEventEmitter stdEmitter = audioPlayer.AddComponent<StudioEventEmitter>();
		stdEmitter.Event = clip;
		stdEmitter.Play();
		while(audioPlayer) {
			yield return null;
		}
		Destroy(stdEmitter);
	}

	// private IEnumerator TimeOffsetSfx(AudioClip clip, float time) {
	// 	AudioSource source = gameObject.AddComponent<AudioSource>();
	// 	source.clip = clip;
	// 	source.time = time;
	// 	source.loop = false;
	// 	source.volume = sfxSource.volume;
	// 	source.Play();
	// 	while(source.isPlaying) {
	// 		yield return null;
	// 	}
	// 	Destroy(source);
	// }
}
