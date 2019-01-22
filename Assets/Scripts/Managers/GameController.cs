using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private bool m_isPaused = false;
    public GameObject pausePanel;

    void Start() {
        m_isPaused = false;
        pausePanel.SetActive(false);
    }

    void Update()
    {
        if(InputManager.instance.PressedStart()) {
            TogglePause();
        }
    }

    private void TogglePause() {
        m_isPaused = !m_isPaused;
        Time.timeScale = m_isPaused ? 0 : 1;
        pausePanel.SetActive(m_isPaused);
    }
}
