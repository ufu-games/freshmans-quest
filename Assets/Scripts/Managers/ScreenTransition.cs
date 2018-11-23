using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTransition : MonoBehaviour, IInteractable {

	private LevelTransition level;
	[HideInInspector]
	public bool Enabled = false;

	void Awake(){
		StartCoroutine(Delay());
	}

	void Start(){
		level = GetComponentInParent<LevelTransition>();
		if(level == null) {
			print("LevelTransition não encontrado, o sistema de transições não vai funcionar!");
		}
	}

    public void Interact()
    {
        if (level.m_player != null && level.m_cam != null && level.m_check != null && Enabled)
        {
            if (!level.m_nowCollider.bounds.Contains(level.m_player.transform.position) && !level.Transitioning && !level.m_check.JustSpawned)
            {
                level.m_nowCollider = this.GetComponent<PolygonCollider2D>();
                StartCoroutine(level.Transition());
            }
        }
        if (level.m_player != null && level.m_cam != null && level.m_check == null && Enabled)
        {
            if (!level.m_nowCollider.bounds.Contains(level.m_player.transform.position) && !level.Transitioning)
            {
                level.m_nowCollider = this.GetComponent<PolygonCollider2D>();
                StartCoroutine(level.Transition());
            }

        }
    }
	public IEnumerator Delay(){
		yield return new WaitForSeconds(.5f);
		Enabled = true;
	}
}
