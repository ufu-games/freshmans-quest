using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversidadeSoundScript : MonoBehaviour
{
    private GameObject player;
    public SoundManager soundManager;
    float value;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if(player.transform.position.y >= 17) {
            value = 1;
        } else { //-19 to 95
            value = (player.transform.position.x + 19f)/(114f);
        }
        soundManager.SetParameterFMOD("Distance",value*100);
    }
}
