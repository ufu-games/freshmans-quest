using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GroundEnemyBehavior : MonoBehaviour {

    //The target player
    public Transform player;
    //At what distance will the enemy walk towards the player?
    public float walkingDistance = 10.0f;
    //In what time will the enemy complete the journey between its position and the players position
    public float smoothTime = 10.0f;
    //Vector3 used to store the velocity of the enemy
    private Vector3 smoothVelocity = Vector3.zero;
    private float m_originalScale;
    private Animator m_animator;

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        m_originalScale = transform.localScale.x;
        m_animator = GetComponent<Animator>();
    }
    void Update()
    {
        if((player.position.x - transform.position.x) > 0) {
            transform.localScale = new Vector3(-m_originalScale, transform.localScale.y, transform.localScale.z);
        } else {
            transform.localScale = new Vector3(m_originalScale, transform.localScale.y, transform.localScale.z);
        }

        float distance = Vector3.Distance(transform.position, player.position);
        
        if (distance < walkingDistance)
        {
            m_animator.Play("Running");
            transform.position = Vector3.SmoothDamp(transform.position, player.position, ref smoothVelocity, smoothTime);
        } else {
            m_animator.Play("Idle");
        }
    }
}
