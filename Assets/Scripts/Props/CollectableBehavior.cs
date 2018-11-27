using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableBehavior : MonoBehaviour, IInteractable {

	public float Value;
	public TypeOfCollectable Type;
	public AudioClip collectedClip;
	public AudioClip continuousClip;
	private float followTime = .65f;
	private float smoothFollow = 2f;

	public enum TypeOfCollectable{Pizza, Homework};

	void Start() {
		if(!continuousClip) {
			print("Falta o AudioClip Continuo do Coletável " + this.name);
		} else {
			if(SoundManager.instance) {
				SoundManager.instance.PlayContinuousSfx(continuousClip,this.gameObject);
			}
		}
	}

	private IEnumerator DestroyCollectableRoutine(PlayerController playerReference) {
		float timeSpent = 0;

		while(timeSpent < followTime) {
			Vector3 tempPosition = playerReference.transform.position;
			timeSpent += Time.deltaTime;
			yield return null;
			transform.position = Vector3.Lerp(transform.position, tempPosition, Time.deltaTime * smoothFollow);
		}

		if(collectedClip && SoundManager.instance) {
			SoundManager.instance.PlaySfx(collectedClip);
		}
		Destroy(gameObject);
	}

	public void Interact(){
		PlayerController t_playerControllerReference = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
		if(Type == TypeOfCollectable.Pizza) {
			t_playerControllerReference.PizzaCollected += Value;
		}
		if(Type == TypeOfCollectable.Homework) {
			t_playerControllerReference.HomeworkCollected += Value;
		}
		
		StartCoroutine(DestroyCollectableRoutine(t_playerControllerReference));
	}
}
