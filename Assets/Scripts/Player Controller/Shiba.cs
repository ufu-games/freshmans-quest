using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shiba : MonoBehaviour, IPetable {
    private void StartPet() {
        SoundManager.instance.ChangeMusic(SoundManager.instance.Settings.ShibasTheme);
    }
    
    public void Pet() {
        Debug.Log("You are now petting the dog :)");
        StartPet();
    }

    public void StopPetting() {
        Debug.Log("You are no longer petting the dog :(");
    }
}
