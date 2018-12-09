using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivateBlackScreen : MonoBehaviour {

	void Awake() {
		foreach(Image im in GetComponentsInChildren<Image>(true)) {
			if(im.tag == "BlackScreen") {
				im.gameObject.SetActive(true);
			}
		}
	}
}
