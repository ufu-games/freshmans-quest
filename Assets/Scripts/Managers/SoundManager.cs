using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	public static SoundManager instance;
	public AudioSource musicSource;
	public AudioSource sfxSource;

	void Awake() {
		if(instance == null) {
			instance = this;
		} else {
			Destroy(gameObject);
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
