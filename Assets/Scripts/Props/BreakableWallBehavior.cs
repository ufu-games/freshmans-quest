﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWallBehavior : MonoBehaviour, IResettableProp {

	public float MinimumVelocityToBreak = 5f;
	[SerializeField]
	[Range(0f,100f)]
	public float PlayerDamping = 50f;
	private PlayerController m_player;
	private BoxCollider2D m_bc;
	private float gap = 0.5f;
	private bool Destroyed = false;

	void Start () {
		m_player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
		m_bc = GetComponent<BoxCollider2D>();
	}

	public void Collision(Vector2 point){
		bool Collided = false;
		Vector3 vect;
		Debug.DrawLine(point,point+Vector2.up*0.01f,Color.white,5);
		vect.z = 0;
		if((point.y >= this.transform.position.y + m_bc.size.y/2 - gap && point.y <= this.transform.position.y + m_bc.size.y/2 + gap)  || (point.y >= this.transform.position.y - m_bc.size.y/2 - gap && point.y <= this.transform.position.y - m_bc.size.y/2 + gap)) {
			if(Mathf.Abs(m_player.m_velocityLastFrame.y) >= MinimumVelocityToBreak){
				vect.x = m_player.m_velocityLastFrame.x;
				vect.y = m_player.m_velocityLastFrame.y*PlayerDamping/100;
				Collided = true;
				m_player.SetMovement(vect);
				
			}
		}
		if((point.x >= this.transform.position.x + m_bc.size.x/2 - gap && point.x <= this.transform.position.x + m_bc.size.x/2 + gap)  || (point.x >= this.transform.position.x - m_bc.size.x/2 - gap && point.x <= this.transform.position.x - m_bc.size.x/2 + gap) && !Collided) {
			if(Mathf.Abs(m_player.m_velocityLastFrame.x) > MinimumVelocityToBreak){
				vect.x = m_player.m_velocityLastFrame.x*PlayerDamping/100;
				vect.y = m_player.m_velocityLastFrame.y;
				Collided = true;
				m_player.SetMovement(vect);
			}
		}

		if(Collided) {
			GetComponent<SpriteRenderer>().enabled = false;
			GetComponent<BoxCollider2D>().enabled = false;
			Destroyed = true;
		}
	}

	public void Reset() {
		if(Destroyed) {
			GetComponent<SpriteRenderer>().enabled = false;
			GetComponent<BoxCollider2D>().enabled = false;
			Destroyed = false;
		}
	}
}
