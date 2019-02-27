using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class SoundManager : MonoBehaviour {

	public static SoundManager instance;
	[Header("Music")]
	public AudioSource musicSource;
	public bool musicEnabled = true;
	[Range(0,1)]
	public float musicVolume = 1f;
	
	[Header("Sound Effects")]
	public AudioSource sfxSource;
	public bool sfxEnabled = true;
	[Range(0,1)]
	public float sfxVolume = 1f;
	StudioEventEmitter fmodEventEmitter;
	FMOD.Studio.Bus masterBus;

	void Awake() {
		if(instance == null) {
			instance = this;
			fmodEventEmitter = GetComponent<StudioEventEmitter>();
			masterBus = FMODUnity.RuntimeManager.GetBus("Bus:/");

			// is this bad?
			DontDestroyOnLoad(gameObject);
		} else if(instance != this) {
			instance.fmodEventEmitter.ChangeEvent(GetComponent<StudioEventEmitter>().Event);
			instance.fmodEventEmitter.Play();
			Destroy(gameObject);
		}
	}

	public void UpdateAudioSources() {
		if(musicSource) musicSource.volume = musicVolume;
		if(sfxSource) sfxSource.volume = sfxVolume;
		if(masterBus.isValid()) masterBus.setVolume(musicVolume);
	}

	public void SetParameterFMOD(string parameter, float value) {
		if(fmodEventEmitter) {
			fmodEventEmitter.SetParameter(parameter, value);
		}
	}

	public void ChangeMusic(AudioClip music) {
		musicSource.Stop();
		musicSource.clip = music;
		musicSource.Play();
	}

	public void PlaySfx(AudioClip clip) {
		sfxSource.PlayOneShot(clip);
	}

	public void StopSfx() {
		sfxSource.Stop();
	}

	public void PlaySfxWithTimeOffset(AudioClip clip, float time) {
		StartCoroutine(TimeOffsetSfx(clip,time));
	}

	public void PlayContinuousSfx(AudioClip clip, GameObject audioPlayer) {
		StartCoroutine(ContinuousSfxCoroutine(clip,audioPlayer));
	}

	private IEnumerator ContinuousSfxCoroutine(AudioClip clip, GameObject audioPlayer) {
		AudioSource Source = audioPlayer.AddComponent<AudioSource>();
		Source.clip = clip;
		Source.rolloffMode = AudioRolloffMode.Linear;
		Source.loop = true;
		Source.volume = 0.1f;
		Source.spatialBlend = 1;
		Source.minDistance = 2;
		Source.maxDistance = 6;
		Source.Play();
		while(audioPlayer) {
			yield return null;
		}
		Destroy(Source);
	}

	private IEnumerator TimeOffsetSfx(AudioClip clip, float time) {
		AudioSource source = gameObject.AddComponent<AudioSource>();
		source.clip = clip;
		source.time = time;
		source.loop = false;
		source.volume = sfxSource.volume;
		source.Play();
		while(source.isPlaying) {
			yield return null;
		}
		Destroy(source);
	}
}
