using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PizzaSliceCounterUI : MonoBehaviour {

	public static PizzaSliceCounterUI instance;
	public Image[] pizzaSlices;

	void Awake() {
		if(instance == null) instance = this;
		else Destroy(gameObject);
	}

	public void UpdateCounter(int counter) {
		int t_counter = Mathf.Min(counter, pizzaSlices.Length);
		for(int i = 0; i < pizzaSlices.Length; i++) {
			pizzaSlices[i].enabled = false;
		}
		for(int i = 0; i < t_counter; i++) {
			pizzaSlices[i].enabled = true;
		}
	}
}
