using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimentoInimigoVoador : MonoBehaviour, IResettableProp
{

    //the starting relative position to the player
    private int StartRelativePosition;
    private bool idle = true;
    private float yTarget;
    private GameObject player;
    public float speed;
    private Vector2 initialPosition;
    // Start is called before the first frame update
    void Start()
    {
        this.initialPosition = transform.position;
        this.player = GameObject.Find("Player");
        var playerPos = this.player.GetComponent<Transform>().position;
        this.StartRelativePosition = (int)Mathf.Sign(transform.position.x - playerPos.x);
    }

    // Update is called once per frame
    void Update()
    {
        if(this.idle){
            if(Mathf.Sign(transform.position.x - this.player.GetComponent<Transform>().position.x) != this.StartRelativePosition){
                this.idle = false;
                this.yTarget = this.player.GetComponent<Transform>().position.y + Random.Range(-0.25f, 0.25f);
                Debug.Log(this.yTarget);
            }
        } else{
            var newPosition = transform.position;
            if(transform.position.y != this.yTarget){
                if(Mathf.Abs(transform.position.y - this.yTarget) < this.speed){
                  newPosition.y = this.yTarget;  
                }else{
                    newPosition.y += Mathf.Sign(this.yTarget - transform.position.y) * speed;
                }
            }else{
                newPosition.x += speed * Mathf.Sign(this.player.GetComponent<Transform>().position.x - transform.position.x);
            }
            this.transform.position = newPosition;
        }
    }
    public void Reset() {
		Debug.Log("resetando");
		transform.position = initialPosition;
        idle = true;
        var playerPos = this.player.GetComponent<Transform>().position;
        this.StartRelativePosition = (int)Mathf.Sign(transform.position.x - playerPos.x);
	}
}
