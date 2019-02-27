using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour, IResettableProp
{
    public enum Direction {Right, Left, Stoped};

    public Direction InitialDirection;
    public float speed;
    public bool FollowPlayer;
    public float MinDistanceToFollowPlayer;

    private Rigidbody2D rb;
    private float realDirection;
    private Vector2 initialPosition;
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        this.initialPosition = transform.position;

        if(InitialDirection == Direction.Right)realDirection = 1;
        if(InitialDirection == Direction.Left)realDirection = -1;
        if(InitialDirection == Direction.Stoped)realDirection = 0;

        player = GameObject.Find("Player");

    }
    public float getDirection(){
        return realDirection;
    }

    // Update is called once per frame
    void Update()
    {
        var newPosition = transform.position;
        var playerPos = player.GetComponent<Transform>().position;
        if(FollowPlayer == true && Mathf.Abs(transform.position.x - playerPos.x) < MinDistanceToFollowPlayer) {
            if(transform.position.x > playerPos.x ){
                if(transform.position.x - speed > playerPos.x)
                    newPosition.x -= speed;
                else
                    newPosition.x = playerPos.x;             
            } else {
                if(transform.position.x + speed < playerPos.x)
                    newPosition.x += speed;
                else
                    newPosition.x = playerPos.x; 
            }
            
        } else {
            newPosition.x += this.realDirection * speed;
        }

        UpdateLocalScale(transform.position - playerPos);
        transform.position = newPosition;
        
    }

    private void UpdateLocalScale(Vector3 movement) {
        transform.localScale = new Vector3(Mathf.Sign(movement.x) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    void OnTriggerEnter2D(Collider2D col){
        if(col.gameObject.layer == LayerMask.NameToLayer("PlayerRedirector")){
            this.realDirection = col.gameObject.GetComponent<EnemyRedirectioner>().getAngle();
        }
    }

    public void Reset() {
		Debug.Log("resetando");
		transform.position = initialPosition;
        if(InitialDirection == Direction.Right)realDirection = 1;
        if(InitialDirection == Direction.Left)realDirection = -1;
        if(InitialDirection == Direction.Stoped)realDirection = 0;
	}
}
