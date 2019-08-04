using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [Header("Movement Parameters")]
    public float runSpeed = 7.5f;
	public float dashSpeed = 10f;
    public float jumpPeakHeight = 4.5f;
    public float horizontalDistanceToJumpPeak = 4.5f;

    [Header("Environment Parameters")]
    public float jumpingPlatformJumpingVelocityMultiplier = 1.25f;

    // Movement Handling
    [Header("Movement Damping")]
    public float groundDamping = .25f;
    public float inAirDamping = 0.08333f;
    private const float km_slippingFrictionMultiplier = .1f;

    // Jump Handling
    [ReadOnly]
    private float m_gravity;
    private float m_goingUpGravity;
	private float m_goingDownGravity;
    private float m_jumpInitialVelocity;

	private const float km_terminalVelocity = -20f;
	private const float km_jumpPressedRememberTime = 0.15f;
	private const float km_groundedRememberTime = 0.15f;
	private const float km_cutJumpHeight = 0.5f;
	private const float km_dashTime = 0.3f;

    private float m_timeToJumpPeak;

	private float m_jumpPressedRemember;
	private float m_groundedRemember;
	private float m_dashingRemainingTime;
	private bool m_pressedDownArrowLastFrame = false;

	[ReadOnly]
	public bool m_jumpedLastFrame = false;
	
	// WALL JUMP HANDLING Handling
	private const float km_onWallGravity = -1f;
	private Vector2 m_wallJumpVelocity;
	private const float km_maxDistanceOffWall = 5;
	
	[ReadOnly]
	private bool m_isOnWall;
	private bool m_skipMoveOnUpdateThisFrame = false;
	
	[Space(5)]
	public GameObject dialogueHintObject;
	
	[Space(5)]
	[Header("Particle Effects")]
	public ParticleSystem landingParticles;

	[HideInInspector]
	private float normalizedHorizontalSpeed = 0;

	private Prime31.CharacterController2D m_controller;
	public Transform m_playerSprite;
	private Animator m_animator;
	private Vector3 m_velocity;

	// Scale Juicing
	private Vector2 m_originalScale;
	private Vector2 m_goingUpScaleMultiplier = new Vector2(0.7f, 1.3f);
	private Vector2 m_groundingScaleMultiplier = new Vector2(1.3f, 0.7f);

	// easily extendable
	private bool isInCannon = false;
	private CannonBehaviour Cannon;
	private bool m_isSlipping = false;

	//Breakable Wall Handling
	[ReadOnly]
	public Vector3 m_velocityLastFrame;

	public enum EPlayerState {
		Normal,
		Jumping,
		OnWall,
        JumpingFromWall,
		Cannon,
		MoveBlockedDialogue, // cutscenes, dialogues...
		IsPettingDog, // Yes, I did it.
		Dashing,
	}

	private EPlayerState m_currentPlayerState;

	private const float petTheDogDistance = 0.1f;

	void Awake() {
        // Calculating Gravity and Jump Initial Velocity...
        m_jumpInitialVelocity = (2 * jumpPeakHeight * runSpeed) / (horizontalDistanceToJumpPeak);
        m_goingUpGravity = -((2 * jumpPeakHeight * runSpeed * runSpeed) / (horizontalDistanceToJumpPeak * horizontalDistanceToJumpPeak));
        m_goingDownGravity = m_goingUpGravity * 2.75f;
        m_gravity = m_goingUpGravity;
        m_wallJumpVelocity = new Vector2(runSpeed, m_jumpInitialVelocity);
        m_timeToJumpPeak = horizontalDistanceToJumpPeak / runSpeed;

        // Caching Components
        m_animator = GetComponentInChildren<Animator>();
		m_playerSprite = m_animator.transform;
		m_controller = GetComponent<Prime31.CharacterController2D>();
		m_controller.onControllerCollidedEvent += onControllerCollider;
		m_controller.onTriggerEnterEvent += onTriggerEnterEvent;
		m_controller.onTriggerStayEvent += OnTriggerStayEvent;
		m_controller.onTriggerExitEvent += onTriggerExitEvent;
		m_originalScale = m_playerSprite.localScale;

		if(dialogueHintObject){
			dialogueHintObject.SetActive(false);
		} else {
			Debug.LogWarning("Player nao possui DialogueHint!");
		}

		m_currentPlayerState = EPlayerState.Normal;
    }

	void Start() {
		// Setting the Pizza Count
		if(PizzaCounterUI.instance && SaveSystem.instance) PizzaCounterUI.instance.UpdateCounterWithoutRoutine(SaveSystem.instance.myData.pizzaCounter);
	}


    #region Event Listeners

    void onControllerCollider( RaycastHit2D hit ) {
		m_isSlipping = (hit.transform.gameObject.tag == "Slippery" && !(m_controller.isColliding(Vector2.left) || m_controller.isColliding(Vector2.right)));

		IDangerous dangerousInteraction = hit.collider.gameObject.GetComponent<IDangerous>();
		ICollisionInteraction collisionInteraction = hit.collider.gameObject.GetComponent<ICollisionInteraction>();
		
		if(hit.collider.tag == "BreakableWall") {
			hit.collider.gameObject.GetComponent<BreakableWallBehavior>().Collision(hit.point);
		}

		if(dangerousInteraction != null) {
			dangerousInteraction.InteractWithPlayer(this.GetComponent<Collider2D>());
		}

		if(collisionInteraction != null) {
			if(m_controller.isGrounded) {
				collisionInteraction.Interact();
			}
		}

		if(hit.collider.tag == "MovingPlataform") {
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
		
		// bail out on plain old ground hits cause they arent very interesting
		if( hit.normal.y == 1f )
			return;
		// logs any collider hits if uncommented. it gets noisy so it is commented out for the demo
		// Debug.LogWarning( "flags: " + m_controller.collisionState + ", hit.normal: " + hit.normal );	
	}

	void OnTriggerStayEvent(Collider2D col) {
		IShowDialogue showDialogue = col.gameObject.GetComponent<IShowDialogue>();
		IPetable canPet = col.gameObject.GetComponent<IPetable>();

		if(m_currentPlayerState == EPlayerState.MoveBlockedDialogue || m_currentPlayerState == EPlayerState.IsPettingDog) return;

		if(showDialogue != null) {
			if(InputManager.instance.PressedStartDialogue() && m_currentPlayerState != EPlayerState.MoveBlockedDialogue) {
				SaveSystem.instance?.NPCChatted();
				showDialogue.ShowDialogue();
				StartDialogue();
			}
		}

		// as much as we would love it
		// petting the dog while you are already petting the dog would result on some programming issues!
		if(canPet != null && m_currentPlayerState != EPlayerState.IsPettingDog) {
			if(InputManager.instance.PressedToPetTheDog()) {
				this.m_currentPlayerState = EPlayerState.IsPettingDog;
				canPet.Pet();
				Vector3 difPosition = col.gameObject.transform.position - transform.position;
				m_playerSprite.transform.localScale = new Vector3(Mathf.Sign(difPosition.x) * Mathf.Abs(m_playerSprite.transform.localScale.x), m_playerSprite.transform.localScale.y, m_playerSprite.transform.localScale.z);
                // CameraScript.instance.ForceCameraSize(3f);
				StartCoroutine(HideDialogueHintRoutine());
			}
		}
	}

	void onTriggerEnterEvent(Collider2D col) {
		// Debug.LogWarning( "onTriggerEnterEvent: " + col.gameObject.name );

		IDangerous dangerousInteraction = col.gameObject.GetComponent<IDangerous>();
		IInteractable interaction = col.gameObject.GetComponent<IInteractable>();
		INonHarmfulInteraction nonHarmfulInteraction = col.gameObject.GetComponent<INonHarmfulInteraction>();
		IShowDialogue showDialogue = col.gameObject.GetComponent<IShowDialogue>();

		if(showDialogue != null && dialogueHintObject) {
			StartCoroutine(ShowDialogueHintRoutine());
		}

		if(dangerousInteraction != null) {
			
			PlayerDeath();
			dangerousInteraction.InteractWithPlayer(this.GetComponent<Collider2D>());
		}

		if(interaction != null) {
			interaction.Interact();
		}

		if(nonHarmfulInteraction != null) {
			nonHarmfulInteraction.InteractWithPlayer(this.GetComponent<Collider2D>());
		}

		if(col.gameObject.layer == LayerMask.NameToLayer("JumpingPlatform")) {
			m_gravity = m_goingUpGravity;
			m_velocity.y = jumpingPlatformJumpingVelocityMultiplier * m_jumpInitialVelocity;
			
			if(SoundManager.instance && SoundManager.instance.Settings.Player_jump != "") {
				SoundManager.instance.PlaySfx(SoundManager.instance.Settings.Player_jump);
			}

			StartCoroutine(ChangeScale(m_playerSprite.localScale * m_goingUpScaleMultiplier));
			// [CHANGING STATE]
			m_currentPlayerState = EPlayerState.Jumping;
		}

		if(col.gameObject.layer == LayerMask.NameToLayer("Cannon")){
			isInCannon = true;
			this.transform.position = col.gameObject.transform.position;
			m_velocity = Vector2.zero;
			m_animator.GetComponent<SpriteRenderer>().enabled = false;
			this.Cannon = col.gameObject.GetComponent<CannonBehaviour>();
			Camera.main.GetComponentInChildren<CinemachineVirtualCamera>().m_Lens.OrthographicSize *= this.Cannon.zoomOutMultiplier;
			this.Cannon.setActive(true);
			m_currentPlayerState = EPlayerState.Cannon;
		}
	}

	void onTriggerExitEvent( Collider2D col ) {
		if(!this.enabled) return;

		INonHarmfulInteraction nonHarmfulInteraction = col.gameObject.GetComponent<INonHarmfulInteraction>();
		IInteractableLeaveTrigger interactWhenLeft = col.gameObject.GetComponent<IInteractableLeaveTrigger>();
		IShowDialogue showDialogue = col.gameObject.GetComponent<IShowDialogue>();

		if(showDialogue != null && dialogueHintObject) {
			 StartCoroutine(HideDialogueHintRoutine());
		}

		if(nonHarmfulInteraction != null) {
			nonHarmfulInteraction.InteractWithPlayer(this.GetComponent<Collider2D>());
		}

		if(interactWhenLeft != null) {
			interactWhenLeft.Interact();
		}
	}

	public void getsThrownTo(float rotationAngle, float maxVelocity){
		m_jumpPressedRemember = 0;
		m_groundedRemember = 0;
		m_gravity = m_goingUpGravity;

		m_velocity.y = Mathf.Cos(rotationAngle * Mathf.Deg2Rad) * maxVelocity;
		m_velocity.x = -Mathf.Sin(rotationAngle * Mathf.Deg2Rad) * maxVelocity;

		Vector2 deltaPosition = new Vector2(m_velocity.x * Time.deltaTime,m_velocity.y*Time.deltaTime);
	
		m_controller.move( deltaPosition );

		m_animator.Play("Jump");
		StartCoroutine(ChangeScale(m_playerSprite.localScale * m_goingUpScaleMultiplier));

		if(isInCannon)
			Camera.main.GetComponentInChildren<CinemachineVirtualCamera>().m_Lens.OrthographicSize /= this.Cannon.zoomOutMultiplier;
	}
	#endregion

	void Update()
	{	
		m_groundedRemember -= Time.deltaTime;
		m_jumpPressedRemember -= Time.deltaTime;
        normalizedHorizontalSpeed = Input.GetAxisRaw("Horizontal");

        SaveSystem.instance?.TickTimePlayed();

		// for some reason this wasn't working on the ProcessNormalState()
		if(m_controller.isGrounded) {
			m_isOnWall = false;
			m_groundedRemember = km_groundedRememberTime;
			m_velocity.y = 0;
			
			// wasn't grounded last frame
			if(!m_controller.collisionState.wasGroundedLastFrame) {
				Instantiate(landingParticles, transform.position + (Vector3.down / 2f), Quaternion.identity).Play();
				
				if(SoundManager.instance && SoundManager.instance.Settings.Player_walk != "") {
					SoundManager.instance.PlaySfx(SoundManager.instance.Settings.Player_walk);
				}

				if(Mathf.Abs(m_velocityLastFrame.y - km_terminalVelocity) < 0.01f) {
					InputManager.instance.VibrateWithTime(.75f, 0.15f);
				}

				StartCoroutine(ChangeScale(m_playerSprite.localScale * m_groundingScaleMultiplier));
			}
		} else {
			if(m_currentPlayerState == EPlayerState.Normal) {
				m_currentPlayerState = EPlayerState.Jumping;
			}
		}

		if(InputManager.instance.PressedJump()) {
			m_jumpPressedRemember = km_jumpPressedRememberTime;
		}

		switch(m_currentPlayerState) {
			case EPlayerState.Normal:
				m_jumpedLastFrame = false;
				ProcessNormalState();
			break;
			case EPlayerState.Jumping:
				m_jumpedLastFrame = false;
				ProcessJumpingState();
			break;
			case EPlayerState.OnWall:
				// Add Wall Rest here if needed.
			break;
            case EPlayerState.JumpingFromWall:
                ProcessJumpingFromWallState();
                break;
			case EPlayerState.Cannon:
				ProcessCannonState();
			break;
			case EPlayerState.MoveBlockedDialogue:
				ProcessDialogueState();
			break;
			case EPlayerState.IsPettingDog:
				ProcessPettingState();
			break;
			case EPlayerState.Dashing:
				ProcessDashingState();
			break;
		}
		AnimationLogic();
        ProcessSpriteScale();
        m_velocityLastFrame = m_velocity;
		m_pressedDownArrowLastFrame = Input.GetAxisRaw("Vertical") == -1;

		// Handling Player Velocity and Moving
		// Horizontal Velocity
		float t_groundDamping = m_isSlipping ? (groundDamping * km_slippingFrictionMultiplier) : groundDamping;
		var smoothedMovementFactor = m_controller.isGrounded ? t_groundDamping : inAirDamping;
        // m_velocity.x = Mathf.Lerp(normalizedHorizontalSpeed * runSpeed, m_velocity.x , Mathf.Pow(1 - smoothedMovementFactor, Time.deltaTime*60));
        // m_velocity.x = Mathf.Lerp(m_velocity.x, normalizedHorizontalSpeed * runSpeed, Mathf.Pow(1 - smoothedMovementFactor, Time.deltaTime * 60));
        float t_xVelocityLerp = Mathf.Clamp01(smoothedMovementFactor * Time.deltaTime);
        m_velocity.x = Mathf.Lerp(m_velocity.x, normalizedHorizontalSpeed * runSpeed, t_xVelocityLerp);

        // Vertical Velocity
        // ACCELERATING with gravity
        m_velocity.y += m_gravity * Time.deltaTime;
		m_velocity.y = Mathf.Max(km_terminalVelocity, m_velocity.y);
        Vector2 velocityVerlet = new Vector2(m_velocity.x, m_velocity.y + (.5f * m_gravity * Time.deltaTime * Time.deltaTime));
		
		Vector2 eulerDeltaPosition = m_velocity * Time.deltaTime;
        Vector2 velocityVerletDeltaPosition = velocityVerlet * Time.deltaTime;

		if(!m_skipMoveOnUpdateThisFrame) {
			m_controller.move(velocityVerletDeltaPosition);
			m_velocity = m_controller.velocity;
		}

		m_skipMoveOnUpdateThisFrame = false;
	}

	private void AnimationLogic() {
		if(gameObject.activeSelf){
			// Handling other game events
			if(m_currentPlayerState == EPlayerState.IsPettingDog) {
				m_animator.Play("Petting");
			} else if(m_currentPlayerState == EPlayerState.Dashing) {
				m_animator.Play("Dashing");
			} else { // Handling normal game events
				if(m_isOnWall) {
					m_animator.Play("Wall");
				} else if(Mathf.Abs(m_velocity.y) > Mathf.Epsilon) {
					if(m_velocity.y > 0) {
						m_animator.Play("Jump");
					} else {
						m_animator.Play("Falling");
					}
				} else if(Mathf.Abs(normalizedHorizontalSpeed) > 0 && m_controller.isGrounded) {
					m_animator.Play("Running");
				} else {
					m_animator.Play("Idle");
				}
			}	
		}
	}

	#region Processing States
	private void ProcessNormalState() {
        // REGULAR JUMP
        if (((m_groundedRemember > 0) && (m_jumpPressedRemember > 0))) {
            m_jumpPressedRemember = 0;
            m_groundedRemember = 0;
            m_jumpedLastFrame = true;
            SaveSystem.instance?.Jumped();

            m_gravity = m_goingUpGravity;
            m_velocity.y = m_jumpInitialVelocity;

            if (SoundManager.instance && SoundManager.instance.Settings.Player_jump != "") {
                SoundManager.instance.PlaySfx(SoundManager.instance.Settings.Player_jump);
            }

            StartCoroutine(ChangeScale(m_playerSprite.localScale * m_goingUpScaleMultiplier));
            m_currentPlayerState = EPlayerState.Jumping;
        }

		// DASH
		if(m_groundedRemember > 0 && Input.GetAxisRaw("Vertical") == -1 && !m_pressedDownArrowLastFrame) {
			m_dashingRemainingTime = km_dashTime;
			m_currentPlayerState = EPlayerState.Dashing;
		}
    }

	private void ProcessJumpingState() {
		WallJump();

		// Cutting Jump
		if(InputManager.instance.ReleasedJump()) {
			if(m_velocity.y > 0) {
				m_velocity.y *= Mathf.Pow(km_cutJumpHeight , Time.deltaTime*60);
			}
		}

		// Changing Gravity on Fall
        if(m_isOnWall) {
            m_gravity = km_onWallGravity; 
        } else if(m_velocity.y > 0) {
            m_gravity = m_goingUpGravity;
		} else {
            m_gravity = m_goingDownGravity;
        }

		if(m_controller.isGrounded) {
			m_currentPlayerState = EPlayerState.Normal;
		}
	}

    private void ProcessJumpingFromWallState() {
        if(m_controller.isGrounded) {
            m_currentPlayerState = EPlayerState.Normal;
        }
    }


    private void ProcessDialogueState() {
        if (!DialogueManager.instance.isShowingDialogue) {
            m_currentPlayerState = EPlayerState.Normal;
        }
	}

	private void ProcessPettingState() {
		if(InputManager.instance.StopPettingTheDog()) {
			Debug.Log("You are no longer petting the dog :(");

			// CameraScript.instance.UnforceCameraSize();
			SoundManager.instance.ChangeMusicAccordingToScene();
			StartCoroutine(ShowDialogueHintRoutine());
			m_currentPlayerState = EPlayerState.Normal;
		}
	}

	private void ProcessCannonState() {
		if(InputManager.instance.PressedJump()){
			// [TO DO] ver a possibilidade de zerar a gravidade por um tempo após o tiro
			var angleCannon = Cannon.getAngle();
			float distanceBetweenCenterAndExplosion = 0;
			getsThrownTo(angleCannon, Cannon.getThrowMultiplier());
			GameObject part = (GameObject) Instantiate(Resources.Load("BeckerCannonParticle"));
			float partangle = angleCannon+90;
			part.transform.rotation = Quaternion.Euler(-partangle,90,0);
			part.transform.position = new Vector3(Cannon.transform.position.x + distanceBetweenCenterAndExplosion*Mathf.Cos(Mathf.Deg2Rad*(angleCannon+90)), Cannon.transform.position.y + distanceBetweenCenterAndExplosion*Mathf.Sin(Mathf.Deg2Rad*(angleCannon+90)),0);
			part.GetComponent<ParticleSystem>().Play();
			m_animator.gameObject.GetComponent<SpriteRenderer>().enabled = true;
			isInCannon = false;
			Cannon.setActive(false);

			m_currentPlayerState = EPlayerState.Jumping;
		}

		m_skipMoveOnUpdateThisFrame = true;
	}

	private void ProcessDashingState() {
		m_dashingRemainingTime -= Time.deltaTime;

		if(m_dashingRemainingTime > 0) {
			m_controller.move(m_velocity.x > 0 ? new Vector3(dashSpeed * Time.deltaTime,0,0) : new Vector3(-dashSpeed * Time.deltaTime,0,0));

			m_skipMoveOnUpdateThisFrame = true;
		} else {
			m_dashingRemainingTime = 0;
			if(m_groundedRemember > 0) {
				m_currentPlayerState = EPlayerState.Normal;
			} else {
				m_currentPlayerState = EPlayerState.Jumping;
				m_gravity = m_goingUpGravity;
			}
		}
	}
	#endregion

	private void ProcessSpriteScale() {
		if(Mathf.Abs(normalizedHorizontalSpeed) > Mathf.Epsilon ||
			Mathf.Abs(m_controller.velocity.x) > 0.05f) {
				// tem que chegar por um valor relativamente alto no controller porque
				// sempre tem algum resquicio de movimento nele

				float sign;
				if(m_currentPlayerState == EPlayerState.Dashing) {
					sign = Mathf.Sign(m_controller.velocity.x);
				} else {
					sign = (normalizedHorizontalSpeed != 0 ? Mathf.Sign(normalizedHorizontalSpeed) : Mathf.Sign(m_controller.velocity.x));
				}				
				if(!m_isOnWall) {
					m_playerSprite.localScale = new Vector3(sign * Mathf.Abs(m_playerSprite.localScale.x), m_playerSprite.localScale.y, m_playerSprite.localScale.z);
				}
		}
	}

	private void WallJump() {
		//Debug.Log("right " + m_controller.isNear(Vector2.right,maxDistanceOffWall));
		
		// Sticking to the Wall
		if(!m_isOnWall
			&&
			!m_controller.isGrounded
			&&
			((m_controller.isColliding(Vector2.left) &&  Input.GetAxisRaw("Horizontal") == -1) || (m_controller.isColliding(Vector2.right) &&  Input.GetAxisRaw("Horizontal") == 1)))
		{
            m_velocity = Vector3.zero;
            m_skipMoveOnUpdateThisFrame = true;
			m_isOnWall = true;
		} else if(
			m_isOnWall
			&&
			!m_controller.isNear(Vector2.left,km_maxDistanceOffWall) && !m_controller.isNear(Vector2.right,km_maxDistanceOffWall)
			)
		{
			m_isOnWall = false;

		}

		// Jumping off the wall
		if((m_isOnWall && InputManager.instance.PressedJump())
			||
			(!m_isOnWall
			&&
			InputManager.instance.PressedJump()
			&&
			((m_controller.isNear(Vector2.left,km_maxDistanceOffWall) || m_controller.isNear(Vector2.right,km_maxDistanceOffWall))))
			) {

			m_isOnWall = false;
			m_velocity.x = m_wallJumpVelocity.x * (m_controller.isNear(Vector2.left,km_maxDistanceOffWall) ? 1 : -1);
			m_velocity.y = m_wallJumpVelocity.y;
			m_gravity = m_goingUpGravity;

            
            // Changing to Jumping From Wall
            // But after a while we want to go for our regular jumping code because we might want to wall jump again
            m_currentPlayerState = EPlayerState.JumpingFromWall;
            StartCoroutine(ChangeStateWithDelay(EPlayerState.Jumping, ( m_timeToJumpPeak / 2.0f) ));
			
			SaveSystem.instance?.Jumped();

			if(SoundManager.instance && SoundManager.instance.Settings.Player_walljump != "") {
				SoundManager.instance.PlaySfx(SoundManager.instance.Settings.Player_walljump);
			}

			// Instantiating Particles
			if(m_controller.isNear(Vector2.left, km_maxDistanceOffWall)) {
				ParticleSystem particle = Instantiate(landingParticles, transform.position + (Vector3.left / 2f), Quaternion.identity);
				particle.transform.Rotate(0, 90, 0);
				particle.Play();
			} else if(m_controller.isNear(Vector2.right, km_maxDistanceOffWall)) {
				ParticleSystem particle = Instantiate(landingParticles, transform.position + (Vector3.right / 2f), Quaternion.identity);
				particle.transform.Rotate(0, -90, 0);
				particle.Play();
			}

			StartCoroutine(ChangeScale(m_playerSprite.localScale * m_goingUpScaleMultiplier));
		}
	}

    private IEnumerator ChangeStateWithDelay(EPlayerState _playerState, float _delay) {
        yield return new WaitForSeconds(_delay);
        m_currentPlayerState = _playerState;
    }

	private IEnumerator ChangeScale(Vector2 scale) {
		m_playerSprite.localScale = scale;
		yield return new WaitForSeconds(0.075f);
		m_playerSprite.localScale = new Vector3(Mathf.Sign(m_playerSprite.localScale.x) * Mathf.Abs(m_originalScale.x), m_originalScale.y, m_playerSprite.localScale.z);
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

	#region Dialogue Related Functions

	private IEnumerator GenericDialogueHintRoutine(Vector3 startingScale, Vector3 aimScale) {
		float timeElapsed = 0f;
		float transitionTime = 0.15f;
		float t;

		dialogueHintObject.transform.localScale = startingScale;

		while(timeElapsed < transitionTime) {
			timeElapsed += Time.deltaTime;
			t = Interpolation.BounceEaseInOut(timeElapsed / transitionTime);
			dialogueHintObject.transform.localScale = Vector3.Lerp(startingScale, aimScale, t);
			yield return null;
		}

		dialogueHintObject.transform.localScale = aimScale;
		yield return null;
	}

	private IEnumerator HideDialogueHintRoutine() {
		yield return StartCoroutine(GenericDialogueHintRoutine(new Vector3(1,1,1), new Vector3(0,0,0)));
		dialogueHintObject.SetActive(false);
	}

	private IEnumerator ShowDialogueHintRoutine() {
		dialogueHintObject.SetActive(true);
		yield return StartCoroutine(GenericDialogueHintRoutine(new Vector3(0,0,0), new Vector3(1,1,1)));
	}

	public void StartDialogue() {
		if(dialogueHintObject) dialogueHintObject.SetActive(false);

		m_animator.Play("Idle");

		m_velocity = Vector2.zero;
		m_currentPlayerState = EPlayerState.MoveBlockedDialogue;
	}

	// this is called when the player leaves the trigger of the dialogue
	// this isn't supposed to happen but let's keep it here for now
	public void EndDialogue() {
		m_currentPlayerState = EPlayerState.Normal;
	}

	#endregion

	public void UpdatePizzaCounter() {
		if(PizzaCounterUI.instance) PizzaCounterUI.instance.UpdateCounter(Mathf.RoundToInt(SaveSystem.instance.myData.pizzaCounter));
	}

	public void PlayerDeath() {
		if(SoundManager.instance != null) {
			if(SoundManager.instance.Settings.Player_death != "" && SoundManager.instance) {
					SoundManager.instance.PlaySfx(SoundManager.instance.Settings.Player_death); 
					InputManager.instance.VibrateWithTime(1f, 0.3f);
			}
		}
	}
}