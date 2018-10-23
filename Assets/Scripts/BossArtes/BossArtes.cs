using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossArtes : MonoBehaviour, IDangerous {

	private float m_distanceToPlayer;
	public float minDashTime = 4f;
	public float maxDashTime = 6f;
	public float dashVelocity = 2f;
	public float waitBeforeDash = 1.5f;
	public float waitTimeAfterDash = 2f;
	public GameObject playerReference;

	private float m_timeToNextDash;
	private float m_dashDistance;
	private float offscreenY = -11.5f;
	private bool m_isDashing;
	private bool m_isLocked;
	private Vector3 m_lockPosition;
	private SpriteRenderer m_spriteRenderer;
	private Animator m_animator;
	
	[HideInInspector]
	public bool canDash = true;
	
	void Start() {
		m_spriteRenderer = GetComponent<SpriteRenderer>();
		m_animator = GetComponent<Animator>();

		ResetBoss();
	}

	public void ResetBoss() {
		m_distanceToPlayer = Mathf.Abs(transform.position.x - playerReference.transform.position.x);
		m_timeToNextDash = Random.Range(minDashTime, maxDashTime);
		m_isDashing = false;
		m_isLocked = false;
	}

	public void StopBoss() {
		StopAllCoroutines();
		canDash = false;
		m_isDashing = false;
		m_isLocked = false;
	}

	public IEnumerator ReturnToScreen() {
		m_lockPosition.y += offscreenY;
		while(m_lockPosition.y < 0) {
			m_lockPosition.y += (dashVelocity / 2);
			yield return null;
		}

		m_isLocked = false;
		m_isDashing = false;
	}

	private IEnumerator Dash() {
		m_isLocked = true;
		m_lockPosition = transform.position;
		yield return new WaitForSeconds(waitBeforeDash);
		
		m_dashDistance = 0;
		while(transform.position.x < (playerReference.transform.position.x + (2*m_distanceToPlayer))) {
			m_dashDistance += dashVelocity;
			yield return null;
		}

		yield return new WaitForSeconds(waitTimeAfterDash);
		m_dashDistance = 0;
		m_animator.Play("Idle");
		m_timeToNextDash = Time.time + Random.Range(minDashTime, maxDashTime);
		StartCoroutine(ReturnToScreen());
	}
	void Update () {

		float sinModifier = Mathf.Abs((Mathf.Sin(Time.time / 5f * Mathf.PI)) * 2f);

		if(!m_isLocked) {
			this.transform.position = new Vector3(playerReference.transform.position.x - m_distanceToPlayer, playerReference.transform.position.y + sinModifier, transform.position.z);
		} else {
			this.transform.position = new Vector3(playerReference.transform.position.x - m_distanceToPlayer + m_dashDistance,m_lockPosition.y, m_lockPosition.z);
		}

		if(Time.time >= m_timeToNextDash && !m_isDashing && canDash) {
			m_isDashing = true;
			m_animator.Play("Attack");
			StartCoroutine(Dash());
		}
	}

	void IDangerous.InteractWithPlayer(Collider2D player) {
		DaliLevelManager.instance.ResetPlayer();
	}

	void OnTriggerEnter2D(Collider2D other) {
		if(other.gameObject.layer == LayerMask.NameToLayer("Hazard")) {
			Debug.Log("Collided with Hazard, boss should die");
		}
	}

}
