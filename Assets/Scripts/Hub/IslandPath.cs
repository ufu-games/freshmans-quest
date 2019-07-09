using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IslandPath {
    public IslandData.EIsland origin;
    public IslandData.EIsland destination;
    public IslandMapsManager.EPlayerAnimation playerAnimation;
    public Transform[] path;
}
