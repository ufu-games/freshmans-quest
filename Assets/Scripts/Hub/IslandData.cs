using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandData : MonoBehaviour {
    public enum EIsland {
        University,
        Arts,
        Medicine,
        Chemistry,
        Philosophy
    }

    // User Interface Related
    public EIsland island;
    public string islandName;
    public Transform playerPositionOnIsland;

    // Whether player has or has not played or finished the level
    public bool hasPlayedBefore;
    public bool hasFinishedLevel;

    // Level Data
    public float playerBestScoreOnLevel;
    public int collectableAmount;
    public int collectablesCollected;
    // someway to track which collectables were collected?!
}
