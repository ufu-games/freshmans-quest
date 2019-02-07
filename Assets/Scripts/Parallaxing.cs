using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxing : MonoBehaviour {
    
    [Header("Parallax Configurations")]
    /* How smooth the parallax is going to be */
    [Range(0, 1)]
    public float parallaxAmount = 1f; 

    /* Array of elements that are going to be Parallaxed */
    public Transform[] backgrounds;
    /* Proportion of Camera's movement to move backgrounds */
    private float[] parallaxScales;

    private Transform m_cameraReference;
    private Vector3 m_previousCameraPosition;

    void Awake() {
        m_cameraReference = Camera.main.transform;
    }

    void Start() {
        m_previousCameraPosition = m_cameraReference.position;

        parallaxScales = new float[backgrounds.Length];

        for(int i = 0; i < backgrounds.Length; i++) {
            parallaxScales[i] = backgrounds[i].position.z * - 1;
        }
    }

    void Update() {
        for(int i = 0; i < backgrounds.Length; i++) {
            /* The Parallax is the opposite of the camera movement */
            float parallax = (m_previousCameraPosition.x - m_cameraReference.position.x) * parallaxScales[i];

            /* set the target x position, which is the current position + parallax */
            float backgroundTargetPositionX = backgrounds[i].position.x + parallax;

            Vector3 backgroundTargetPosition = new Vector3(backgroundTargetPositionX, backgrounds[i].position.y, backgrounds[i].position.z);

            /* Lerp between current position and the target position */
            backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, backgroundTargetPosition, parallaxAmount * Time.deltaTime);
        }

        m_previousCameraPosition = m_cameraReference.position;
    }
}
