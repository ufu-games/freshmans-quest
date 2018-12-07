using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour {

	public TextMeshProUGUI fpsText;
	private int fpsCounter = 0;
	private int amountOfFramesFPS = 30;

	void Start() {
		fpsCounter = 0;
	}

	void Update() {
		fpsCounter++;

		if(fpsCounter >= amountOfFramesFPS) {
			fpsText.text =  "FPS: "	+ Mathf.Round((1 / Time.deltaTime));
			fpsCounter = 0;
		}
	}
}
