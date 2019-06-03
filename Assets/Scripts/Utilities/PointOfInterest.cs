using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointOfInterest : MonoBehaviour, IInteractable, INonHarmfulInteraction
{
    [HideInInspector]
    public Transform ThePointOfInterest = null;
    
    PolygonCollider2D m_col = null;

    
    void Awake()
    {
        m_col = GetComponent<PolygonCollider2D>();
        if(!m_col) {
            Debug.LogError($"{this.name} - {this.GetType()}: This point of interest is missing a PolygonCollider2D.");
            Destroy(this);
        }
        ThePointOfInterest = transform.GetChild(0);
        ThePointOfInterest.GetComponent<SpriteRenderer>().enabled = false;
    }

    public void Interact()
    {
        if(CamTarget.instance) {
            CamTarget.instance.AddPointInView(this);
        }
    }

	public void InteractWithPlayer(Collider2D col){ //Isso é chamado na entrada e na saida, quero filtrar e pegar somente a saida
		if(!col.IsTouching(m_col)) {	
			if(CamTarget.instance) {
                CamTarget.instance.RemovePointInView(this);
            }
		}
	}

}
