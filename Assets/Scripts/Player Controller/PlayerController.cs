using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	
	[Space(5)]
	[Header("General Configuration")]
	public bool hasWallJump = false;
	public bool hasFloat = false;

	[Space(5)]
	[Header("Movement Handling")]
	public float runSpeed = 8f;
	public float groundDamping = 20f; // how fast do we change direction? higher means faster
	
	[Header("Jump Handling")]
	public float goingUpGravity = -25f;
	public float goingDownGravity = -50f;
	public float floatingGravity = -10f;
	private float m_gravity;
	public float inAirDamping = 5f;
	public float jumpHeight = 5f;
	public float jumpPressedRememberTime = 0.15f;
	public float groundedRememberTime = 0.15f;
	public float cutJumpHeight = 0.35f;
	private float m_jumpPressedRemember;
	private float m_groundedRemember;
	private bool m_floating;
	private GameObject m_camTarget;
	
	[Space(5)]
	[Header("Wall Jump Handling")]
	public float onWallGravity = -5f;
	public Vector2 wallJumpVelocity = new Vector2(-5f, 5f);
	private bool m_isOnWall;
	private bool getingOffWall = false;

	[Space(5)]
	[Header("Other Parameters")]
	public float jumpingPlatformMultiplier = 2.5f;
	
	[Space(5)]
	[Header("Particle Effects")]
	public ParticleSystem landingParticles;
	
	[Space(5)]
	[Header("Audio Handling")]
	public AudioClip hurtClip;
	
	[Space(5)]
	[Header("Cam Target")]
	public float movementOffset = 1;
	public float movementSpeedUpwards = 0.1f;
	public float movementSpeedDownwards = 0.4f;
	public Vector2 targetOffset = new Vector2(0f,0.25f);
	private float m_initialY;
	private bool justjumped = false;
	private	bool inSmallJump = true;

	[HideInInspector]
	private float normalizedHorizontalSpeed = 0;

	private Prime31.CharacterController2D m_controller;
	public Transform m_playerSprite;
	private Animator m_animator;
	private RaycastHit2D m_lastControllerColliderHit;
	private Vector3 m_velocity;
	
	// Dialogue Handling
	// se tiver dialogo rolando na tela, bloquear o input do player...
	private bool m_isShowingDialogue;

	// Scale Juicing
	private Vector2 m_originalScale;
	private Vector2 m_goingUpScaleMultiplier = new Vector2(0.8f, 1.2f);
	private Vector2 m_groundingScaleMultiplier = new Vector2(1.2f, 0.8f);

	// OnCollision, OnTrigger, etc... - Have some kind of API (i.e. Interact, ...)
	// easily extendable


	void Awake()
	{
		m_animator = GetComponentInChildren<Animator>();

		m_controller = GetComponent<Prime31.CharacterController2D>();

		// listen to some events for illustration purposes
		m_controller.onControllerCollidedEvent += onControllerCollider;
		m_controller.onTriggerEnterEvent += onTriggerEnterEvent;
		m_controller.onTriggerExitEvent += onTriggerExitEvent;
		
		m_originalScale = m_playerSprite.localScale;
		
		m_gravity = goingUpGravity;

		m_camTarget = GameObject.Find("Camera Target"); 
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

	void onTriggerEnterEvent(Collider2D col) {
		Debug.Log( "onTriggerEnterEvent: " + col.gameObject.name );

		// Interfaces
		IDangerous dangerousInteraction = col.gameObject.GetComponent<IDangerous>();
		IInteractable interaction = col.gameObject.GetComponent<IInteractable>();
		IShowDialogue showDialogue = col.gameObject.GetComponent<IShowDialogue>();

		if(dangerousInteraction != null) {
			if(hurtClip) { SoundManager.instance.PlaySfx(hurtClip); }
			dangerousInteraction.InteractWithPlayer(this.GetComponent<Collider2D>());
		}

		if(interaction != null) {
			interaction.Interact();
		}

		if(showDialogue != null) {
			showDialogue.ShowDialogue();
			m_isShowingDialogue = true;
		}

		if(col.gameObject.layer == LayerMask.NameToLayer("JumpingPlatform")) {
            float rotationAngle = col.gameObject.transform.eulerAngles.z;
            float maxVelocity = Mathf.Sqrt(jumpingPlatformMultiplier * 2f * jumpHeight * -m_gravity);

            m_jumpPressedRemember = 0;
			m_groundedRemember = 0;
			m_gravity = goingUpGravity;

            m_velocity.y = Mathf.Cos(rotationAngle * Mathf.Deg2Rad) * maxVelocity;
            m_velocity.x = -Mathf.Sin(rotationAngle * Mathf.Deg2Rad) * maxVelocity;



            Vector2 deltaPosition = new Vector2(m_velocity.x * Time.deltaTime, (m_velocity.y * Time.deltaTime));
		
			m_controller.move( deltaPosition );

			m_animator.Play( "Jump" );
			StartCoroutine(ChangeScale(m_playerSprite.localScale * m_goingUpScaleMultiplier));
		}
	}


	void onTriggerExitEvent( Collider2D col ) {
		Debug.Log( "onTriggerExitEvent: " + col.gameObject.name );
	}

	#endregion

	void Update()
	{
		if(m_controller.isGrounded) {
			m_groundedRemember = groundedRememberTime;
			m_gravity = goingUpGravity;
			m_velocity.y = 0;

			// became grounded this frame
			if(!m_controller.collisionState.wasGroundedLastFrame) {
				Instantiate(landingParticles, transform.position + (Vector3.down / 4f), Quaternion.identity).Play();
				StartCoroutine(ChangeScale(m_playerSprite.localScale * m_groundingScaleMultiplier));
			}
		}

		if(m_isShowingDialogue) {
			m_animator.Play("Idle");
			m_velocity = Vector2.zero;
			if(!DialogueManager.instance.isShowingDialogue) m_isShowingDialogue = false;
			return;
		}

		Move();
		CamTargetHandling();
		AnimationLogic();
		Jump();
		if(hasFloat) Float();
		if(hasWallJump) WallJump();

		var smoothedMovementFactor = m_controller.isGrounded ? groundDamping : inAirDamping;
		// mudar aqui, nao usar lerp no futuro
		if(!m_isOnWall)m_velocity.x = Mathf.Lerp( m_velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor );

		// velocity verlet for y velocity
		// m_velocity.y += (m_gravity * Time.deltaTime + (.5f * m_gravity * (Time.deltaTime * Time.deltaTime)));
		// limiting gravity
		m_velocity.y = Mathf.Max(m_gravity, m_velocity.y + (m_gravity * Time.deltaTime + (.5f * m_gravity * (Time.deltaTime * Time.deltaTime))));
		
		// ignora as "one way platforms" por um frame (para cair delas)
		if( m_controller.isGrounded && Input.GetKey( KeyCode.DownArrow ) )
		{
			m_velocity.y *= 3f;
			m_controller.ignoreOneWayPlatformsThisFrame = true;
		}

		// applying velocity verlet on delta position for y axis
		// standard euler on x axis
		// heap allocation = bad
		Vector2 deltaPosition = new Vector2(m_velocity.x * Time.deltaTime, (m_velocity.y * Time.deltaTime));
		
		m_controller.move( deltaPosition );
		m_velocity = m_controller.velocity;
	}

	private IEnumerator ChangeScale(Vector2 scale) {
		m_playerSprite.localScale = scale;
		yield return new WaitForSeconds(0.075f);
		m_playerSprite.localScale = new Vector3(Mathf.Sign(m_playerSprite.localScale.x) * Mathf.Abs(m_originalScale.x), m_originalScale.y, m_playerSprite.localScale.z);
	}

	private void AnimationLogic() {
		if(gameObject.activeSelf){
			if(m_isOnWall) {
				m_animator.Play("Wall");
			} else if(Mathf.Abs(m_velocity.y) > Mathf.Epsilon) {
				if(m_floating) {
					m_animator.Play("Floating");
				} else {
					m_animator.Play("Jump");
				}
			} else if(Mathf.Abs(normalizedHorizontalSpeed) > 0 && m_controller.isGrounded) {
				m_animator.Play("Running");
			} else {
				m_animator.Play("Idle");
			}
		}
	}

	private void Move() {
		
		float horizontalMovement = Input.GetAxisRaw("Horizontal");
		normalizedHorizontalSpeed = horizontalMovement;
		
		if(horizontalMovement != 0) {
			if(!m_isOnWall) m_playerSprite.localScale = new Vector3(Mathf.Sign(horizontalMovement) * Mathf.Abs(m_playerSprite.localScale.x), m_playerSprite.localScale.y, m_playerSprite.localScale.z);
		}
		if(m_isOnWall){
			if(m_controller.isColliding(Vector2.right)){
					m_playerSprite.localScale = new Vector3(Mathf.Abs(m_playerSprite.localScale.x), m_playerSprite.localScale.y, m_playerSprite.localScale.z);
			} else{
					m_playerSprite.localScale = new Vector3(-1 * Mathf.Abs(m_playerSprite.localScale.x), m_playerSprite.localScale.y, m_playerSprite.localScale.z);
			}
		}
	}

	private void Jump() {
		m_groundedRemember -= Time.deltaTime;
		m_jumpPressedRemember -= Time.deltaTime;

		if(Input.GetButtonDown("Jump")) {
			m_jumpPressedRemember = jumpPressedRememberTime;
		}

		if(Input.GetButtonUp("Jump")) {
			if(m_velocity.y > 0) {
				m_velocity.y = m_velocity.y * cutJumpHeight;
			}
		}

		if(m_velocity.y < 0 && !m_isOnWall) {
			m_gravity = goingDownGravity;
		}

		// REGULAR JUMP
		if( ( (m_groundedRemember > 0) && (m_jumpPressedRemember > 0) ) ) {
			m_jumpPressedRemember = 0;
			m_groundedRemember = 0;

			m_velocity.y = Mathf.Sqrt( 2f * jumpHeight * -m_gravity );
			
			StartCoroutine(ChangeScale(m_playerSprite.localScale * m_goingUpScaleMultiplier));
			m_animator.Play( "Jump" );
		}
	}

	private void Float() {
		if(m_velocity.y >= 0) return;	
		if(Input.GetButton("Jump")) {
			m_floating = true;
			m_gravity = floatingGravity;
		} else if(!m_isOnWall) {
			m_floating = false;
			m_gravity = goingDownGravity;
		}
	}

	private void WallJump() {
		if(m_controller.isGrounded) {
			m_isOnWall = false;
			return;
		}
		
		// Stick to Wall
		if(!m_isOnWall && ( (m_controller.isColliding(Vector2.left) && Input.GetAxisRaw("Horizontal") != 1) ||
							(m_controller.isColliding(Vector2.right)&& Input.GetAxisRaw("Horizontal") != -1))) {
				// wasn't on wall last frame
				if(!m_isOnWall) StartCoroutine(ChangeScale(m_goingUpScaleMultiplier));
				m_isOnWall = true;
				StopCoroutine(letGoOfWall());
				getingOffWall  = false;
			} else if(((m_controller.isColliding(Vector2.left) && (Input.GetAxisRaw("Horizontal") == 1)) ||
					   (m_controller.isColliding(Vector2.right) && (Input.GetAxisRaw("Horizontal") == -1))) && m_isOnWall){
				if(!getingOffWall){
					StartCoroutine(letGoOfWall());
					getingOffWall  = true;
				}
			} else if(m_controller.isColliding(Vector2.left) || m_controller.isColliding(Vector2.right)){
				StopCoroutine(letGoOfWall());
				getingOffWall = false;
			} else{
				StopCoroutine(letGoOfWall());
				getingOffWall = false;
				m_isOnWall = false;
			}
		if(m_isOnWall && m_velocity.y <= 0) m_gravity = onWallGravity;

		// Wall Jump
		if(m_isOnWall && Input.GetButtonDown("Jump")) {
			StopCoroutine(letGoOfWall());
			m_isOnWall = false;
			int jumpDirection =  m_controller.isColliding(Vector2.left) ? -1:1;
			m_velocity.x = wallJumpVelocity.x * jumpDirection;
			m_gravity = goingUpGravity;
			m_velocity.y = Mathf.Sqrt(2f * wallJumpVelocity.y * -m_gravity);
			StartCoroutine(ChangeScale(m_goingUpScaleMultiplier));
		}
	}

	private void CamTargetHandling(){
		if(Mathf.Abs(m_velocity.y) <= 0.01f){
			m_camTarget.transform.position = transform.position + (Vector3)targetOffset;
			m_initialY = m_camTarget.transform.position.y;
			if(!justjumped) {
				inSmallJump = true;
			}
		} else {
			if(m_velocity.y > 0 && m_camTarget.transform.position.y <= transform.position.y + targetOffset.y + movementOffset && m_camTarget.transform.position.y > m_initialY + 1f) {
				m_camTarget.transform.position += Vector3.up*movementSpeedUpwards;
				inSmallJump = false;
				justjumped = true;
			}
			if(m_camTarget.transform.position.y < m_initialY - 0.01f) {
				inSmallJump = false;
			}
			if(m_velocity.y < 0 && m_camTarget.transform.position.y >= transform.position.y + targetOffset.y - movementOffset && !inSmallJump) {
				m_camTarget.transform.position += Vector3.down*movementSpeedDownwards;
				justjumped = false;
			}
		}
	}

	private IEnumerator letGoOfWall(){
		yield return new WaitForSeconds(0.5f);
		m_velocity.x = (wallJumpVelocity.x / 2f) * (m_controller.isColliding(Vector2.left) ? -1:1);
		m_isOnWall = false;
	}

	public void StopMovement(){
		m_velocity = new Vector3(0f,0f,0f);
	}

	public Vector3 GetVelocity(){
		return m_velocity;
	}
}

