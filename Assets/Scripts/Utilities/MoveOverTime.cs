using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOverTime : MonoBehaviour {
    [Range(1,100)]
    public float movingDistance = 5f;
    [Range(0,1)]
    public float movingVelocity = 0.1f;
    private Vector3 m_initialPosition;

    void Start() {
        m_initialPosition = this.transform.position;
    }

    void Update() {
        Vector3 tempPosition = m_initialPosition;
        tempPosition.x += Mathf.Sin(Time.time * movingVelocity) * movingDistance;

        this.transform.position = tempPosition;
    }
}
