using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TIPong : TIBaseState {

	public GameObject BossBar;
	public GameObject PlayerBar;
	public GameObject Ball;
	public Vector2 bossbar_offset;
	public Vector2 playerbar_offset;
	public Vector2 ball_offset;
	public float bar_speed;
	public float ball_speed;
	private bool Pressed1;
	private bool Pressed3;
	private int RoundsWon = 0;
	private bool BallHitted;


	public override void ButtonPressed(int i) {
		if(i==1){
			Pressed1 = true;
		}

		if(i==3) {
			Pressed3 = true;
		}
	}

	public override void ButtonUnpressed(int i) {
		if(i==1){
			Pressed1 = false;
		}

		if(i==3) {
			Pressed3 = false;
		}
	}

	public void Update() {
		if(Pressed1) {
			MoveBar(PlayerBar,Vector2.left * bar_speed);
		}
		if(Pressed3) {
			MoveBar(PlayerBar,Vector2.right * bar_speed);
		}

		if(!Pressed1 && !Pressed3) {
			PlayerBar.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		}

		if(Mathf.Abs(Ball.transform.position.y - 2.45f) > 3.35f){
			BallHitted = true;
		}
	}

	public override void Create(){
		GetComponent<TIBossBehavior>().FaceTextDeactivate();
		Ball = (GameObject) Instantiate(Resources.Load("Ball"));
		Ball.transform.position = this.transform.position + (Vector3) ball_offset;
		BossBar = (GameObject) Instantiate(Resources.Load("Bar"));
		BossBar.transform.position = this.transform.position + (Vector3) bossbar_offset;
		BossBar.GetComponent<EdgeCollider2D>().offset = Vector2.down * 0.5f;
		BossBar.GetComponent<PlatformEffector2D>().rotationalOffset = 180;
		PlayerBar = (GameObject) Instantiate(Resources.Load("Bar"));
		PlayerBar.transform.position = this.transform.position + (Vector3) playerbar_offset;
		PlayerBar.GetComponent<EdgeCollider2D>().offset = Vector2.up * 0.5f;

		GetComponent<TIBossBehavior>().ButtonActivate(1,Color.white);
		GetComponent<TIBossBehavior>().ButtonActivate(3,Color.white);
		StartCoroutine(Flow());
	}

	public override void MyDestroy(){
		GetComponent<TIBossBehavior>().ButtonDeactivate(1);
		GetComponent<TIBossBehavior>().ButtonDeactivate(3);
		Destroy(BossBar);
		Destroy(PlayerBar);
		Destroy(Ball);
		Destroy(this);
	}

	public IEnumerator Flow(){
		for(int i=0;i<3;i++){
			BallHitted = false;
			Ball.transform.position = this.transform.position + (Vector3)ball_offset;
			Ball.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			BossBar.transform.position = this.transform.position + (Vector3)bossbar_offset;
			BossBar.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			PlayerBar.transform.position = this.transform.position + (Vector3)playerbar_offset;
			PlayerBar.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			yield return new WaitForSeconds(2.5f);
			float random_angle;
			if(Random.Range(0,2) == 0) { //Atirar pra cima
				random_angle = Random.Range(-40f,40f) + 90;
			} else { //Atirar pra baixo
				random_angle = Random.Range(-40f,40f) - 90;
			}
			Ball.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Cos((random_angle) * Mathf.Deg2Rad) * ball_speed,Mathf.Sin((random_angle) * Mathf.Deg2Rad) * ball_speed);
			while(!BallHitted){
				if(Ball.transform.position.x > BossBar.transform.position.x) {
					MoveBar(BossBar, Vector2.right * bar_speed * 0.6f);
				}
				if(Ball.transform.position.x < BossBar.transform.position.x) {
					MoveBar(BossBar, Vector2.left * bar_speed * 0.6f);
				}
				if(BossBar.transform.position.x + 0.1f > Ball.transform.position.x && BossBar.transform.position.x - 0.1f < Ball.transform.position.x) {
					BossBar.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
				}
				yield return null;
			}
			if(Ball.transform.position.y > this.transform.position.y) {
				RoundsWon++;
			}

			
		}
		if(RoundsWon >= 2) {
			GetComponent<TIBossBehavior>().DealDMGBoss(1);
		} else {
			GetComponent<TIBossBehavior>().DealDMGPlayer(1);
		}
	}

	public void MoveBar(GameObject bar, Vector2 direction){
		if(bar.GetComponent<Rigidbody2D>().transform.position.x + direction.x*Time.deltaTime > 4.3f || bar.GetComponent<Rigidbody2D>().transform.position.x + direction.x*Time.deltaTime < -3.8f) {
			bar.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
		} else {
			bar.GetComponent<Rigidbody2D>().velocity = direction;
		}
	}
}
