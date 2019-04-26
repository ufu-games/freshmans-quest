using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TriggerSpikeParent : MonoBehaviour, IResettableProp
{   
    [Header("Sprites")]
    public Sprite SpikeInsideSprite;
    public Sprite SpikeAntecipationSprite;
    public Sprite SpikePopOutSprite;
    [Header("Time Durations")]
    public float AntecipationTime = 1;
    public float ActivatedTime = 0.5f;

    private BoxCollider2D m_collider;
    private SpriteRenderer m_renderer;
    private TriggerSpikeChild m_child;

    private bool m_isTriggering = false;
    private bool m_gotTriggered = false;

    void Start()
    {
        m_renderer = GetComponent<SpriteRenderer>();
        if(!m_renderer) {
            gameObject.AddComponent<SpriteRenderer>();
            m_renderer = GetComponent<SpriteRenderer>();
            m_renderer.sprite = null;
            m_renderer.sortingLayerName = "Platforms";
            m_renderer.sortingOrder = -1;
        }

        m_collider = GetComponent<BoxCollider2D>();
        if(!m_collider) {
            Debug.LogError($"{this.gameObject.name} - {this.GetType()}: This Gameobject is missing a Box Collider 2D, Aborting");
            Destroy(this);
        }

        m_child = GetComponentInChildren<TriggerSpikeChild>();
        if(!m_child) {
            Debug.LogError($"{this.gameObject.name} - {this.GetType()}: This Gameobject is missing a child with the TriggerSpikeChild script, Aborting");
            Destroy(this);
        }
        
        if(SpikeInsideSprite == null) {
            SpikeInsideSprite = m_renderer.sprite;
        }

        if(!SpikeInsideSprite || !SpikeAntecipationSprite || !SpikePopOutSprite) {
            Debug.LogError($"{this.gameObject.name} - {this.GetType()}: This component is missing some of their sprites references, Aborting");
            Destroy(this);
        }

        m_renderer.sprite = SpikeInsideSprite;
        m_collider.enabled = false;
        
        if(this.tag != "Resetable") {
            this.tag = "Resetable";
        }

        if(this.gameObject.layer != LayerMask.NameToLayer("Platform")) {
            this.gameObject.layer = LayerMask.NameToLayer("Platform");
        }

        if(m_child.gameObject.layer != LayerMask.NameToLayer("Interactable")) {
            m_child.gameObject.layer = LayerMask.NameToLayer("Interactable");
        }  

        StartCoroutine(MyCoroutine());
    }

    IEnumerator MyCoroutine() {
        while(this) {
            if(m_gotTriggered) {
                m_isTriggering = true;
                m_gotTriggered = false;
                m_renderer.sprite = SpikeAntecipationSprite;
                yield return new WaitForSeconds(AntecipationTime);

                m_renderer.sprite = SpikePopOutSprite;
                m_collider.enabled = true;
                yield return new WaitForSeconds(ActivatedTime);

                m_renderer.sprite = SpikeInsideSprite;
                m_collider.enabled = false;
                m_isTriggering = false;
            }
            yield return null;
        }
    }

    public void GotTriggered() {
        if(!m_isTriggering) {
            m_gotTriggered = true;
        }
    }

    public void Reset() {
        m_isTriggering = false;
        m_gotTriggered = false;
        m_renderer.sprite = SpikeInsideSprite;
        m_collider.enabled = false;
        StopAllCoroutines();
        StartCoroutine(MyCoroutine());
    }
}