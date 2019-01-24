using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBody : MonoBehaviour, IInteractable, IInteractableLeaveTrigger {

    public GameObject bodyToFall;
    // public float timeToDrop = 1.5f;
    public float fallGravity = 2f;

    private Transform m_bodyTransform;
    private Rigidbody2D m_bodyRigidbody;
    
    private bool m_preparingToFall;
    private bool m_isFalling;
    private bool m_hasFallen;

    void Awake() {
        m_bodyTransform = bodyToFall.GetComponent<Transform>();
        m_bodyRigidbody = bodyToFall.GetComponent<Rigidbody2D>();

        m_preparingToFall = false;
        m_isFalling = false;
        m_hasFallen = false;
    }

    void Start() {
        if(bodyToFall == null) {
            Debug.LogWarning("Script FallingBody não possui um corpo atribuido!");
        }
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
        /* Fancy: Adicionar som do objeto se "desprendendo" aqui */
        bodyToFall.layer = LayerMask.NameToLayer("Hazard");

        while(m_isFalling) {
            yield return null;
            Debug.Log(m_bodyRigidbody.velocity);

            if(m_bodyRigidbody.velocity.y < Mathf.Epsilon) {
                m_isFalling = false;
                m_hasFallen = true;
                /* Fancy: adicionr som do objeto colidindo aqui */
            }
        }

        bodyToFall.layer = LayerMask.NameToLayer("Platform");
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
}
