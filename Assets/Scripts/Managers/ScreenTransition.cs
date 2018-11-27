using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTransition : MonoBehaviour, IInteractable, INonHarmfulInteraction {

	private LevelTransition level;
	[HideInInspector]
	public bool Enabled = false;

	void Awake(){
		StartCoroutine(Delay());
		GetComponentInChildren<SpriteRenderer>().enabled = false;
	}

	void Start(){
		level = GetComponentInParent<LevelTransition>();
		if(level == null) {
			print("LevelTransition não encontrado, o sistema de transições não vai funcionar!");
		}
	}

    public void Interact()
    {
        if (level.m_player != null && level.m_cam != null && Enabled) {
			level.InColliders.Add(GetComponent<PolygonCollider2D>());
        }
    }

	public void InteractWithPlayer(Collider2D col){ //Isso é chamado na entrada e na saida, quero filtrar e pegar somente a saida
		if(!col.IsTouching(GetComponent<PolygonCollider2D>())) {	
			if(level.m_player && level.m_cam && Enabled) {
				level.InColliders.Remove(GetComponent<PolygonCollider2D>());
			}
		}
	}

	public IEnumerator Delay(){
		yield return new WaitForSeconds(.5f);
		Enabled = true;
	}

	public Vector2 GetSpawnPoint(){
		List<Vector2> l = new List<Vector2>();
		foreach(Transform t in GetComponentsInChildren<Transform>()) {
			if(t != transform){
				return t.position;
			}
		}
		return transform.position;
	}
}
