using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HeartSystem : MonoBehaviour {
	
	public Sprite[] heartSprites;
	public Image heartUI;
	
	
	void Start () {
		heartUI.sprite = heartSprites [3];
	}
	
	public void attHearts(int vida){
		heartUI.sprite = heartSprites[vida];
	}
}
