using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSpikeChild : MonoBehaviour, IInteractable
{   
    private TriggerSpikeParent m_parent;

    void Start()
    {
        if(transform.parent == null) {
            Debug.LogError($"{this.gameObject.name} - {this.GetType()}: This Gameobject needs to have a parent, Aborting");
            Destroy(this);
        }

        m_parent = transform.parent.GetComponent<TriggerSpikeParent>();
        if(!m_parent) {
            Debug.LogError($"{this.gameObject.name} - {this.GetType()}: This Gameobject parent don't have the TriggerSpikeParent Component, Aborting");
            Destroy(this);
        }

        if(GetComponent<SpriteRenderer>()) {
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    public void Interact() {
        m_parent.GotTriggered();
    } 
}
