using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour, ICollisionInteraction, IResettableProp {
    public float timeUntilFall = 3f;
    public float alphaToDeactivateCollision = 0.3f;
    public float timeUntilReappear = 3f;
    public float reappearAnimationTime = 1f;
    public float alphaToReactivateCollision = 0.7f;

    public ParticleSystem fallingParticle;

    private SpriteRenderer m_spriteRenderer;
    private BoxCollider2D m_boxCollider;
    private bool m_canBeInteractWith;
    
    void Awake() {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_boxCollider = GetComponent<BoxCollider2D>();
        m_canBeInteractWith = true;
    }

    private IEnumerator ReappearPlatformRoutine() {
        yield return new WaitForSeconds(timeUntilReappear - reappearAnimationTime);
        m_spriteRenderer.enabled = true;
        float timeElapsed = 0f;
        
        while(timeElapsed < reappearAnimationTime) {
            timeElapsed += Time.deltaTime;
            Color tempColor = m_spriteRenderer.color;
            
            if(tempColor.a >= alphaToReactivateCollision) {
                m_boxCollider.enabled = true;
                m_canBeInteractWith = true;
            }

            tempColor.a = Mathf.Lerp(0, 1, (timeElapsed/reappearAnimationTime));
            m_spriteRenderer.color = tempColor;
            yield return null;
        }

        Color t_color = m_spriteRenderer.color;
        t_color.a = 1;
        m_spriteRenderer.color = t_color;
        yield return null;

    }

    private IEnumerator FallPlatformRoutine() {
        float timeElapsed = 0f;

        // change this later, shaders are a good idea
        Instantiate(fallingParticle, transform.position, Quaternion.identity).Play();
        
        while(timeElapsed < timeUntilFall) {
            timeElapsed += Time.deltaTime;
            Color tempColor = m_spriteRenderer.color;
            
            if(tempColor.a <= alphaToDeactivateCollision) {
                m_boxCollider.enabled = false;
            }

            tempColor.a = Mathf.Lerp(1, 0, (timeElapsed / timeUntilFall));
            m_spriteRenderer.color = tempColor;
            yield return null;
        }


        Color t_color = m_spriteRenderer.color;
        t_color.a = 0;
        m_spriteRenderer.color = t_color;
        yield return null;

        m_spriteRenderer.enabled = false;
        StartCoroutine(ReappearPlatformRoutine());
    }
    void ICollisionInteraction.Interact() {
        if(!m_canBeInteractWith) return; 

        Debug.LogWarning("Collision with the Falling Platform");
        m_canBeInteractWith = false;
        StartCoroutine(FallPlatformRoutine());
    }

    void IResettableProp.Reset() {
        Debug.LogFormat("Falling Platform Reset Function");

        StopAllCoroutines();
        m_spriteRenderer.enabled = true;
        m_boxCollider.enabled = true;

        Color color = m_spriteRenderer.color;
        color.a = 1;
        m_spriteRenderer.color = color;

        m_canBeInteractWith = true;
    }
}
