using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxController : MonoBehaviour {

	public Sprite Image;
	public float ImageSpeed;
	private GameObject m_player;
	private GameObject m_image;
	private float m_offset;
	private float m_playerInitialPosition;
	private Vector3 vect;

	void Start () {
		m_player = GameObject.FindWithTag("Player");
		m_image = new GameObject();
		m_image.AddComponent<SpriteRenderer>();
		m_image.transform.position = this.transform.position;
		m_image.GetComponent<SpriteRenderer>().sprite = Image;
		m_playerInitialPosition = m_player.transform.position.x;
	}
	
	void Update () {
		vect.x = this.transform.position.x + -ImageSpeed * (m_player.transform.position.x - m_playerInitialPosition);
		vect.y = this.transform.position.y;
		vect.z = 1;
		m_image.transform.position = vect;
	}
}
