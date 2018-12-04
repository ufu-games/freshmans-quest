using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugUI : MonoBehaviour {
	public bool activateDebugUI = false;
	public GameObject[] debugUIObjects;
	public int updateEveryFrameNFrames = 10;
	private int m_frameCounter;

	[Header("Player Information")]
	public TextMeshProUGUI playervx;
	public TextMeshProUGUI playervy;
	private PlayerController m_playerReference;

	void Start() {
		if(!activateDebugUI) {
			for(int i = 0; i < debugUIObjects.Length; i++) {
				debugUIObjects[i].SetActive(false);
			} 
		} else {
			for(int i = 0; i < debugUIObjects.Length; i++) {
				debugUIObjects[i].SetActive(true);
			}
			m_playerReference = FindObjectOfType<PlayerController>();
		}
	}

	void Update() {
		if(!activateDebugUI) return;
		m_frameCounter++;
		if(m_frameCounter % updateEveryFrameNFrames != 0) return;
		

		if(playervx) {
			playervx.text = "p_x: " + System.Math.Round(m_playerReference.GetVelocity().x, 2);
		}

		if(playervy) {
			playervy.text = "p_y: " + System.Math.Round(m_playerReference.GetVelocity().y, 2);
		}
	}
}
