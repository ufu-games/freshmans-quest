using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OlhoBehaviour : MonoBehaviour {
	private bool closed = false;
	private float invulnerabilityTime = 1;
	private SpriteRenderer m_spriteRenderer;
	private bool invulnerable = false;
	// Use this for initialization
	void Start () {
		m_spriteRenderer = GetComponent<SpriteRenderer>();
		if(m_spriteRenderer) m_spriteRenderer.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private IEnumerator InvulnerabilityTimer(){
		float elapsedTime = 0;

		while(elapsedTime < this.invulnerabilityTime) {
			//if(m_spriteRenderer) m_spriteRenderer.enabled = !m_spriteRenderer.enabled;
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		//if(m_spriteRenderer) m_spriteRenderer.enabled = true;
		if(m_spriteRenderer) m_spriteRenderer.enabled = false;
		this.invulnerable = false;
	}

	void TakeDamage(float damage){
		if(m_spriteRenderer) m_spriteRenderer.enabled = true;
		if(!invulnerable){
			this.invulnerable = true;
			StartCoroutine(InvulnerabilityTimer());
			if(closed == false){
				closed = true;
				transform.parent.gameObject.GetComponent<TreeBehaviour>().changeOrientation();
			} else{
				closed = false;
				transform.parent.gameObject.GetComponent<TreeBehaviour>().fastRotation();
			}
		}
	}

	void OnTriggerStay2D(Collider2D other) {
		if(other.gameObject.name == "PlayerMeleeAttack(Clone)"){
			TakeDamage(other.gameObject.GetComponent<DamageTrigger>().damage);
		}
	}
}
