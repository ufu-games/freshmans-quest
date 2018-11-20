using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableBehavior : MonoBehaviour, IInteractable {

	public float Value;
	public TypeOfCollectable Type;

	public enum TypeOfCollectable{Pizza, Homework};

	public void Interact(){
		if(Type == TypeOfCollectable.Pizza) {
			GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().PizzaCollected += Value;
		}
		if(Type == TypeOfCollectable.Homework) {
			GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().HomeworkCollected += Value;
		}
		Destroy(this.gameObject);
	}
}
