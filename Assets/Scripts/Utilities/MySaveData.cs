using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MySaveData
{   
    [ReadOnly]
    public int Jumps = 0;
    [ReadOnly]
    public int Deaths = 0;
    [ReadOnly]
    public float timePlayed = 0;
    [ReadOnly]
    public int pizzaCounter = 0;

    [ReadOnly]
    public int lastStage = 0;
    [ReadOnly]
    public bool isInStage = false;

    //Hidden Stats

    [HideInInspector]
    public int NPCChat = 0;

    public void Reset() {
        Jumps = 0;
        Deaths = 0;
        timePlayed = 0;
        pizzaCounter = 0;
        lastStage = 0;
        isInStage = false;
        NPCChat = 0;
    }
}
