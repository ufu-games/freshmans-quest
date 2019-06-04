using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandMapsManager : MonoBehaviour {
    public Transform lilyHead;
    public Transform[] islands;

    private Vector3 headOffset = new Vector3(0, 2.0f, 0);
    private int m_currentIslandIndex = 0;

    // each island should have => name, position, first time or not, finished or not

    private void Start() {
        lilyHead.transform.position = islands[m_currentIslandIndex].transform.position + headOffset;
    }

    private void Update() {
        // Testing Movements...
        if(Input.GetKeyDown(KeyCode.D)) {
            m_currentIslandIndex = (m_currentIslandIndex + 1) % islands.Length;
            lilyHead.transform.position = islands[m_currentIslandIndex].transform.position + headOffset;
        } else if(Input.GetKeyDown(KeyCode.A)) {
            m_currentIslandIndex = (m_currentIslandIndex - 1) % islands.Length;
            if (m_currentIslandIndex < 0) m_currentIslandIndex = 0;

            lilyHead.transform.position = islands[m_currentIslandIndex].transform.position + headOffset;
        }
    }
}
