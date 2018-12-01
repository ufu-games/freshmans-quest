using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PizzaCounterUI : MonoBehaviour {

	public static PizzaCounterUI instance;
	public TextMeshProUGUI pizzaCounter;
	void Awake() {
		if(instance == null) instance = this;
		else Destroy(gameObject);
	}

	public void UpdateCounter(int count) {
		pizzaCounter.text = count.ToString();
	}
	
}
