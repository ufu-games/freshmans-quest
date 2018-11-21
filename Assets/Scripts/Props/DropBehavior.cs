using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropBehavior : MonoBehaviour {

	public float Velocity = 1f;
	private Vector3 vect;
	private BoxCollider2D boxCollider;
	private Animator m_ani;
	private bool Stopped;
	// Use this for initialization

	void Start () {
		boxCollider = GetComponent<BoxCollider2D>();
		m_ani = GetComponent<Animator>();
		vect.x = 0;
		vect.y = -Velocity;
		vect.z = 0;
	}
	
	void Update () {
		if(!Stopped) {
			vect.y = -Velocity;
			transform.position += vect;
		}
		foreach(RaycastHit2D hit in Physics2D.RaycastAll(transform.position + Vector3.down*boxCollider.size.y/2,Vector2.down,Velocity)) {
			if((hit.collider.gameObject.layer == LayerMask.NameToLayer("Platform") || hit.collider.gameObject.layer == LayerMask.NameToLayer("Slippery")) && !Stopped) {
				Stopped = true;
				m_ani.Play("Death");
				StartCoroutine(waitForDestroy());
			}
		}
	}

	private IEnumerator waitForDestroy() {
		yield return new WaitForSeconds(0.18f);
		Destroy(this.gameObject);
	}
}
