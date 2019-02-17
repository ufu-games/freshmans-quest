using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {
	public static CameraScript instance;
	private const float maxAngle = 5f;
	public float maxOffset = 1f;

	Vector3 initialRotation;
	Cinemachine.CinemachineVirtualCamera m_virtualCamera;
	Transform m_virtualCameraTransform;
	Vector3 m_originalCameraPosition;
	Vector3 m_originalCameraRotation;

	/* Screen Shake */
	public float cameraTrauma;

	void Awake() {
		if(instance == null) {
			instance = this;
		} else {
			DontDestroyOnLoad(gameObject);
		}
	}

	void Start() {
		m_virtualCamera = GameObject.FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
		m_virtualCameraTransform = m_virtualCamera.transform;
		cameraTrauma = 0f;
	}

	void Update() {
		if(cameraTrauma <= 0f) {
			return;
		}

		cameraTrauma -= Time.deltaTime;
		cameraTrauma = Mathf.Clamp(cameraTrauma, 0, 1f);

		if(cameraTrauma <= 0f) {
			this.transform.position = m_originalCameraPosition;
			this.transform.localEulerAngles = m_originalCameraRotation;
		}

		float angle = maxAngle * (cameraTrauma * cameraTrauma) * Random.Range(-1f, 1f);
		float offsetX = maxOffset * (cameraTrauma * cameraTrauma) * Random.Range(-1f, 1f);
		float offsetY = maxOffset * (cameraTrauma * cameraTrauma) * Random.Range(-1f, 1f);

		Vector3 t_position = transform.position;
		Vector3 t_rotation = transform.localEulerAngles;
		t_position.x += offsetX;
		t_position.y += offsetY;
		t_rotation.z += angle;

		this.transform.position = t_position;	
		this.transform.localEulerAngles = t_rotation;
	}

	public void AddTraumaToCamera(float amount) {
		Debug.LogWarning("Adding Trauma!");
		if(cameraTrauma <= 0f) {
			m_originalCameraPosition = this.transform.position;
			m_originalCameraRotation = this.transform.localEulerAngles;

			cameraTrauma = amount;
		} else {
			cameraTrauma += amount;
		}
	}

}
