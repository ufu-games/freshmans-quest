using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatWithSin : MonoBehaviour {
	
	Vector3 originalPosition;
	float angle = 0f;

	void Start() {
		originalPosition = transform.position;
	}

	void Update() {
		angle += Time.deltaTime;
		transform.position = new Vector3(originalPosition.x, originalPosition.y + (Mathf.Sin(angle) / 8), originalPosition.z);
	}
}
