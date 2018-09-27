using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthManager : MonoBehaviour {
	public float Hp;
	private bool invulnerable = false;
	public float invulnerabilityTime;
	public Vector2 knockbackForce = new Vector2(5f, 5f);
	private SpriteRenderer m_spriteRenderer;
	private Rigidbody2D m_rigidbody;

	void Start() {
		m_spriteRenderer = GetComponent<SpriteRenderer>();
		m_rigidbody = GetComponent<Rigidbody2D>();
	}

	public void TakeDamage(float damage){
		if(this.invulnerable == false){
			this.invulnerable = true;
			if(Hp - damage <= 0){
				Hp = 0;
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			} else {
				this.Hp -= damage;
			}
			StartCoroutine(InvulnerabilityTimer());
		}
	}

	public void Knockback() {
		Vector2 knockDirection = new Vector2(-Mathf.Sign(transform.localScale.x) * knockbackForce.x, knockbackForce.y);

		m_rigidbody.velocity = knockDirection;
	}
	private IEnumerator InvulnerabilityTimer(){
		float elapsedTime = 0;

		while(elapsedTime < this.invulnerabilityTime) {
			if(m_spriteRenderer) m_spriteRenderer.enabled = !m_spriteRenderer.enabled;
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		if(m_spriteRenderer) m_spriteRenderer.enabled = true;
		this.invulnerable = false;
	}
	void OnTriggerStay2D(Collider2D other) {
		if(other.tag == "Enemy" || other.tag == "DamageSource"){
			Knockback();
			TakeDamage(1);
		}
	}
}
