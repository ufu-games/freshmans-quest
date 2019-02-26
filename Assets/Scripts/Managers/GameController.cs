using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class GameController : MonoBehaviour
{
    private bool m_isPaused = false;
    public GameObject pausePanel;
    public GameObject returnToGameText;

    private bool m_canPauseThisFrame = true;
    private EventSystem m_currentEventSystem;


    void Start() {
        m_isPaused = false;
        pausePanel.SetActive(false);
        m_currentEventSystem = EventSystem.current;
    }

    void Update()
    {
        if(InputManager.instance.PressedStart() && !m_isPaused) {
            TogglePause();
        }

        m_canPauseThisFrame = true;
    }

    private void PauseGame() {
        pausePanel.SetActive(true);
        m_isPaused = true;
        Time.timeScale = 0;
        m_currentEventSystem.SetSelectedGameObject(returnToGameText);
    }

    public void UnpauseGame() {
        m_currentEventSystem.SetSelectedGameObject(null);
        pausePanel.SetActive(false);
        m_isPaused = false;
        Time.timeScale = 1;
    }

    public void SaveAndQuit() {
        SaveSystem.instance.UISaveGame();
        LevelManagement.LevelManager.instance.LoadLevel(0);
    }

    private void TogglePause() {
        if(!m_canPauseThisFrame) return;

        if(!m_isPaused) {
            PauseGame();
        }
    }
}
