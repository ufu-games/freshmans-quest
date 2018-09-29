using UnityEngine;
using System.Collections;

public class pauseController : MonoBehaviour {

	public Transform uiCanvas;
	public Transform pauseCanvas;
	public bool pause = false;

	void Start () {
		pauseCanvas.gameObject.SetActive (false);
	}

	void Update () {

		if (Input.GetKeyDown (KeyCode.Escape)) 
			pause = !pause;


		if (!pause){
			pauseCanvas.gameObject.SetActive(false);
			Time.timeScale = 1;
			uiCanvas.gameObject.SetActive(true);
		}else{
			pauseCanvas.gameObject.SetActive(true);
			Time.timeScale = 0;
			uiCanvas.gameObject.SetActive(false);
		}

	}
	public void continuar (){
		pause = false;
	}
	public void sair(){
		Application.Quit ();
	}
}
