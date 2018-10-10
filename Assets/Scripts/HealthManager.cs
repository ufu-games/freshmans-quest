using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour {
	public AudioClip wasHitClip;
	public float Hp;
	private bool invulnerable = false;
	public float invulnerabilityTime;
	public Vector2 knockbackForce = new Vector2(5f, 5f);
	public bool isEnemy;
	private SpriteRenderer m_spriteRenderer;
	private Rigidbody2D m_rigidbody;

    public Image healthUI;
    private HeartSystem healthUISystem;
    private TreeBehaviour behaviourBoss;

	void Start() {
		m_spriteRenderer = GetComponent<SpriteRenderer>();
		m_rigidbody = GetComponent<Rigidbody2D>();
        behaviourBoss = GetComponent<TreeBehaviour>();
        if (this.healthUI)
            this.healthUISystem = this.healthUI.GetComponent<HeartSystem>();
	}

    public void TakeDamage(float damage) {
		Debug.Log("Take Damage: " + this.invulnerable);
        if (this.invulnerable == false) {
            if (wasHitClip) SoundManager.instance.PlaySfx(wasHitClip);
            this.invulnerable = true;
            if (Hp - damage <= 0) {
                Hp = 0;
            } else {
                this.Hp -= damage;
            }
            if (this.healthUISystem && !isEnemy){
                this.healthUISystem.attHearts((int)this.Hp);
            }
            if (this.behaviourBoss)
                this.behaviourBoss.UpSpeed();
			StartCoroutine(InvulnerabilityTimer());
		}
	}

	public void Knockback() {
		int multiplyFactor = -1;

		// a escala padrao do inimigo nao esta na mesma direcao que a escala do personagem
		if(isEnemy) {
			multiplyFactor = 1;
		}

		Vector2 knockDirection = new Vector2(multiplyFactor * Mathf.Sign(transform.localScale.x) * knockbackForce.x, knockbackForce.y);

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

    public bool GetInvunerability()
    {
        return this.invulnerable;
    }
}
