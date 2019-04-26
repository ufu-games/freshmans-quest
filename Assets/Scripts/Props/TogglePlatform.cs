using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TogglePlatform : MonoBehaviour
{
    public bool IsActivatedByDefault = true;
    [ReadOnly]
    public bool IsActivatedNow;
    public Sprite ActivatedSprite;
    public Sprite DeactivatedSprite;

    private SpriteRenderer m_renderer;
    private BoxCollider2D m_collider;
    private PlayerController m_player;

    void Start()
    {
        m_renderer = GetComponent<SpriteRenderer>();
        if(!m_renderer) {
            Debug.LogError($"{this.gameObject.name} - {this.GetType()}: This Gameobject is missing a Sprite Renderer, Aborting");
            Destroy(this);
        }

        m_collider = GetComponent<BoxCollider2D>();
        if(!m_collider) {
            Debug.LogError($"{this.gameObject.name} - {this.GetType()}: This Gameobject is missing a Box Collider 2D, Aborting");
            Destroy(this);
        }

        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        
        if(this.tag != "Resetable") {
            this.tag = "Resetable";
        }

        if(this.gameObject.layer != LayerMask.NameToLayer("Platform")) {
            this.gameObject.layer = LayerMask.NameToLayer("Platform");
        }

        if(ActivatedSprite == null && IsActivatedByDefault) {
            ActivatedSprite = m_renderer.sprite;
        }

        if(DeactivatedSprite == null && !IsActivatedByDefault) {
            DeactivatedSprite = m_renderer.sprite;
        }

        if(IsActivatedByDefault) {
            Activate();
        } else {
            Deactivate();
        }
    }

    void LateUpdate()
    {
        if(m_player) {
            if(m_player.m_jumpedLastFrame) {
                if(IsActivatedNow) {
                    Deactivate();
                } else {
                    Activate();
                }
            }
        }
    }

    void Activate() {
        m_collider.enabled = true;
        m_renderer.sprite = ActivatedSprite;
        IsActivatedNow = true;
    }

    void Deactivate() {
        m_collider.enabled = false;
        m_renderer.sprite = DeactivatedSprite;
        IsActivatedNow = false;
    }
}
