using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	[Header("Movement Handling")]
	public float runSpeed = 8f;
	public float groundDamping = 20f; // how fast do we change direction? higher means faster
	
	[Header("Jump Handling")]
	public float gravity = -25f;
	public float inAirDamping = 5f;
	public float jumpHeight = 3f;
	public float jumpPressedRememberTime = 0.15f;
	public float groundedRememberTime = 0.15f;
	public float cutJumpHeight = 0.35f;
	private float m_jumpPressedRemember;
	private float m_groundedRemember;
	[Header("Audio Handling")]
	public AudioClip hurtClip;


	[HideInInspector]
	private float normalizedHorizontalSpeed = 0;

	private Prime31.CharacterController2D m_controller;
	private Animator m_animator;
	private RaycastHit2D m_lastControllerColliderHit;
	private Vector3 m_velocity;


	void Awake()
	{
		m_animator = GetComponent<Animator>();
		m_controller = GetComponent<Prime31.CharacterController2D>();

		// listen to some events for illustration purposes
		m_controller.onControllerCollidedEvent += onControllerCollider;

		if(GameObject.Find("DaliLevelManager")) {
			Debug.Log("Esta na fight do Dali!");
			m_controller.onTriggerEnterEvent += DaliBossTriggerEnterEvent;
		} else {
			m_controller.onTriggerEnterEvent += onTriggerEnterEvent;
		}

		m_controller.onTriggerExitEvent += onTriggerExitEvent;
	}


	#region Event Listeners

	void onControllerCollider( RaycastHit2D hit )
	{
		// bail out on plain old ground hits cause they arent very interesting
		if( hit.normal.y == 1f )
			return;

		// logs any collider hits if uncommented. it gets noisy so it is commented out for the demo
		Debug.Log( "flags: " + m_controller.collisionState + ", hit.normal: " + hit.normal );
	}

	void DaliBossTriggerEnterEvent(Collider2D col) {
		DaliLevelManager daliLevel = FindObjectOfType<DaliLevelManager>();

		if(col.gameObject.layer == LayerMask.NameToLayer("BossLayer") ||
		col.gameObject.layer == LayerMask.NameToLayer("Hazard")) {
			if(hurtClip) SoundManager.instance.PlaySfx(hurtClip);
			daliLevel.ResetPlayer();
		} else if(col.gameObject.layer == LayerMask.NameToLayer("DaliCheckpoint")) {
			daliLevel.PassedCheckpoint(transform.position);
		} else if(col.gameObject.layer == LayerMask.NameToLayer("DaliFinalCheckpoint")) {
			daliLevel.PassedCheckpoint(transform.position);
			daliLevel.EndOfLevel();
		} else if(col.gameObject.layer == LayerMask.NameToLayer("JumpingPlatform")) {
			m_velocity.y = Mathf.Sqrt( 5f * jumpHeight * -gravity );
			m_animator.Play( "Jump" );
		}
	}


	void onTriggerEnterEvent( Collider2D col ) {
		Debug.Log( "onTriggerEnterEvent: " + col.gameObject.name );
	}


	void onTriggerExitEvent( Collider2D col )
	{
		Debug.Log( "onTriggerExitEvent: " + col.gameObject.name );
	}

	#endregion

	void Update()
	{
		if(m_controller.isGrounded) {
			m_groundedRemember = groundedRememberTime;
			m_velocity.y = 0;
		}

		Move();
		AnimationLogic();
		Jump();

		var smoothedMovementFactor = m_controller.isGrounded ? groundDamping : inAirDamping;
		// mudar aqui, usar lerp no futuro
		m_velocity.x = Mathf.Lerp( m_velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor );

		m_velocity.y += gravity * Time.deltaTime;

		
		// ignora as "one way platforms" por um frame (para cair delas)
		if( m_controller.isGrounded && Input.GetKey( KeyCode.DownArrow ) )
		{
			m_velocity.y *= 3f;
			m_controller.ignoreOneWayPlatformsThisFrame = true;
		}

		m_controller.move( m_velocity * Time.deltaTime );
		m_velocity = m_controller.velocity;
	}

	private void AnimationLogic() {
		if(Mathf.Abs(m_velocity.y) > Mathf.Epsilon) {
			m_animator.Play("Jump");
		} else if(Mathf.Abs(normalizedHorizontalSpeed) > 0 && m_controller.isGrounded) {
			m_animator.Play("Running");
		} else {
			m_animator.Play("Idle");
		}
	}

	private void Move() {
		if( Input.GetKey( KeyCode.RightArrow ) ) {
			normalizedHorizontalSpeed = 1;
			if( transform.localScale.x < 0f ) {
				transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );
			}
		}
		else if( Input.GetKey( KeyCode.LeftArrow ) ) {
			normalizedHorizontalSpeed = -1;
			if( transform.localScale.x > 0f ) {
				transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );
			}
		} else {
			normalizedHorizontalSpeed = 0;
		}
	}

	private void Jump() {
		m_groundedRemember -= Time.deltaTime;
		m_jumpPressedRemember -= Time.deltaTime;

		if(Input.GetKeyDown(KeyCode.UpArrow)) {
			m_jumpPressedRemember = jumpPressedRememberTime;
		}

		if(Input.GetKeyUp(KeyCode.UpArrow)) {
			if(m_velocity.y > 0) {
				m_velocity.y += cutJumpHeight;
			}
		}

		// WALL JUMP
		if( ((m_groundedRemember > 0) || ((m_controller.collisionState.right || m_controller.collisionState.left) && !m_controller.isGrounded)) && (m_jumpPressedRemember > 0) ) {
			m_jumpPressedRemember = 0;
			m_groundedRemember = 0;

			m_velocity.y = Mathf.Sqrt( 2f * jumpHeight * -gravity );
			m_animator.Play( "Jump" );
		}
	}
}

