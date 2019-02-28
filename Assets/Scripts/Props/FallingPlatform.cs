using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour, ICollisionInteraction {
    public float timeUntilFall = 3f;
    public float timeUntilReappear = 3f;
    public float reappearAnimationTime = 1f;

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
        float step = (1.0f / (reappearAnimationTime / Time.deltaTime));
        float timeElapsed = 0f;
        
        while(timeElapsed < reappearAnimationTime) {
            timeElapsed += Time.deltaTime;
            Color tempColor = m_spriteRenderer.color;
            tempColor.a += step;
            m_spriteRenderer.color = tempColor;
            yield return null;
        }

        m_boxCollider.enabled = true;
        m_canBeInteractWith = true;
    }

    private IEnumerator FallPlatformRoutine() {
        // roughly estimating 30fps;
        float stepsInFrames = timeUntilFall / Time.deltaTime;
        float step = (1.0f / stepsInFrames); 
        float timeElapsed = 0f;

        // change this later, shaders are a good idea
        while(timeElapsed < timeUntilFall) {
            timeElapsed += Time.deltaTime;
            Color tempColor = m_spriteRenderer.color;
            tempColor.a -= step;
            m_spriteRenderer.color = tempColor;

            yield return null;
        }

        m_spriteRenderer.enabled = false;
        m_boxCollider.enabled = false;
        StartCoroutine(ReappearPlatformRoutine());
    }
    void ICollisionInteraction.Interact() {
        if(!m_canBeInteractWith) return; 

        Debug.LogWarning("Collision with the Falling Platform");
        m_canBeInteractWith = false;
        StartCoroutine(FallPlatformRoutine());
    }
}
