using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBehavior : MonoBehaviour {

    public Transform targetD;
    public Transform targetU;
    public float speed;
    

    void OnTriggerEnter2D(Collider2D other)
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetD.position, step);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetU.position, step);
    }
}
