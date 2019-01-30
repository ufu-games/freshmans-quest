using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ParallaxEffect : MonoBehaviour {
    
    [Range(0,100)]
    public float imageSpeed = 5;
    private float m_imageSpeed;
    private Transform m_playerTransform;
    private Transform m_thisTransform;
    private float m_playerInitialPosition;
    private Vector3 t_vector;
    private Transform[] m_transformInChildren;

    void Start() {
        m_playerTransform = GameObject.FindWithTag("Player").transform;
        m_thisTransform = this.transform;
        m_playerInitialPosition = m_playerTransform.position.x;
        m_imageSpeed = imageSpeed / 100;

        m_transformInChildren = GetComponentsInChildren<Transform>(false).Where(x => x.gameObject.transform.parent != transform.parent).ToArray();
    }

    void Update() {
        /* The Relative Position and Moving Objects Shouldn't be the same! */
        t_vector.x = m_thisTransform.position.x + -m_imageSpeed * (m_playerTransform.position.x - m_playerInitialPosition);
		t_vector.y = m_thisTransform.position.y;
		t_vector.z = 1;

        // m_thisTransform.position = t_vector;
        foreach(Transform t in m_transformInChildren) {
            t_vector.x = -m_imageSpeed * (m_playerTransform.position.x - m_playerInitialPosition);
            t_vector.y = t.position.y;
            t_vector.z = t.position.z;

            t.position = t_vector;
        }
    }
}
