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
    /// <summary>
	/// mask with all layers that the player should interact with
	/// </summary>
    public LayerMask platformMask = 0;
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

            UpdateLocalScale(transform.position - playerPos);
        } else {
            newPosition.x += this.realDirection * speed;
            // invertendo a direcao pq o padrao do sprite do inimigo é olhando para a esquerda
            UpdateLocalScale(new Vector3(-this.realDirection, 0, 0));
        }
        var movementDirection = Mathf.Sign(newPosition.x - transform.position.x);
        var hit = Physics2D.Raycast(new Vector2(newPosition.x + 0.5f * movementDirection,newPosition.y - 0.5f), Vector2.down,1,platformMask);
        if(hit.collider != null){
            transform.position = newPosition;
        }else{
            this.realDirection *= -1;
        }
        
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
		transform.position = initialPosition;
        if(InitialDirection == Direction.Right)realDirection = 1;
        if(InitialDirection == Direction.Left)realDirection = -1;
        if(InitialDirection == Direction.Stoped)realDirection = 0;
	}
}
