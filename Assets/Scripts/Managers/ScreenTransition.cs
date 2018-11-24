using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTransition : MonoBehaviour, IInteractable {

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
			
			bool Allowed;
			if(level.m_check) {
				if(level.m_check.JustSpawned) {
					Allowed = false;
				} else {
					Allowed = true;
				}
				level.m_check.LastCheckpoint = GetSpawnPoint();
			} else {
				Allowed = true;
			}
            if (!level.Transitioning && Allowed) {
                level.m_nowCollider = this.GetComponent<PolygonCollider2D>();
                StartCoroutine(level.Transition());
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
