using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossArtes : MonoBehaviour, IDangerous {

	public enum EDaliBossStates {
		// esse estado o boss só fica acompanhando o player no canto da tela
		dummy,
		// esse estado o boss acompnha e dá um dash a cada x segundos
		timedDash,
		// esse estado o boss segue o player com um certo delay	
		followingPlayer,
		// esse estado o boss fica acompanhando o player até que algum objeto na cena sinalize para ele dar um dash
		waitForDash,

		// esse estado o boss trava no eixo Y antes de dar o dash
		lockedYAxis,
		// esse estado o boss esta no meio do dash
		dashing,
		// esse estado o boss ja deu o dash e esta esperando pra poder voltar a tela
		recovering,

	};

	
	[Space(5)]
	[Header("Boss Configuration")]
	public float minDashTime = 4f;
	public float maxDashTime = 6f;
	public float dashVelocity = 2f;
	public float flashingTime = 0.5f;
	public float waitBeforeDash = 1.5f;
	public float waitTimeAfterDash = 2f;
	public GameObject playerReference;

	[Space(5)]
	[Header("Boss Track Configuration")]
	public float xAxisOffset = 2.4f;
	public float lerpVelocity = 5f;

	private EDaliBossStates m_bossState;
	private float m_distanceToPlayer;
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

		Debug.Log("Main Camera pixel width: " + Camera.main.pixelWidth);
		Debug.Log("Main Camera scaled pixel width: " + Camera.main.scaledPixelWidth);
		Debug.Log("?: " + Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0f)));

		m_bossState = EDaliBossStates.timedDash;
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

	private IEnumerator FlashBoss() {
		float timeElapsed = 0f;
		int t_counter = 0;

		while(timeElapsed < flashingTime) {
			if(t_counter % 2 == 0) {
				m_spriteRenderer.color = Color.red;
			} else {
				m_spriteRenderer.color = Color.white;
			}
			t_counter++;

			timeElapsed += Time.deltaTime;
			yield return null;
		}

		m_spriteRenderer.color = Color.white;
	}

	private IEnumerator Dash() {
		yield return null;

		while(transform.position.x < (playerReference.transform.position.x + (5*m_distanceToPlayer))) {
			transform.position = new Vector3(transform.position.x + dashVelocity, transform.position.y, transform.position.z);
			yield return null;
		}

		m_animator.Play("Idle");
		transform.position = Camera.main.ScreenToWorldPoint(new Vector3(25f, -100f, 10f));
		m_bossState = EDaliBossStates.recovering;
		yield return new WaitForSeconds(waitTimeAfterDash);
		m_timeToNextDash = Time.time + Random.Range(minDashTime, maxDashTime);
		m_bossState = EDaliBossStates.timedDash;
	}

	private void TrackPlayer(bool trackYAxis = true) {
		float sinModifier = Mathf.Abs((Mathf.Sin(Time.time / 3f * Mathf.PI)));
		Vector3 bossPosition = Vector3.zero;

		if(trackYAxis) {
			bossPosition = new Vector3(Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0f)).x + xAxisOffset, playerReference.transform.position.y + sinModifier, transform.position.z);
		} else {
			bossPosition = new Vector3(Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0f)).x + xAxisOffset, transform.position.y, transform.position.z);
		}

		transform.position = Vector3.Lerp(transform.position, bossPosition, Time.deltaTime * lerpVelocity);
	}
	void Update () {

		switch(m_bossState) {
			case EDaliBossStates.dummy:
				TrackPlayer();
			break;
			case EDaliBossStates.timedDash:
				TrackPlayer();

				if(Time.time >= m_timeToNextDash) {
					m_animator.Play("Attack");
					StartCoroutine(FlashBoss());
					m_bossState = EDaliBossStates.lockedYAxis;
					m_timeToNextDash = Time.time + waitBeforeDash;
				}
			break;
			case EDaliBossStates.lockedYAxis:
				TrackPlayer(false);

				if(Time.time >= m_timeToNextDash) {
					StartCoroutine(Dash());
					m_bossState = EDaliBossStates.dashing;
				}
			break;
			case EDaliBossStates.dashing:
			break;
			case EDaliBossStates.recovering:
				TrackPlayer(false);
			break;
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
