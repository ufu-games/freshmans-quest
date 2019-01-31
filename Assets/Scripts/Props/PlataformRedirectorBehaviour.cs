using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlataformRedirectorBehaviour : MonoBehaviour
{

    public enum Direction {Right, Up, Left, Down};
    
    public Direction direction;
    public bool useCustomAngle;
    public float customAngle;
    private float angle;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public float getAngle(){
        if(!useCustomAngle){
            angle = Mathf.PI/2 * (int)direction;
        } else angle = customAngle * Mathf.PI/180;
        return angle;
    }
}

