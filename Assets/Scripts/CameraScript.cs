using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {
	public static CameraScript instance;
	public float idleAmplitude = 0f;
	public float shakeAmplitude = 3f;


	Vector3 initialRotation;
	Cinemachine.CinemachineBasicMultiChannelPerlin m_perlin;
	Cinemachine.CinemachineVirtualCamera m_virtualCamera;

	void Awake() {
		if(instance == null) {
			instance = this;
		} else {
			DontDestroyOnLoad(gameObject);
		}
	}

	void Start() {
		m_virtualCamera = GameObject.FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
		m_perlin = GameObject.FindObjectOfType<Cinemachine.CinemachineBasicMultiChannelPerlin>();
		ResetCamera();
	}

	private IEnumerator ShakeRoutine(float duration, float shakeAmplitude) {
		Debug.Log("Shaking Screen");
		m_perlin.m_AmplitudeGain = shakeAmplitude;
		yield return new WaitForSeconds(duration);
		ResetCamera();
	}

	public void ShakeCamera(float duration) {
		StartCoroutine(ShakeRoutine(duration, shakeAmplitude));
	}

	void ResetCamera() {
		m_perlin.m_AmplitudeGain = idleAmplitude;
	}

}
