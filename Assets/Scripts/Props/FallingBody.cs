using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBody : MonoBehaviour, IInteractable, IInteractableLeaveTrigger, IResettableProp {

    public GameObject bodyToFall;
    // public float timeToDrop = 1.5f;
    public float fallGravity = 2f;
    public AudioClip bodyFallingClip;

    private Transform m_bodyTransform;
    private Vector2 m_bodyOriginalPosition;
    private KillOnCollision m_fallingBodyKillScript;
    private Rigidbody2D m_bodyRigidbody;
    
    private bool m_preparingToFall;
    private bool m_isFalling;
    private bool m_hasFallen;

    void Awake() {
        m_bodyTransform = bodyToFall.GetComponent<Transform>();
        m_bodyRigidbody = bodyToFall.GetComponent<Rigidbody2D>();
        m_fallingBodyKillScript = bodyToFall.GetComponent<KillOnCollision>();
        m_bodyOriginalPosition = m_bodyTransform.position;

        m_preparingToFall = false;
        m_isFalling = false;
        m_hasFallen = false;
    }

    void Start() {
        if(bodyToFall == null) {
            Debug.LogWarning("Script FallingBody não possui um corpo atribuido!");
        }

        m_fallingBodyKillScript.isDangerous = false;
    }

    private IEnumerator ShakeBodyRoutine() {
        Vector2 originalPosition = m_bodyTransform.position;
        while(m_preparingToFall) {
            m_bodyTransform.position = originalPosition + new Vector2(Random.Range(0f, .075f), Random.Range(0f, .075f));

            /* Fancy: adicionar som do objeto mexendo aqui */
            
            yield return null;
            yield return null;
        }
    }

    private IEnumerator FallBodyRoutine() {
        m_bodyRigidbody.gravityScale = fallGravity;
        m_fallingBodyKillScript.isDangerous = true;
        m_bodyRigidbody.velocity = new Vector2(0f, Mathf.Epsilon);

        /* Fancy: Adicionar som do objeto se "desprendendo" aqui */
        // if(bodyFallingClip) SoundManager.instance.PlaySfx(bodyFallingClip);

        while(m_isFalling) {
            yield return null;

            if(Mathf.Abs(m_bodyRigidbody.velocity.y) < Mathf.Epsilon) {
                m_isFalling = false;
                m_hasFallen = true;
                m_fallingBodyKillScript.isDangerous = false;
                
                /* Fancy: adicionr som do objeto colidindo aqui */
            }
        }
    }

    void IInteractable.Interact() {
        if(m_hasFallen || m_isFalling || m_preparingToFall) return;

        m_preparingToFall = true;
        StartCoroutine(ShakeBodyRoutine());
    }

    void IInteractableLeaveTrigger.Interact() {
        if(m_hasFallen || m_isFalling) return;

        m_preparingToFall = false;
        m_isFalling = true;

        StopAllCoroutines();
        StartCoroutine(FallBodyRoutine());
    }

    void IResettableProp.Reset() {
        Debug.LogError("Falling Body Reset");
        m_bodyTransform.position = m_bodyOriginalPosition;
        m_bodyRigidbody.gravityScale = 0f;
        m_bodyRigidbody.velocity = Vector2.zero;

        m_preparingToFall = false;
        m_isFalling = false;
        m_hasFallen = false;
    }
}
