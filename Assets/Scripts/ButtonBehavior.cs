using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBehavior : MonoBehaviour {

    public Transform targetD;
    public Transform targetU;
    public float speed;
    public int Index;
    public GameObject Boss;
    

    void OnTriggerEnter2D(Collider2D other)
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetD.position, step);
        Boss.GetComponent<TIBossBehavior>().ButtonPressed(Index);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetU.position, step);
    }
}
