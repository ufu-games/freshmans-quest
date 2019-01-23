using UnityEngine;

public class KillBehavior : MonoBehaviour, IInteractable
{
    public void Interact() {
        FindObjectOfType<CheckpointSystemBehavior>().ResetPlayer();
    }
}
