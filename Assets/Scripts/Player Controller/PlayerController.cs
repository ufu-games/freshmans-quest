using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;


/*
	Player States:
	Idle,
	Moving,
	Jumping,
	OnWall,
	WallJumping (?),
	Cannon,
	CannotMove, (Dialogue)
*/
public class PlayerController : MonoBehaviour {
	
	[Space(5)]
	[Header("General Configuration")]
	public bool hasWallJump = false;

	[Space(5)]
	[Header("Collectables")]
	public float PizzaCollected = 0;
	public float HomeworkCollected = 0;

	[Space(5)]
	[Header("Movement Handling")]
	public float runSpeed = 8f;
	public float groundDamping = 20f; // how fast do we change direction? higher means faster
	public float slippingFrictionMultiplier = .1f;
	
	[Header("Jump Handling")]
	public float goingUpGravity = -25f;
	public float goingDownGravity = -50f;
	public float floatingGravity = -10f;
	[ReadOnly]
	public float m_gravity;
	public float inAirDamping = 5f;
	public float jumpHeight = 5f;
	public float jumpPressedRememberTime = 0.15f;
	public float groundedRememberTime = 0.15f;
	public float cutJumpHeight = 0.35f;
	public float leftGoOffWalDelay = 2f;
	private float m_jumpPressedRemember;
	private float m_groundedRemember;
	private bool m_floating;
	
	[Space(5)]
	[Header("Wall Jump Handling")]
	public float onWallGravity = -5f;
	//public Vector2 wallJumpVelocity = new Vector2(-5f, 5f);
	public Vector2 wallJumpVelocity;
	public float maxDistanceOffWall;
	[ReadOnly]
	public bool m_isOnWall;
	private bool getingOffWall = false;
	private bool m_skipMoveOnUpdateThisFrame = false;
	private bool m_startedslidingwall = false;
	private bool m_fastsliding = false;

	[Space(5)]
	[Header("Other Parameters")]
	public float jumpingPlatformMultiplier = 2.5f;
	public GameObject dialogueHintObject;
	
	[Space(5)]
	[Header("Particle Effects")]
	public ParticleSystem landingParticles;
	
	[Space(5)]
	[Header("Audio Handling")]
	public AudioClip hurtClip;
	public AudioClip[] stepClips;
	public AudioClip jumpingClip;
	
	//Camera Handling
	private CinemachineFramingTransposer m_cam;

	[HideInInspector]
	private float normalizedHorizontalSpeed = 0;

	private Prime31.CharacterController2D m_controller;
	public Transform[] m_playerSprites;
	private Animator[] m_animators;
	private RaycastHit2D m_lastControllerColliderHit;
	private Vector3 m_velocity;
	
	// Dialogue Handling
	// se tiver dialogo rolando na tela, bloquear o input do player...
	private bool m_isShowingDialogue;

	// Scale Juicing
	private Vector2 m_originalScale;
	private Vector2 m_goingUpScaleMultiplier = new Vector2(0.8f, 1.2f);
	private Vector2 m_groundingScaleMultiplier = new Vector2(1.2f, 0.8f);

	// easily extendable
	private bool isInCannon = false;
	private CannonBehaviour Cannon;
	private bool m_isSlipping = false;

	//Breakable Wall Handling
	[ReadOnly]
	public Vector3 m_velocityLastFrame;

	void Awake()
	{
		m_animators = GetComponentsInChildren<Animator>();
		m_playerSprites = new Transform[m_animators.Length];
		for(int i=0;i<m_animators.Length;i++) {
			m_playerSprites[i] = m_animators[i].transform;
		}

		m_controller = GetComponent<Prime31.CharacterController2D>();
		m_controller.onControllerCollidedEvent += onControllerCollider;
		m_controller.onTriggerEnterEvent += onTriggerEnterEvent;
		m_controller.onTriggerStayEvent += OnTriggerStayEvent;
		m_controller.onTriggerExitEvent += onTriggerExitEvent;
		
		m_originalScale = m_playerSprites[0].localScale;
		m_playerSprites[1].localScale = m_originalScale;
		
		m_gravity = goingUpGravity;

        if(Camera.main.GetComponentInChildren<CinemachineVirtualCamera>()) {
			m_cam = Camera.main.GetComponentInChildren<CinemachineFramingTransposer>();
        } else {
			Debug.LogWarning("Não há Cinemachine presente na cena! A Cãmera não seguirá o personagem.");
		}  

		dialogueHintObject.SetActive(false);
		UpdatePizzaCounter();
    }


    #region Event Listeners

    void onControllerCollider( RaycastHit2D hit )
	{
		m_isSlipping = hit.transform.gameObject.tag == "Slippery" && !(m_controller.isColliding(Vector2.left) || m_controller.isColliding(Vector2.right));

		IDangerous dangerousInteraction = hit.collider.gameObject.GetComponent<IDangerous>();

		if(hit.collider.tag == "BreakableWall") {
			hit.collider.gameObject.GetComponent<BreakableWallBehavior>().Collision(hit.point);
		}

		if(dangerousInteraction != null) {
			dangerousInteraction.InteractWithPlayer(this.GetComponent<Collider2D>());
		}
		if(hit.collider.tag == "MovingPlataform"){
			Debug.Log(hit.normal.x);
			var plataformDirection = hit.collider.gameObject.GetComponent<MovingPlataform>().getAngle();
			var plataformSpeed = hit.collider.gameObject.GetComponent<MovingPlataform>().speed;
			if(hit.normal.y > 0){
				var newPosition = transform.position; 

       			newPosition.x += Mathf.Cos(plataformDirection) * plataformSpeed;
        		newPosition.y += Mathf.Sin(plataformDirection) * plataformSpeed;
        		transform.position = newPosition;
			}
			if(hit.normal.x < 0 && transform.position.x < hit.collider.gameObject.transform.position.x){
				var newPosition = transform.position; 

       			newPosition.x += Mathf.Cos(plataformDirection) * plataformSpeed;
        		transform.position = newPosition;
			}
		}

		/* TO DO: Dropping Platform */
		/* TO DO: Moving Platform */
		
		// bail out on plain old ground hits cause they arent very interesting
		if( hit.normal.y == 1f )
			return;
		// logs any collider hits if uncommented. it gets noisy so it is commented out for the demo
		// Debug.LogWarning( "flags: " + m_controller.collisionState + ", hit.normal: " + hit.normal );	
	}

	void OnTriggerStayEvent(Collider2D col) {
		IShowDialogue showDialogue = col.gameObject.GetComponent<IShowDialogue>();

		if(showDialogue != null && !m_isShowingDialogue) {
			dialogueHintObject.SetActive(true);

			if(InputManager.instance.PressedConfirm()) {
				showDialogue.ShowDialogue();
				m_isShowingDialogue = true;
			}
		} else {
			dialogueHintObject.SetActive(false);
		}
	}

	void onTriggerEnterEvent(Collider2D col) {
		// Debug.LogWarning( "onTriggerEnterEvent: " + col.gameObject.name );

		// Interfaces
		IDangerous dangerousInteraction = col.gameObject.GetComponent<IDangerous>();
		IInteractable interaction = col.gameObject.GetComponent<IInteractable>();
		INonHarmfulInteraction nonHarmfulInteraction = col.gameObject.GetComponent<INonHarmfulInteraction>();

		if(dangerousInteraction != null) {
			if(hurtClip && SoundManager.instance) { SoundManager.instance.PlaySfx(hurtClip); }
			dangerousInteraction.InteractWithPlayer(this.GetComponent<Collider2D>());
		}

		if(interaction != null) {
			interaction.Interact();
		}

		if(nonHarmfulInteraction != null) {
			nonHarmfulInteraction.InteractWithPlayer(this.GetComponent<Collider2D>());
		}

		if(col.gameObject.layer == LayerMask.NameToLayer("JumpingPlatform")) {
            float rotationAngle = col.gameObject.transform.eulerAngles.z;
            float maxVelocity = Mathf.Sqrt(jumpingPlatformMultiplier * 2f * jumpHeight * -m_gravity);
			this.getsThrownTo(rotationAngle, maxVelocity);
			m_skipMoveOnUpdateThisFrame = true;
		}

		if(col.gameObject.layer == LayerMask.NameToLayer("Cannon")){
			//Debug.Log("entrou no canhao");
			isInCannon = true;
			this.transform.position = col.gameObject.transform.position;
			m_velocity = Vector2.zero;
			foreach(Animator ani in m_animators) {
				ani.gameObject.GetComponent<SpriteRenderer>().enabled = false;
			}
			this.Cannon = col.gameObject.GetComponent<CannonBehaviour>();
			Camera.main.GetComponentInChildren<CinemachineVirtualCamera>().m_Lens.OrthographicSize *= this.Cannon.zoomOutMultiplier;
			this.Cannon.setActive(true);
		}
	}

	public void getsThrownTo(float rotationAngle, float maxVelocity){
		m_jumpPressedRemember = 0;
		m_groundedRemember = 0;
		m_gravity = goingUpGravity;

		m_velocity.y = Mathf.Cos(rotationAngle * Mathf.Deg2Rad) * maxVelocity;
		m_velocity.x = -Mathf.Sin(rotationAngle * Mathf.Deg2Rad) * maxVelocity;

		Vector2 deltaPosition = new Vector2(m_velocity.x * Time.deltaTime,m_velocity.y*Time.deltaTime);
	
		m_controller.move( deltaPosition );

		foreach(Animator ani in m_animators) {
			ani.Play( "Jump" );
		}
		foreach(Transform transf in m_playerSprites) {
			StartCoroutine(ChangeScale(transf.localScale * m_goingUpScaleMultiplier));
			break;
		}

		if(isInCannon)
			Camera.main.GetComponentInChildren<CinemachineVirtualCamera>().m_Lens.OrthographicSize /= this.Cannon.zoomOutMultiplier;
	}


	void onTriggerExitEvent( Collider2D col ) {
		if(!this.enabled) return;

		INonHarmfulInteraction nonHarmfulInteraction = col.gameObject.GetComponent<INonHarmfulInteraction>();
		IInteractableLeaveTrigger interactWhenLeft = col.gameObject.GetComponent<IInteractableLeaveTrigger>();

		if(nonHarmfulInteraction != null) {
			nonHarmfulInteraction.InteractWithPlayer(this.GetComponent<Collider2D>());
		}

		if(interactWhenLeft != null) {
			interactWhenLeft.Interact();
		}
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
				if(SoundManager.instance && stepClips.Length > 0) {
					SoundManager.instance.PlaySfx(stepClips[Random.Range(0, stepClips.Length)]);
				}
				foreach(Transform transf in m_playerSprites) {
					StartCoroutine(ChangeScale(transf.localScale * m_groundingScaleMultiplier));
					break;
				}
			}
		}

		if(m_isShowingDialogue) {
			foreach(Animator ani in m_animators) {
				ani.Play("Idle");
			}
			m_velocity = Vector2.zero;
			m_velocity.y = goingDownGravity;
			m_controller.move(m_velocity * Time.deltaTime);

			if(!DialogueManager.instance.isShowingDialogue) m_isShowingDialogue = false;
			return;
		}

		m_velocityLastFrame = m_velocity;

		Move();
		CamHandling();
		AnimationLogic();
		if(!isInCannon && !m_isOnWall) Jump();
		if(hasWallJump) WallJump();
		
		if(isInCannon && InputManager.instance.PressedJump()){
			var angleCannon = Cannon.getAngle();
			float distanceBetweenCenterAndExplosion = 0;
			getsThrownTo(angleCannon, Cannon.getThrowMultiplier());
			GameObject part = (GameObject) Instantiate(Resources.Load("BeckerCannonParticle"));
			float partangle = angleCannon+90;
			part.transform.rotation = Quaternion.Euler(-partangle,90,0);
			part.transform.position = new Vector3(Cannon.transform.position.x + distanceBetweenCenterAndExplosion*Mathf.Cos(Mathf.Deg2Rad*(angleCannon+90)), Cannon.transform.position.y + distanceBetweenCenterAndExplosion*Mathf.Sin(Mathf.Deg2Rad*(angleCannon+90)),0);
			part.GetComponent<ParticleSystem>().Play();
			foreach(Animator ani in m_animators) {
				ani.gameObject.GetComponent<SpriteRenderer>().enabled = true;
			}
			isInCannon = false;
			Cannon.setActive(false);
		}

		float t_groundDamping = m_isSlipping ? (groundDamping * slippingFrictionMultiplier) : groundDamping;
		var smoothedMovementFactor = m_controller.isGrounded ? t_groundDamping : inAirDamping;

		//if(!m_isOnWall) {
			m_velocity.x = Mathf.Lerp(normalizedHorizontalSpeed * runSpeed, m_velocity.x,Mathf.Pow(1 - smoothedMovementFactor, Time.deltaTime*60));
		//}
		
		// limiting gravity
		m_velocity.y = Mathf.Max(m_gravity, m_velocity.y + (m_gravity * Time.deltaTime + (.5f * m_gravity * (Time.deltaTime * Time.deltaTime))));
		
		// ignora as "one way platforms" por um frame (para cair delas)
		// ISSO AQUI DÁ UM BUG
		// SE VC SEGURAR PRA BAIXO E PULAR, O PERSONAGEM DÁ UM PULÃO
		// A PARTIR DE AGORA ISSO É UMA FEATURE
		if( m_controller.isGrounded && Input.GetKey( KeyCode.DownArrow ) )
		{
			m_velocity.y *= Mathf.Pow(3f,Time.deltaTime);
			m_controller.ignoreOneWayPlatformsThisFrame = true;
		}

		// applying velocity verlet on delta position for y axis
		// standard euler on x axis
		// heap allocation = bad
		Vector2 deltaPosition = new Vector2(m_velocity.x * Time.deltaTime,m_velocity.y * Time.deltaTime);
		
		if(!isInCannon && !m_skipMoveOnUpdateThisFrame) {
			m_controller.move( deltaPosition );
			m_velocity = m_controller.velocity;
		}

		if(m_controller.isGrounded) {
			m_startedslidingwall = false;
			m_fastsliding = false;
		}

		m_skipMoveOnUpdateThisFrame = false;
	}

	private IEnumerator ChangeScale(Vector2 scale) {
		foreach(Transform transf in m_playerSprites) {
			transf.localScale = scale;
		}
		yield return new WaitForSeconds(0.075f);
		foreach(Transform transf in m_playerSprites) {
			transf.localScale = new Vector3(Mathf.Sign(transf.localScale.x) * Mathf.Abs(m_originalScale.x), m_originalScale.y, transf.localScale.z);
		}
	}
	
	private void AnimationLogic() {
		if(gameObject.activeSelf){
			foreach(Animator ani in m_animators) {
				if(m_isOnWall) {
					ani.Play("Wall");
				} else if(Mathf.Abs(m_velocity.y) > Mathf.Epsilon) {
					if(m_velocity.y > 0) {
						ani.Play("Jump");
					} else {
						ani.Play("Falling");
					}
				} else if(Mathf.Abs(normalizedHorizontalSpeed) > 0 && m_controller.isGrounded) {
					ani.Play("Running");
				} else {
					ani.Play("Idle");
				}
			}
		}
	}

	private void Move() {
		float horizontalMovement = Input.GetAxisRaw("Horizontal");
		normalizedHorizontalSpeed = horizontalMovement;
		
		if(horizontalMovement != 0) {
			if(!m_isOnWall) 
			foreach(Transform transf in m_playerSprites) {
				transf.localScale = new Vector3(Mathf.Sign(horizontalMovement) * Mathf.Abs(transf.localScale.x), transf.localScale.y, transf.localScale.z);
			} 
		}

		if(m_isOnWall) {
			if(m_controller.isColliding(Vector2.right)){
					foreach(Transform transf in m_playerSprites) {
						transf.localScale = new Vector3(Mathf.Abs(transf.localScale.x), m_originalScale.y, transf.localScale.z);
					}
			} else{
					foreach(Transform transf in m_playerSprites) {
						transf.localScale = new Vector3(Mathf.Abs(transf.localScale.x)* -1, m_originalScale.y, transf.localScale.z);
					}
			}
		}
	}

	private void Jump() {
		m_groundedRemember -= Time.deltaTime;
		m_jumpPressedRemember -= Time.deltaTime;

		if(InputManager.instance.PressedJump()) {
			m_jumpPressedRemember = jumpPressedRememberTime;
		}

		if(InputManager.instance.ReleasedJump()) {
			if(m_velocity.y > 0) {
				m_velocity.y = m_velocity.y * Mathf.Pow(cutJumpHeight,Time.deltaTime*60);
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
			
			if(SoundManager.instance && jumpingClip) {
				SoundManager.instance.PlaySfx(jumpingClip);
			}
			foreach(Transform transf in m_playerSprites) {
				StartCoroutine(ChangeScale(transf.localScale * m_goingUpScaleMultiplier));
				break;
			}
			foreach(Animator ani in m_animators) {
				ani.Play( "Jump" );
			}
		}
	}

	private void WallJump() {
		
		//Debug.Log("right " + m_controller.isNear(Vector2.right,maxDistanceOffWall));

		if(m_controller.isGrounded) {
			m_isOnWall = false;
			return;
		}
		
		// Stick To Wall
		// is not on wall AND
		// is pressing the trigger AND
		// is colliding on left or right
		if(!m_isOnWall
			&&
			!m_controller.isGrounded
			//&& 
			// Input.GetButton("StickToWall")
			//InputManager.instance.PressedWallJump()
			&&
			((m_controller.isColliding(Vector2.left) &&  m_velocity.x < 0)
			|| 
			(m_controller.isColliding(Vector2.right) &&  m_velocity.x > 0)))
		{
			// wasn't on wall last frame
			
			m_isOnWall = true;
		} else if(
			// is on wall AND
			// is not pressing AND
			// is colliding
			m_isOnWall
			&&
			!m_controller.isNear(Vector2.left,maxDistanceOffWall) && !m_controller.isNear(Vector2.right,maxDistanceOffWall)
			//&&
			//!InputManager.instance.PressedWallJump()
			//&&
			//(m_controller.isColliding(Vector2.left) || m_controller.isColliding(Vector2.right))
			)
		{
			m_isOnWall = false;

		}

		// EFFECTIVELY JUMPING OFF WALL
		// if is on wall AND
		// is pressing the jump button

		if((m_isOnWall 
			&& 
			// Input.GetButtonDown("Jump")
			InputManager.instance.PressedJump())

			||

			(!m_isOnWall
			&&
			InputManager.instance.PressedJump()
			&&
			((m_controller.isNear(Vector2.left,maxDistanceOffWall) || m_controller.isNear(Vector2.right,maxDistanceOffWall))))
			) {
			if(!m_isOnWall) {
				if(m_controller.isColliding(Vector2.right)){
					foreach(Transform transf in m_playerSprites) {
						transf.localScale = new Vector3(Mathf.Abs(transf.localScale.x)*-1, m_originalScale.y, transf.localScale.z);
					}
				} else{
					foreach(Transform transf in m_playerSprites) {
						transf.localScale = new Vector3(Mathf.Abs(transf.localScale.x), m_originalScale.y, transf.localScale.z);
					}
				}
			}

			m_isOnWall = false;
			m_fastsliding = false;
			m_startedslidingwall = false;
			m_velocity.x = wallJumpVelocity.x * (m_controller.isNear(Vector2.left,maxDistanceOffWall) ? 1 : -1);
			m_gravity = goingUpGravity;
			m_velocity.y = Mathf.Sqrt(wallJumpVelocity.y * -m_gravity);
			
			if(SoundManager.instance && jumpingClip) {
				SoundManager.instance.PlaySfx(jumpingClip);
			}

			foreach(Transform transf in m_playerSprites) {
					StartCoroutine(ChangeScale(transf.localScale * m_goingUpScaleMultiplier));
					break;
			}
		}
	}
	

	private void CamHandling(){
		if(!m_cam) return;

		if(m_controller.isGrounded) {
			m_cam.m_DeadZoneHeight = 0.02f;
			m_cam.m_ScreenY = 0.6f;
			m_cam.m_YDamping = 0.8f;
		} else {
			m_cam.m_DeadZoneHeight = 0.2f;
			m_cam.m_ScreenY = 0.5f;
			m_cam.m_YDamping = 0.1f;
		}
	}

	public void StopMovement(){
		m_velocity = Vector3.zero;
	}

	public void SetMovement(Vector3 vect) {
		m_velocity = vect;
	}

	public Vector3 GetVelocity(){
		return m_velocity;
	}

	public void ActivateSillouette() {
		foreach(Transform transf in m_playerSprites) {
			transf.GetComponent<SpriteRenderer>().material.SetFloat("_FlashAmount", 1.0f);
		}
	}

	public void ToggleSillouette() {
		foreach(Transform transf in m_playerSprites) {
			float value = transf.GetComponent<SpriteRenderer>().material.GetFloat("_FlashAmount");

			if(value > 0) {
				transf.GetComponent<SpriteRenderer>().material.SetFloat("_FlashAmount", 0.0f);
			} else if(value == 0) {
				transf.GetComponent<SpriteRenderer>().material.SetFloat("_FlashAmount", 1.0f);
			}		
		}
	}

	public void StartDialogue() {
		m_isShowingDialogue = true;
	}

	public void EndDialogue() {
		m_isShowingDialogue = false;
	}

	public void UpdatePizzaCounter() {
		if(PizzaCounterUI.instance) PizzaCounterUI.instance.UpdateCounter(Mathf.RoundToInt(PizzaCollected));
		else if(PizzaSliceCounterUI.instance) PizzaSliceCounterUI.instance.UpdateCounter(Mathf.RoundToInt(PizzaCollected));
	}
}