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

	public static GameObject Player;

	private List<emitters> emits = new List<emitters>();

	void Awake() {
		if(instance == null) {
			instance = this;
			fmodEventEmitter = GetComponent<StudioEventEmitter>();
			masterBus = FMODUnity.RuntimeManager.GetBus("Bus:/");
			Player = GameObject.FindGameObjectWithTag("Player");
			// is this bad?
			DontDestroyOnLoad(gameObject);
		} else if(instance != this) {
			// instance.fmodEventEmitter.ChangeEvent(GetComponent<StudioEventEmitter>().Event);
			// instance.fmodEventEmitter.Play();
			ChangeMusicAccordingToScene();
			Player = GameObject.FindGameObjectWithTag("Player");
			Destroy(gameObject);
		}
	}

	public void ChangeMusicAccordingToScene() {
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
	}

	public void UpdateAudioSources() {
		if(masterBus.isValid()) {
			masterBus.setVolume(musicVolume);
			foreach(emitters emit in emits) {
				emit.UpdateVolume();
			}
		}
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
		emitters emit = new emitters();
		stdEmitter.Event = clip;
		stdEmitter.PlayEvent = EmitterGameEvent.ObjectStart;
		stdEmitter.StopEvent = EmitterGameEvent.ObjectDestroy;
		stdEmitter.Play();
		stdEmitter.EventInstance.setVolume(0);

		emit.source = audioPlayer;
		emit.emit = stdEmitter;

		emits.Add(emit);
		
		while(audioPlayer) {
			if(Player) {
				emit.UpdateVolume();
			}
			yield return null;
		}
		emits.Remove(emit);
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
	private class emitters {
		public StudioEventEmitter emit = null;
		public GameObject source = null;

		public void UpdateVolume() {
			if(Player) {
					float distance = Vector2.Distance(source.transform.position,Player.transform.position);
					if(distance <= 6) {
						emit.EventInstance.setVolume((6-distance)/6);
					}
			}
		}
	}

}
