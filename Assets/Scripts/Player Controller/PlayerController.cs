using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	
	// Movement Handling
	private const float runSpeed = 7f;
	private const float groundDamping = .25f; // how fast do we change direction? higher means faster
	private const float slippingFrictionMultiplier = .1f;
	
	// Jump Handling
	private const float goingUpGravity = -35f;
	private const float goingDownGravity = -60f;
	[ReadOnly]
	private float m_gravity;
	private const float inAirDamping = 0.08333f;
	private const float jumpHeight = 2f;
	private const float jumpPressedRememberTime = 0.1f;
	private const float groundedRememberTime = 0.1f;
	private const float cutJumpHeight = 0.35f;

	private float m_jumpPressedRemember;
	private float m_groundedRemember;
	
	// WALL JUMP HANDLING Handling
	private const float onWallGravity = -1f;
	private Vector2 wallJumpVelocity = new Vector2(10f, 3.0f);
	private const float maxDistanceOffWall = 5;
	
	[ReadOnly]
	private bool m_isOnWall;
	private bool m_skipMoveOnUpdateThisFrame = false;
	private bool m_startedslidingwall = false;
	private bool m_fastsliding = false;

	[Space(5)]
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
	
	// Bloqueando Direcionais no WallJump
	private bool m_blockingHorizontalControl = false;
	private Coroutine m_blockInputOnWallJumpCoroutine;

	public enum EPlayerState {
		Normal,
		Jumping,
		OnWall,
		Cannon,
		MoveBlockedDialogue, // cutscenes, dialogues...
	}

	private EPlayerState m_currentPlayerState;

	void Awake() {
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

		if(dialogueHintObject){
			dialogueHintObject.SetActive(false);
		} else {
			Debug.LogWarning("Player nao possui DialogueHint!");
		}

		m_currentPlayerState = EPlayerState.Normal;
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

		if(showDialogue != null) {
			// [TO DO] Check if it needs to check whether or not dialogue is already playing
			if(InputManager.instance.PressedStartDialogue()) {
				SaveSystem.instance.NPCChatted();
				showDialogue.ShowDialogue();
				StartDialogue();
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
			float t_jumpingPlatformMultiplier = col.gameObject.GetComponent<JumpingPlatform>().jumpingMultiplier;
            float rotationAngle = col.gameObject.transform.eulerAngles.z;
			m_gravity = goingUpGravity;
            float maxVelocity = Mathf.Sqrt(t_jumpingPlatformMultiplier * 2f * jumpHeight * -m_gravity);
			this.getsThrownTo(rotationAngle, maxVelocity);
			m_skipMoveOnUpdateThisFrame = true;
		}

		if(col.gameObject.layer == LayerMask.NameToLayer("Cannon")){
			isInCannon = true;
			this.transform.position = col.gameObject.transform.position;
			m_velocity = Vector2.zero;
			foreach(Animator ani in m_animators) {
				ani.gameObject.GetComponent<SpriteRenderer>().enabled = false;
			}
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
	#endregion

	void Update()
	{	
		m_groundedRemember -= Time.deltaTime;
		m_jumpPressedRemember -= Time.deltaTime;
		Debug.LogWarningFormat("Current Player State: {0}", m_currentPlayerState);

		SaveSystem.instance.TickTimePlayed();

		// for some reason this wasn't working on the ProcessNormalState()
		if(m_controller.isGrounded) {
			m_groundedRemember = groundedRememberTime;
			m_gravity = goingUpGravity;
			m_velocity.y = 0;
			
			// wasn't grounded last frame
			if(!m_controller.collisionState.wasGroundedLastFrame) {
				m_blockingHorizontalControl = false;
				Instantiate(landingParticles, transform.position + (Vector3.down / 2f), Quaternion.identity).Play();
				
				if(SoundManager.instance && stepClips.Length > 0) {
					SoundManager.instance.PlaySfx(stepClips[Random.Range(0, stepClips.Length)]);
				}

				foreach(Transform transf in m_playerSprites) {
					StartCoroutine(ChangeScale(transf.localScale * m_groundingScaleMultiplier));
					break;
				}
			}
		}

		if(InputManager.instance.PressedJump()) {
			m_jumpPressedRemember = jumpPressedRememberTime;
		}

		switch(m_currentPlayerState) {
			case EPlayerState.Normal:
				ProcessNormalState();
			break;
			case EPlayerState.Jumping:
				ProcessJumpingState();
			break;
			case EPlayerState.OnWall:
			break;
			case EPlayerState.Cannon:
				ProcessCannonState();
			break;
			case EPlayerState.MoveBlockedDialogue:
				ProcessDialogueState();
			break;
		}

		AnimationLogic();
		m_velocityLastFrame = m_velocity;
		CamHandling();

		// Handling Player Velocity and Moving
		float t_groundDamping = m_isSlipping ? (groundDamping * slippingFrictionMultiplier) : groundDamping;
		var smoothedMovementFactor = m_controller.isGrounded ? t_groundDamping : inAirDamping;

		m_velocity.x = Mathf.Lerp(normalizedHorizontalSpeed * runSpeed, m_velocity.x,Mathf.Pow(1 - smoothedMovementFactor, Time.deltaTime*60));
		
		// // limiting gravity
		m_velocity.y = Mathf.Max(m_gravity, m_velocity.y + (m_gravity * Time.deltaTime + (.5f * m_gravity * (Time.deltaTime * Time.deltaTime))));
		
		Vector2 deltaPosition = new Vector2(m_velocity.x * Time.deltaTime,m_velocity.y * Time.deltaTime);

		if(!m_skipMoveOnUpdateThisFrame) {
			m_controller.move( deltaPosition );
			m_velocity = m_controller.velocity;
		}
		
		// ignora as "one way platforms" por um frame (para cair delas)
		// ISSO AQUI DÁ UM BUG
		// SE VC SEGURAR PRA BAIXO E PULAR, O PERSONAGEM DÁ UM PULÃO
		// A PARTIR DE AGORA ISSO É UMA FEATURE
		if( m_controller.isGrounded && Input.GetKey( KeyCode.DownArrow ) ) {
			m_velocity.y *= Mathf.Pow(3f,Time.deltaTime);
			m_controller.ignoreOneWayPlatformsThisFrame = true;
		}

		if(m_controller.isGrounded) {
			m_startedslidingwall = false;
			m_fastsliding = false;
		}

		m_skipMoveOnUpdateThisFrame = false;
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

	#region Processing States
	private void ProcessNormalState() {
		Move();
		Jump();
	}

	private void ProcessJumpingState() {
		Move();
		WallJump();

		// Cutting Jump
		if(InputManager.instance.ReleasedJump()) {
			if(m_velocity.y > 0) {
				m_velocity.y = m_velocity.y * Mathf.Pow(cutJumpHeight,Time.deltaTime*60);
			}
		}

		// Changing Gravity on Fall
		if(m_velocity.y < 0 && !m_isOnWall) {
			m_gravity = goingDownGravity;
		}

		if(m_controller.isGrounded) {
			m_currentPlayerState = EPlayerState.Normal;
		}
	}

	private void ProcessDialogueState() {
		if(!DialogueManager.instance.isShowingDialogue) m_currentPlayerState = EPlayerState.Normal;
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
			foreach(Animator ani in m_animators) {
				ani.gameObject.GetComponent<SpriteRenderer>().enabled = true;
			}
			isInCannon = false;
			Cannon.setActive(false);
			m_currentPlayerState = EPlayerState.Jumping;
		}

		m_skipMoveOnUpdateThisFrame = true;
	}
	#endregion

	private void Move() {
		float horizontalMovement = Input.GetAxisRaw("Horizontal");
		normalizedHorizontalSpeed = horizontalMovement;

		if(m_blockingHorizontalControl) {
			normalizedHorizontalSpeed = 0;
		}

		if(Mathf.Abs(normalizedHorizontalSpeed) > Mathf.Epsilon ||
			Mathf.Abs(m_controller.velocity.x) > 0.05f) {
				// tem que chegar por um valor relativamente alto no controller porque
				// sempre tem algum resquicio de movimento nele

				float sign = (normalizedHorizontalSpeed != 0 ? Mathf.Sign(normalizedHorizontalSpeed) : Mathf.Sign(m_controller.velocity.x));
				if(!m_isOnWall) {
					foreach(Transform transf in m_playerSprites) {
						transf.localScale = new Vector3(sign * Mathf.Abs(transf.localScale.x), transf.localScale.y, transf.localScale.z);	
					}
				}
		}
	}

	private void Jump() {
		// REGULAR JUMP
		if( ( (m_groundedRemember > 0) && (m_jumpPressedRemember > 0) ) ) {
			m_jumpPressedRemember = 0;
			m_groundedRemember = 0;
			SaveSystem.instance.Jumped();
			m_gravity = goingUpGravity;
			m_velocity.y = Mathf.Sqrt( 2f * jumpHeight * -m_gravity );
			
			if(SoundManager.instance && jumpingClip) {
				SoundManager.instance.PlaySfx(jumpingClip);
			}

			foreach(Transform transf in m_playerSprites) {
				StartCoroutine(ChangeScale(transf.localScale * m_goingUpScaleMultiplier));
				break;
			}

			// foreach(Animator ani in m_animators) {
			// 	ani.Play( "Jump" );
			// }

			// [CHANGING STATE]
			m_currentPlayerState = EPlayerState.Jumping;
		}
	}

	private void WallJump() {
		//Debug.Log("right " + m_controller.isNear(Vector2.right,maxDistanceOffWall));
		
		// Sticking to the Wall
		if(!m_isOnWall
			&&
			!m_controller.isGrounded
			&&
			((m_controller.isColliding(Vector2.left) &&  m_velocity.x < 0) || (m_controller.isColliding(Vector2.right) &&  m_velocity.x > 0)))
		{
			m_isOnWall = true;
		} else if(
			m_isOnWall
			&&
			!m_controller.isNear(Vector2.left,maxDistanceOffWall) && !m_controller.isNear(Vector2.right,maxDistanceOffWall)
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
			((m_controller.isNear(Vector2.left,maxDistanceOffWall) || m_controller.isNear(Vector2.right,maxDistanceOffWall))))
			) {

			m_isOnWall = false;
			m_fastsliding = false;
			m_startedslidingwall = false;
			m_velocity.x = wallJumpVelocity.x * (m_controller.isNear(Vector2.left,maxDistanceOffWall) ? 1 : -1);
			m_gravity = goingUpGravity;
			m_velocity.y = Mathf.Sqrt(wallJumpVelocity.y * -m_gravity);
			
			SaveSystem.instance.Jumped();

			if(SoundManager.instance && jumpingClip) {
				SoundManager.instance.PlaySfx(jumpingClip);
			}

			// Instantiating Particles
			if(m_controller.isNear(Vector2.left, maxDistanceOffWall)) {
				ParticleSystem particle = Instantiate(landingParticles, transform.position + (Vector3.left / 2f), Quaternion.identity);
				particle.transform.Rotate(0, 90, 0);
				particle.Play();
			} else if(m_controller.isNear(Vector2.right, maxDistanceOffWall)) {
				ParticleSystem particle = Instantiate(landingParticles, transform.position + (Vector3.right / 2f), Quaternion.identity);
				particle.transform.Rotate(0, -90, 0);
				particle.Play();
			}

			foreach(Transform transf in m_playerSprites) {
				StartCoroutine(ChangeScale(transf.localScale * m_goingUpScaleMultiplier));
				break;
			}

			if(m_blockInputOnWallJumpCoroutine != null) {
				StopCoroutine(m_blockInputOnWallJumpCoroutine);
			}
			m_blockInputOnWallJumpCoroutine = StartCoroutine(BlockInputOnWallJumpCoroutine());
		}
	}

	private IEnumerator BlockInputOnWallJumpCoroutine() {
		m_blockingHorizontalControl = true;
		yield return new WaitForSeconds(0.16f);
		m_blockingHorizontalControl = false;
		m_blockInputOnWallJumpCoroutine = null;
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

		foreach(Animator ani in m_animators) {
			ani.Play("Idle");
		}

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
		else if(PizzaSliceCounterUI.instance) PizzaSliceCounterUI.instance.UpdateCounter(Mathf.RoundToInt(SaveSystem.instance.myData.pizzaCounter));
	}
}