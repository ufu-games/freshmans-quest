using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillOnCollision : MonoBehaviour, IDangerous {
    public bool isDangerous = true;

    void IDangerous.InteractWithPlayer(Collider2D player) {
        if(isDangerous) {
            GameObject.FindGameObjectWithTag("Checkpoint System").GetComponent<CheckpointSystemBehavior>().ResetPlayer();
        }
    }
}
