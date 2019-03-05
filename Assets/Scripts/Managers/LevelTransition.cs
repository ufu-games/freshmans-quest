using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class LevelTransition : MonoBehaviour {

	public PolygonCollider2D InitialScreen;
	public float TransitionDuration = 1.5f;
	public float TransitionDamping = 2f;
	[TextArea]
	public string Notas = "Quanto menor o TransitionDamping mais rapido a camera ira se mover para a nova tela.\nCrie Screens (É um prefab) e coloque eles como filho desse LevelTransition e edite os seus Polygon Colliders 2D, depois coloque em \"InitialScreen\" a Screeen que indica a primeira tela.";

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
	[HideInInspector]
	public List<PolygonCollider2D> InColliders;

	public float timeToTransitionScreenSize = 1;
	private UnityEngine.U2D.PixelPerfectCamera[] PixelPerfectCameralist;
	private Vector2Int m_referenceResolution;
	private bool destroyOtherCoroutines = false;
	private float activeZoom = 1;

	void Start () {
		m_player = GameObject.FindGameObjectWithTag("Player");
		m_referenceResolution = new Vector2Int(FindObjectOfType<UnityEngine.U2D.PixelPerfectCamera>().refResolutionX,FindObjectOfType<UnityEngine.U2D.PixelPerfectCamera>().refResolutionY);
		
		PixelPerfectCameralist = FindObjectsOfType<UnityEngine.U2D.PixelPerfectCamera>();
		
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
			m_cam.InvalidatePathCache(); // O InvalidadePathCache serve pra forçar a camera a refazer as limitações do Bounding Shape
		}

		m_nowCollider = InitialScreen;
		
		if(m_nowCollider == null){
			print("Initial Screen vazia, A transição de telas não funcionará");
			this.enabled = false;
		} else {
			if(!InColliders.Contains(m_nowCollider)) {
				InColliders.Add(m_nowCollider);
				activeZoom = m_nowCollider.GetComponent<ScreenTransition>().screenReferenceZoom;
			}
		}
		
		GameObject check = GameObject.FindGameObjectWithTag("Checkpoint System");
		
		if(check == null) {
			print("Checkpoint System não encontrada na cena, Os perigos da cena não funcionarão");
		} else {
            m_check = check.GetComponent<CheckpointSystemBehavior>();
			m_check.LastCheckpoint = m_nowCollider.gameObject.GetComponent<ScreenTransition>().GetSpawnPoint();
        }
	}

	void Update(){
		if(InColliders.Count == 1 && !InColliders.Contains(m_nowCollider)) {
			bool Allowed;
			if(m_check) {
				if(m_check.JustSpawned) {
					Allowed = false;
				} else {
					Allowed = true;
				}
				m_check.LastCheckpoint = InColliders[0].gameObject.GetComponent<ScreenTransition>().GetSpawnPoint();
			} else {
				Allowed = true;
			}
			if (!Transitioning && Allowed) {
				// Removendo Coletaveis e Breakable walls da lista de resetáveis da screen atual 
				//, para evitar que eles sejam respawnadas caso o player sai e volte na sala e morra
				foreach(GameObject go in m_nowCollider.GetComponent<ScreenTransition>().m_resettables) { 
					if(go.GetComponent<CollectableBehavior>() != null) {
						CollectableBehavior collect = go.GetComponent<CollectableBehavior>();
						if(collect.Collected) {
							m_nowCollider.GetComponent<ScreenTransition>().m_resettables.Remove(go);
						}
					}
					if(go.tag == "BreakableWall") {
						if(go.GetComponent<SpriteRenderer>().enabled == false) {
							m_nowCollider.GetComponent<ScreenTransition>().m_resettables.Remove(go);
						}
					}
				}
				m_nowCollider = InColliders[0].gameObject.GetComponent<PolygonCollider2D>();
				StartCoroutine(Transition());
			}
		}
	}

	public IEnumerator Transition(){
		m_player.GetComponent<PlayerController>().enabled = false;
		Transitioning = true;
		m_cam.m_BoundingShape2D = m_nowCollider;
		m_cam.InvalidatePathCache(); // Denovo o Invalidade
		m_cam.m_Damping = TransitionDamping; //Esse Damping aqui é somente para fazer a transição ser suave
		destroyOtherCoroutines = true;
		yield return new WaitForSeconds(TransitionDuration);
		destroyOtherCoroutines = false;
		m_cam.m_Damping = 0; //É necessário que o damping volte a ser zero para que a camera respeite com rigidez o Bounding Shape
		m_player.GetComponent<PlayerController>().enabled = true;

		StartCoroutine(ScreenResolutionLerp());

		//yield return new WaitForSeconds(0.5f);
		Transitioning = false;
	}

	public void SetSpawnPoint() {
		m_check.LastCheckpoint = InColliders[0].GetComponent<ScreenTransition>().spawnpoint	= InColliders[0].gameObject.GetComponent<ScreenTransition>().GetSpawnPoint();
	}

	void OnDestroy() {
		foreach(UnityEngine.U2D.PixelPerfectCamera m_pixelPerfectCamera in PixelPerfectCameralist){
			m_pixelPerfectCamera.refResolutionX = m_referenceResolution.x;
			m_pixelPerfectCamera.refResolutionY = m_referenceResolution.y;
		}
	}

	IEnumerator ScreenResolutionLerp() {
		float t_referenceResolution = m_nowCollider.GetComponent<ScreenTransition>().screenReferenceZoom;
		float elapsedTime = Time.deltaTime;
		float thisZoom = activeZoom;
		
		if(Mathf.Approximately(activeZoom,t_referenceResolution)) {
			yield break;
		}

		while(elapsedTime < timeToTransitionScreenSize) {
			if(destroyOtherCoroutines) {
				activeZoom = thisZoom;
				yield break;
			}
			thisZoom = Mathf.Lerp(activeZoom,t_referenceResolution,elapsedTime/timeToTransitionScreenSize);
			foreach(UnityEngine.U2D.PixelPerfectCamera m_pixelPerfectCamera in PixelPerfectCameralist){
				m_pixelPerfectCamera.refResolutionX = Mathf.RoundToInt(thisZoom* m_referenceResolution.x);
				m_pixelPerfectCamera.refResolutionY = Mathf.RoundToInt(thisZoom* m_referenceResolution.y);
			}
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		foreach(UnityEngine.U2D.PixelPerfectCamera m_pixelPerfectCamera in PixelPerfectCameralist){
			m_pixelPerfectCamera.refResolutionX = Mathf.RoundToInt(t_referenceResolution* m_referenceResolution.x - (t_referenceResolution* m_referenceResolution.x % 16));
			m_pixelPerfectCamera.refResolutionY = Mathf.RoundToInt(t_referenceResolution* m_referenceResolution.y - (t_referenceResolution* m_referenceResolution.y % 9));
		}
		activeZoom = t_referenceResolution;
	}
}
