using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour {

	public float timeToDestroy = 0.5f;

	private IEnumerator DestroyAfterSeconds(float t) {
		yield return new WaitForSeconds(t);
		Destroy(gameObject);
	}

	void Start () {
		StartCoroutine(DestroyAfterSeconds(timeToDestroy));
	}
}
