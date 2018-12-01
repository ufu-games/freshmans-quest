using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTransition : MonoBehaviour, IInteractable, INonHarmfulInteraction {

	private LevelTransition level;
	[HideInInspector]
	public bool Enabled = false;

	[ReadOnly]
	public Vector2 spawnpoint;

	void Awake(){
		StartCoroutine(Delay());
		GetComponentInChildren<SpriteRenderer>().enabled = false;
		level = GetComponentInParent<LevelTransition>();
		if(level == null) {
			print("LevelTransition não encontrado, o sistema de transições não vai funcionar!");
		}

		foreach(Transform t in GetComponentsInChildren<Transform>()) {
			if(t != transform){
				spawnpoint = t.position;
			}
		}
	}

	void Start(){
	}

    public void Interact()
    {
        if (level.m_player != null && level.m_cam != null && Enabled && !level.InColliders.Contains(GetComponent<PolygonCollider2D>())) {
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
		return spawnpoint;
	}
}
