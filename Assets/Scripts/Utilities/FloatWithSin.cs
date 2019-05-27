using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatWithSin : MonoBehaviour {
	
	Vector3 originalPosition;
	private float m_angle = 0f;
    private float m_speed = 1f;
    private float m_speedDivisor = 8f;

	void Start() {
		originalPosition = transform.position;
        m_angle = Random.Range(0, 360);
        m_speed = Random.Range(0.1f, 2.0f);
        m_speedDivisor = Random.Range(2.0f, 8.0f);
	}

	void Update() {
		m_angle += (m_speed * Time.deltaTime);
		transform.position = new Vector3(originalPosition.x, originalPosition.y + (Mathf.Sin(m_angle) / m_speedDivisor), originalPosition.z);
	}
}
