using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBehavior : MonoBehaviour {

	public float JumpHeight;
	public float Velocity;
	public float HorizontalDrag;
	private Rigidbody2D rb;
	private bool isGrounded, justJumped;
	private Vector2 RCPositionLeft, RCPositionRight;
	private Vector2 dragVector;

	void Start () {
		rb = this.GetComponent<Rigidbody2D>();
		justJumped = false;
		isGrounded = false;
		dragVector = new Vector2((1 - HorizontalDrag),1f);
	}

	void Update(){

	}

	void FixedUpdate () {
		RCPositionLeft.x = this.transform.position.x - ((this.transform.lossyScale.x/2)-0.2f);
		RCPositionLeft.y = this.transform.position.y - (this.transform.lossyScale.y/2);
		RCPositionRight.x = this.transform.position.x + ((this.transform.lossyScale.x/2)-0.2f);
		RCPositionRight.y = this.transform.position.y - (this.transform.lossyScale.y/2);
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
		if(Input.GetKeyDown(KeyCode.Space) && isGrounded){
			StartCoroutine(Jump());
		}
		if(Input.GetKey(KeyCode.A)){
			rb.AddForce(Vector2.left * Velocity);
		}
		if(Input.GetKey(KeyCode.D)){
			rb.AddForce(Vector2.right * Velocity);
		}
		dragVector.x = 1 - HorizontalDrag;
		rb.velocity *= dragVector;
	}

	private IEnumerator Jump(){
		isGrounded = false;
		justJumped = true;
		rb.AddForce(new Vector2(0,(Mathf.Sqrt(JumpHeight*2*rb.gravityScale*Physics2D.gravity.magnitude)*rb.mass)),ForceMode2D.Impulse);
		yield return new WaitForSeconds(0.5f);
		justJumped = false;
	}
}
