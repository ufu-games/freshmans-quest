using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBehavior : MonoBehaviour {

	public float JumpHeight;
	public float Velocity;
	public float HorizontalDrag;
	public Direction myDirection = Direction.Right;
	public float MeleeAttackCooldown;
	public float MeleeAttackDamage;
	private Rigidbody2D rb;
	private bool isGrounded, justJumped;
	private Vector2 RCPositionLeft, RCPositionRight;
	private Vector2 dragVector;
	private bool canAttack;
	private Animator m_animator;
	public enum Direction {Left, Right};

	void Start () {
		rb = this.GetComponent<Rigidbody2D>();
		justJumped = false;
		isGrounded = false;
		canAttack = true;
		dragVector = new Vector2((1 - HorizontalDrag),1f);
		m_animator = GetComponent<Animator>();
	}

	void Update () {
		RCPositionLeft.x = this.transform.position.x - ((this.GetComponent<BoxCollider2D>().size.x/2)-0.2f);
		RCPositionLeft.y = this.transform.position.y - (this.GetComponent<BoxCollider2D>().size.y/2);
		RCPositionRight.x = this.transform.position.x + ((this.GetComponent<BoxCollider2D>().size.x/2)-0.2f);
		RCPositionRight.y = this.transform.position.y - (this.GetComponent<BoxCollider2D>().size.y/2);
		RaycastHit2D[] rcLeft = Physics2D.RaycastAll(RCPositionLeft,Vector2.down,0.2f); // Da cast num raycast de cada ponta de baixo do Player
		RaycastHit2D[] rcRight = Physics2D.RaycastAll(RCPositionRight,Vector2.down,0.2f);
		foreach(RaycastHit2D hit in rcLeft){
			if(hit.collider.gameObject.layer == 9 && !justJumped){// Se o raycast acertar, e o personagem não acabou de pular, isGrounded true
				isGrounded = true;
			}
		}
		foreach(RaycastHit2D hit in rcRight){
			if(hit.collider.gameObject.layer == 9 && !justJumped){
				isGrounded = true;
			}
		}
		if(Input.GetKeyDown(KeyCode.I) && isGrounded){
			StartCoroutine(Jump());
		}
		if(Input.GetKey(KeyCode.A)){
			rb.AddForce(Vector2.left * Velocity);
			myDirection = Direction.Left;
		}
		if(Input.GetKey(KeyCode.D)){
			rb.AddForce(Vector2.right * Velocity);
			myDirection = Direction.Right;
		}
		if(Input.GetKeyDown(KeyCode.O) && canAttack){
			StartCoroutine(MeleeAttack());
		}
		dragVector.x = 1 - HorizontalDrag;
		rb.velocity *= dragVector;
		AnimationLogic();
	}

	private void AnimationLogic() {
		if(myDirection == Direction.Right) {
			transform.localScale = new Vector3(1f, transform.localScale.y, transform.localScale.z);
		} else {
			transform.localScale = new Vector3(-1f, transform.localScale.y, transform.localScale.z);
		}

	if(Mathf.Abs(rb.velocity.y) > 0.1f) {
		m_animator.Play("Jump");
	} else
		if(Mathf.Abs(rb.velocity.x) > 0.1) {
			m_animator.Play("Running");
		} else {
			m_animator.Play("Idle");
		}
	}

	private IEnumerator Jump(){
		isGrounded = false;
		justJumped = true;
		rb.AddForce(new Vector2(0,(Mathf.Sqrt(JumpHeight*2*rb.gravityScale*Physics2D.gravity.magnitude)*rb.mass)),ForceMode2D.Impulse);
		yield return new WaitForSeconds(0.5f);
		justJumped = false;
	}

	private IEnumerator MeleeAttack(){
		canAttack = false;
		GameObject attack;
		if(myDirection == Direction.Left){
		  attack = (GameObject) Instantiate(Resources.Load("PlayerMeleeAttack"),rb.transform);
			attack.transform.position = rb.transform.position;
			attack.transform.position += (Vector3) Vector2.left*1.5f;
			attack.GetComponent<DamageTrigger>().damage = MeleeAttackDamage;
			attack.GetComponent<DamageTrigger>().targets.Add(10);
		} else {
			attack = (GameObject) Instantiate(Resources.Load("PlayerMeleeAttack"),rb.transform);
			attack.transform.position = rb.transform.position;
			attack.transform.position += (Vector3) Vector2.right*1.5f;
			attack.GetComponent<DamageTrigger>().damage = MeleeAttackDamage;
			attack.GetComponent<DamageTrigger>().targets.Add(10);
		}
		yield return new WaitForSeconds(1f);
		Destroy(attack.gameObject,0);
		yield return new WaitForSeconds(MeleeAttackCooldown);
		canAttack = true;
	}
}
