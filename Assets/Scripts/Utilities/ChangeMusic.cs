using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMusic : MonoBehaviour, IInteractable {
    public AudioClip musicToChange;
    private bool m_hasChangedMusic;

    void Awake() {
        if(musicToChange == null) {
            Debug.LogWarning("Script ChangeMusic sem musica atribuida!");
        }

        if(SoundManager.instance == null) {
            Debug.LogWarning("Não há SoundManager na cena!");
        } 

        m_hasChangedMusic = false;
    }
    void IInteractable.Interact() {
        if(m_hasChangedMusic) return;
        
        SoundManager.instance.ChangeMusic(musicToChange);
        m_hasChangedMusic = true;
    }
}
