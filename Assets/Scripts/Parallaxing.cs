using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxing : MonoBehaviour {
    
    [Header("Parallax Configurations")]
    /* How smooth the parallax is going to be */
    [Range(0, 1)]
    public float parallaxAmount = 1f; 

    /* Array of elements that are going to be Parallaxed */
    private Transform[] m_backgrounds;
    /* Proportion of Camera's movement to move backgrounds */
    private float[] parallaxScales;

    private Transform m_cameraReference;
    private Vector3 m_previousCameraPosition;
    private Vector3 m_tempbackgroundTargetPosition;

    void Awake() {
        m_cameraReference = Camera.main.transform;
        var tempBackgrounds = GameObject.FindGameObjectsWithTag("Parallax");
        m_backgrounds = new Transform[tempBackgrounds.Length];

        for(int i = 0; i < tempBackgrounds.Length; i++) {
            m_backgrounds[i] = tempBackgrounds[i].transform;
        }
    }

    void Start() {
        m_previousCameraPosition = m_cameraReference.position;

        parallaxScales = new float[m_backgrounds.Length];

        for(int i = 0; i < m_backgrounds.Length; i++) {
            parallaxScales[i] = m_backgrounds[i].position.z * - 1;
        }
    }

    void Update() {
        for(int i = 0; i < m_backgrounds.Length; i++) {
            /* The Parallax is the opposite of the camera movement */
            float parallax = (m_previousCameraPosition.x - m_cameraReference.position.x) * parallaxScales[i];

            /* set the target x position, which is the current position + parallax */
            m_tempbackgroundTargetPosition.x = m_backgrounds[i].position.x + (parallax*parallaxAmount);
            m_tempbackgroundTargetPosition.y = m_backgrounds[i].position.y;
            m_tempbackgroundTargetPosition.z = m_backgrounds[i].position.z;

            /* set the target position */
            m_backgrounds[i].position = m_tempbackgroundTargetPosition;
        }

        m_previousCameraPosition = m_cameraReference.position;
    }
}
