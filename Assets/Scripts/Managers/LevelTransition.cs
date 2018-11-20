using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class LevelTransition : MonoBehaviour {

	private const int SizeX = 7;
	private const int SizeY = 7;

	public ArrayLayout Level;
	public PolygonCollider2D InitialScreen;
	public float TransitionDuration = 1.5f;
	public float TransitionDamping = 2f;
	[TextArea]
	public string Nota = "Quanto menor o TransitionDamping mais rapido a camera ira se mover para a nova tela.\nCrie Polygon Collider 2D e coloque eles nas areas que você quer que seja cada tela, depois coloque em \"InitialScreen\" o Polygon Collider 2D que indica a primeira tela.";

	[ReadOnly]
	public PolygonCollider2D m_nowCollider;
	[ReadOnly]
	public Vector2Int m_nowCoordinate;
	private GameObject m_player;
	private CinemachineConfiner m_cam;
	private bool Transitioning = false;

	void Start () {
		m_player = GameObject.FindGameObjectWithTag("Player");
		if(m_player == null){
			print("Player não encontrado na cena, A transição de telas não funcionará");
		}
		m_cam = Camera.main.GetComponentInChildren<CinemachineConfiner>();
		if(m_cam == null) {
			print("Camera não encontrada na cena, A transição de telas não funcionará");
		} else {
			m_cam.m_BoundingShape2D = InitialScreen;
			m_cam.InvalidatePathCache();

		}
		m_nowCollider = InitialScreen;
		if(m_nowCollider == null){
			print("Initial Screen vazia, A transição de telas não funcionará");
			this.enabled = false;
		}
		for(int i=0;i<SizeX;i++){
			for(int j=0;j<SizeY;j++){
				if(Level.rows[j].row[i] == InitialScreen){
					m_nowCoordinate.x = i;
					m_nowCoordinate.y = j;
				}
			}
		}
	}
	
	void Update () {
		if(m_player != null && m_cam != null){
			if(!m_nowCollider.bounds.Contains(m_player.transform.position) && !Transitioning) {
				if(m_player.transform.position.y  >  m_nowCollider.bounds.center.y + m_nowCollider.bounds.size.y/2){
					StartCoroutine(Transition(0));
				}
				if(m_player.transform.position.x  >  m_nowCollider.bounds.center.x + m_nowCollider.bounds.size.x/2){
					StartCoroutine(Transition(1));
				}
				if(m_player.transform.position.y  <  m_nowCollider.bounds.center.y - m_nowCollider.bounds.size.y/2){
					StartCoroutine(Transition(2));
				}
				if(m_player.transform.position.x  <  m_nowCollider.bounds.center.x - m_nowCollider.bounds.size.x/2){
					StartCoroutine(Transition(3));
				}
			}
		}
	}

	//Direção começa com 0 no norte e segue no sentido horario
	public IEnumerator Transition(int dir){
		switch(dir) {
			case 0:
				if(m_nowCoordinate.y-1 < 0){
					print("Direção invalida!");
					yield break;
				}
				m_nowCoordinate.y--;
				break;
			case 1:
				if(m_nowCoordinate.x+1 >= SizeX){
					print("Direção invalida!");
					yield break;
				}
				m_nowCoordinate.x++;
				break;
			case 2:
				if(m_nowCoordinate.y+1 >= SizeY){
					print("Direção invalida!");
					yield break;
				}
				m_nowCoordinate.y++;
				break;
			case 3:
				if(m_nowCoordinate.x-1 < 0){
					print("Direção invalida!");
					yield break;
				}
				m_nowCoordinate.x--;
				break;
			default:
				print("Direção invalida!");
				yield break;
		}
		m_player.GetComponent<PlayerController>().enabled = false;
		Transitioning = true;
		m_nowCollider = Level.rows[m_nowCoordinate.y].row[m_nowCoordinate.x];
		m_cam.m_BoundingShape2D = m_nowCollider;
		m_cam.InvalidatePathCache();
		m_cam.m_Damping = 2f;
		yield return new WaitForSeconds(TransitionDuration);
		m_cam.m_Damping = 0;
		m_player.GetComponent<PlayerController>().enabled = true;
		yield return new WaitForSeconds(0.5f);
		Transitioning = false;
	}
}
