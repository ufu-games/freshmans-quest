using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    private bool m_isPaused = false;
    public GameObject pausePanel;
    public TextMeshProUGUI returnToGameText;
    public TextMeshProUGUI optionsText;
    public TextMeshProUGUI backToHubText;
    public TextMeshProUGUI saveAndQuitText;
    public Color selectedColor = Color.yellow;
    public Color notSelectedColor = Color.white;

    enum EPauseMenuOptions {
        Return,
        Options,
        Hub,
        SaveQuit
    }

    private EPauseMenuOptions m_pauseMenuOptions;
    
    /* Input Handling */
    private bool m_canGetInput = true;
    private float m_lastFrameVerticalInput;

    /* Gambiarra das Boas */
    private bool m_canPauseThisFrame = true;


    void Start() {
        m_isPaused = false;
        pausePanel.SetActive(false);
    }

    void Update()
    {
        if(m_isPaused) {
            ProcessPauseMenuInput();
        }

        if(InputManager.instance.PressedStart() && !m_isPaused) {
            TogglePause();
        }

        m_canPauseThisFrame = true;
    }

    private IEnumerator SelectText(TextMeshProUGUI text, Color color) {
        text.color = color;
        yield return null;
    }

    private void ProcessPauseMenuInput() {
        if(!m_canGetInput) return;

        Debug.LogWarning("Pause Input");

        float verticalValue = Mathf.Round(Input.GetAxisRaw("Vertical"));

        /* Impedindo que acontecçam mudancas indesejadas de opcoes */
		if(verticalValue == m_lastFrameVerticalInput) {
			verticalValue = 0;
		} else {
			m_lastFrameVerticalInput = verticalValue;
		}

        switch(m_pauseMenuOptions) {
            case EPauseMenuOptions.Return:
                if(verticalValue == 1) {
                    StartCoroutine(SelectText(saveAndQuitText, selectedColor));
                    StartCoroutine(SelectText(returnToGameText, notSelectedColor));

                    m_pauseMenuOptions = EPauseMenuOptions.SaveQuit;
                } else if(verticalValue == -1) {
                    StartCoroutine(SelectText(returnToGameText, notSelectedColor));
                    StartCoroutine(SelectText(optionsText, selectedColor));

                    m_pauseMenuOptions = EPauseMenuOptions.Options;
                }

                if(InputManager.instance.PressedConfirm()) {
                    Debug.LogWarning("RETURN TO GAME");
                    TogglePause();
                    m_canPauseThisFrame = false;
                }
            break;

            case EPauseMenuOptions.Options:
                if(verticalValue == 1) {
                    StartCoroutine(SelectText(returnToGameText, selectedColor));
                    StartCoroutine(SelectText(optionsText, notSelectedColor));

                    m_pauseMenuOptions = EPauseMenuOptions.Return;
                } else if(verticalValue == -1) {
                    StartCoroutine(SelectText(optionsText, notSelectedColor));
                    StartCoroutine(SelectText(backToHubText, selectedColor));

                    m_pauseMenuOptions = EPauseMenuOptions.Hub;
                }

                if(InputManager.instance.PressedConfirm()) {
                        Debug.LogWarning("OPTIONS");
                }
            break;

            case EPauseMenuOptions.Hub:
                if(verticalValue == 1) {
                    StartCoroutine(SelectText(optionsText, selectedColor));
                    StartCoroutine(SelectText(backToHubText, notSelectedColor));

                    m_pauseMenuOptions = EPauseMenuOptions.Options;
                } else if(verticalValue == -1) {
                    StartCoroutine(SelectText(backToHubText, notSelectedColor));
                    StartCoroutine(SelectText(saveAndQuitText, selectedColor));

                    m_pauseMenuOptions = EPauseMenuOptions.SaveQuit;
                }

                if(InputManager.instance.PressedConfirm()) {
                    Debug.LogWarning("GO TO HUB");
                }
            break;

            case EPauseMenuOptions.SaveQuit:
                if(verticalValue == 1) {
                    StartCoroutine(SelectText(saveAndQuitText, notSelectedColor));
                    StartCoroutine(SelectText(backToHubText, selectedColor));

                    m_pauseMenuOptions = EPauseMenuOptions.Hub;
                } else if(verticalValue == -1) {
                    StartCoroutine(SelectText(saveAndQuitText, notSelectedColor));
                    StartCoroutine(SelectText(returnToGameText, selectedColor));

                    m_pauseMenuOptions = EPauseMenuOptions.Return;
                }

                if(InputManager.instance.PressedConfirm()) {
                    Debug.LogWarning("SAVE AND QUIT");
                }
            break;
        }

    }

    private void TogglePause() {
        if(!m_canPauseThisFrame) return;

        m_isPaused = !m_isPaused;
        Time.timeScale = m_isPaused ? 0 : 1;
        
        if(m_isPaused) {
            m_pauseMenuOptions = EPauseMenuOptions.Return;
            StartCoroutine(SelectText(returnToGameText, selectedColor));
        }

        pausePanel.SetActive(m_isPaused);
    }
}
