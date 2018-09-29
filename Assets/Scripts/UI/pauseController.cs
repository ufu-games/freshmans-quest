using UnityEngine;
using System.Collections;

public class pauseController : MonoBehaviour {

	public Transform uiCanvas;
	public Transform pauseCanvas;
    public Transform menuCanvas;
	public bool pause = false;
    public bool gameStarted = false;

	void Start () {
		pauseCanvas.gameObject.SetActive (false);
	}

	void Update () {

		if (Input.GetButtonDown("Pause")){
            if (gameStarted)
                pause = !pause;
            else
                this.StartGame();
        }

		if (!pause){
			pauseCanvas.gameObject.SetActive(false);
			Time.timeScale = 1;
			uiCanvas.gameObject.SetActive(true);
		}else{
			pauseCanvas.gameObject.SetActive(true);
			Time.timeScale = 0;
			uiCanvas.gameObject.SetActive(false);
		}

        if (!gameStarted)
        {
            Time.timeScale = 0;
        }

	}
    public void continuar()
    {
        pause = false;
    }

    public void StartGame()
    {
        this.gameStarted = true;
        this.menuCanvas.gameObject.SetActive(false);
    }
    public void sair()
    {
        Application.Quit();
    }
}
