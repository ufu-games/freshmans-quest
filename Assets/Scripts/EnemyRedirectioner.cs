using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRedirectioner : MonoBehaviour
{
    public enum Direction {Right, Left, Stop};
    
    public Direction direction;
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
        if(direction == Direction.Right)return 1;
        if(direction == Direction.Left)return -1;
        if(direction == Direction.Stop)return 0;
        return 0;
    }
}
