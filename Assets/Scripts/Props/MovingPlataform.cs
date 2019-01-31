using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlataform : MonoBehaviour
{
    public enum Direction {Right, Up, Left, Down};

    public Direction direction;
    public bool useCustomAngle;
    public float customAngle;
    public float speed;

    private float directionAngle;
    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        if(!useCustomAngle){
            directionAngle = Mathf.PI/2 * (int)direction;
        } else directionAngle = customAngle * Mathf.PI/180;

        rb = GetComponent<Rigidbody2D>(); 
    }
    public float getAngle(){
        return directionAngle;
    }
    // Update is called once per frame
    void Update()
    {
        var newPosition = transform.position; 

        newPosition.x += Mathf.Cos(directionAngle) * speed;
        newPosition.y += Mathf.Sin(directionAngle) * speed;

        transform.position = newPosition;
    }

    void OnTriggerEnter2D(Collider2D col){
        if(col.gameObject.layer == LayerMask.NameToLayer("PlataformRedirector")){
            directionAngle = col.gameObject.GetComponent<PlataformRedirectorBehaviour>().getAngle();
        }
    }
    void OnCollisionStay2D(Collision2D col){
        Debug.Log("colidindo com : " + col.gameObject.name);
    }

}
