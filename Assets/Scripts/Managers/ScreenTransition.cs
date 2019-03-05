using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTransition : MonoBehaviour, IInteractable, INonHarmfulInteraction {

	private LevelTransition level;
	[HideInInspector]
	public bool Enabled = false;
	[HideInInspector]
	public List<GameObject> m_resettables;

	private bool checkedStay = false;
	private PolygonCollider2D m_col;

	[ReadOnly]
	public Vector2 spawnpoint;

	void Awake(){
		StartCoroutine(Delay());
		m_col = GetComponent<PolygonCollider2D>();
		m_resettables = new List<GameObject>();
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
        if (level.m_player != null && level.m_cam != null && Enabled && !level.InColliders.Contains(m_col)) {
			level.InColliders.Add(m_col);
        }
    }

	public void InteractWithPlayer(Collider2D col){ //Isso é chamado na entrada e na saida, quero filtrar e pegar somente a saida
		if(!col.IsTouching(m_col)) {	
			if(level.m_player && level.m_cam && Enabled) {
				level.InColliders.Remove(m_col);
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

	public void OnTriggerStay2D(Collider2D col) { // Add os props e os breakable wall da screen na lista m_resettables dela
		if(!Enabled) {
			GameObject go = col.gameObject;
			Debug.LogWarningFormat("Screen Transtion On Trigger Stay 2D: go.name: {0}", go.name);
			if(!m_resettables.Contains(go) && (go.tag == "Prop" || go.tag == "BreakableWall" || go.tag == "Enemy") || go.tag == "Resetable") {
				m_resettables.Add(go);
			}
		}
	}
}
