using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class LevelTransition : MonoBehaviour {

	public PolygonCollider2D InitialScreen;
	public float TransitionDuration = 1.5f;
	public float TransitionDamping = 2f;
	[TextArea]
	public string Nota = "Quanto menor o TransitionDamping mais rapido a camera ira se mover para a nova tela.\nCrie Polygon Collider 2D e coloque eles nas areas que você quer que seja cada tela, depois coloque em \"InitialScreen\" o Polygon Collider 2D que indica a primeira tela.";

	[ReadOnly]
	public PolygonCollider2D m_nowCollider;
	[HideInInspector]
	public GameObject m_player;
	[HideInInspector]
	public CinemachineConfiner m_cam;
	[HideInInspector]
	public bool Transitioning = false;
	[HideInInspector]
	public CheckpointSystemBehavior m_check;

	void Start () {
		m_player = GameObject.FindGameObjectWithTag("Player");
		if(m_player == null){
			print("Player não encontrado na cena, A transição de telas não funcionará");
		}
		m_cam = Camera.main.GetComponentInChildren<CinemachineConfiner>();
		if(m_cam == null) {
			print("Camera não encontrada na cena, A transição de telas não funcionará");
		} else {
			m_cam.gameObject.GetComponent<CinemachineVirtualCamera>().Follow = m_player.transform;
			m_cam.m_ConfineScreenEdges = true;
			m_cam.m_Damping = 0;
			m_cam.m_BoundingShape2D = InitialScreen;
			m_cam.InvalidatePathCache();

		}
		m_nowCollider = InitialScreen;
		if(m_nowCollider == null){
			print("Initial Screen vazia, A transição de telas não funcionará");
			this.enabled = false;
		}
		GameObject check = GameObject.FindGameObjectWithTag("Checkpoint System");
		if(m_check == null) {
			print("Checkpoint System não encontrada na cena, Os perigos da cena não funcionarão");
		} else {
            m_check = check.GetComponent<CheckpointSystemBehavior>();
        }
	}

	public IEnumerator Transition(){
		m_player.GetComponent<PlayerController>().enabled = false;
		Transitioning = true;
		m_cam.m_BoundingShape2D = m_nowCollider;
		m_cam.InvalidatePathCache();
		m_cam.m_Damping = TransitionDamping;
		yield return new WaitForSeconds(TransitionDuration);
		m_cam.m_Damping = 0;
		m_player.GetComponent<PlayerController>().enabled = true;
		yield return new WaitForSeconds(0.5f);
		m_check.LastCheckpoint = m_player.transform.position;
		Transitioning = false;
	}
}
