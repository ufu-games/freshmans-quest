using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.Audio;

public class CollectableBehavior : MonoBehaviour, IInteractable, IResettableProp {

	public float index = 0;
	public AudioClip collectedClip;
	public AudioClip continuousClip;
	[HideInInspector]
	public bool Collected = false;
	[Header("Follow")]
	[Range(0,100)]
	public float DampingFirstMove = 90;
	public float InitialVelocityFirstMove = 15;
	public float AccelerationSecondMove = 1.25f;
	public float InitialVelocitySecondMove = 3;
	private bool StartedFollowing = false;
	private Vector3 initialPosition;

	void Start() {
		initialPosition = transform.position;
		if(!continuousClip) {
			print("Falta o AudioClip Continuo do Coletável " + this.name);
		} else {
			if(SoundManager.instance) {
				SoundManager.instance.PlayContinuousSfx(SoundManager.instance.Settings.Collectable_continuous,this.gameObject);
			}
		}
	}

	private IEnumerator DestroyCollectableRoutine(PlayerController playerReference) {
		float angleBetweenCollandPlayer = Mathf.Rad2Deg * Mathf.Atan2(transform.position.y - playerReference.transform.position.y,transform.position.x - playerReference.transform.position.x);
		float randomAngle = (angleBetweenCollandPlayer + Random.Range(-30f,30f));
		if(randomAngle < 0) {
			randomAngle += 360;
		}
		if(randomAngle > 360) {
			randomAngle -= 360;
		}
		Rigidbody2D rb = this.GetComponent<Rigidbody2D>();
		if(rb == null) {
			print("Falta Rigidbody no Coletável " + name);
			yield return 0;
		}
		rb.velocity = (new Vector3(Mathf.Cos(Mathf.Deg2Rad*randomAngle),Mathf.Sin(Mathf.Deg2Rad*randomAngle),0))*InitialVelocityFirstMove;
		
		//---------- O coletável é jogado numa direção aleatória e começa a freiar
		InputManager.instance.VibrateWithTime(.1f, 0.1f);
		
		while(Mathf.Abs(rb.velocity.magnitude) > 0.05f) {
			yield return null;
			rb.velocity *= DampingFirstMove/100;
		}

		//---------- Depois de frear ele começa a ir rapidamente para o player

		yield return new WaitForSeconds(0.03f);
		float increasingVel = 0;
		while(Vector3.Distance(transform.position,playerReference.transform.position) > 0.5f) {
			increasingVel += AccelerationSecondMove * Time.deltaTime;
			transform.position = Vector3.Lerp(transform.position, playerReference.transform.position, increasingVel);
			yield return null;
		}

		//---------- O Coletável Chegou no player

		SaveSystem.instance.GotPizza();
		playerReference.UpdatePizzaCounter();
		InputManager.instance.VibrateWithTime(.3f, 0.2f);
		if(collectedClip && SoundManager.instance) {
			SoundManager.instance.PlaySfx(SoundManager.instance.Settings.Collectable_pickup);
		}
		GetComponent<SpriteRenderer>().enabled = false;
		GetComponent<Collider2D>().enabled = false;

		Collected = true;
		rb.velocity = Vector2.zero;
		StudioEventEmitter asource = GetComponent<StudioEventEmitter>();
		if(asource != null) {
			asource.Stop();
			Destroy(asource);
		}
        
		/*
		float ImpactAngle = Mathf.Rad2Deg * Mathf.Atan2(transform.position.y - playerReference.transform.position.y,transform.position.x - playerReference.transform.position.x);
		if(ImpactAngle >= 90 && ImpactAngle <= 270) { // Add uma inclinação para cima, para fazer a particula sair um pouco mais pra cima
			ImpactAngle += 20;
		} else {
			ImpactAngle -= 20;
		}


		part.transform.rotation = Quaternion.Euler(ImpactAngle,-90,0);
		*/
		if(GetComponentInChildren<ParticleSystem>()) { //Para a particula continua
			GetComponentInChildren<ParticleSystem>().Stop();
		}
		GameObject part = Instantiate((GameObject) Resources.Load("Pizza Particles 2"),transform.position,Quaternion.identity);
		part.transform.parent = playerReference.transform; //Começa a particula de estouro
		part.transform.rotation = Quaternion.Euler(-90,0,0);
		part.transform.position += Vector3.down*0.3f;
		part.GetComponent<ParticleSystem>().Play();
	}

	public void Interact(){
		if(!StartedFollowing) {
			StartedFollowing = true;
			PlayerController t_playerControllerReference = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
			StartCoroutine(DestroyCollectableRoutine(t_playerControllerReference));
		}
	}

	public void Reset() {
		if(!GetComponent<AudioSource>()) {
			SoundManager.instance.PlayContinuousSfx(SoundManager.instance.Settings.Collectable_continuous,this.gameObject);
		}
		if(GetComponentInChildren<ParticleSystem>()) {
			GetComponentInChildren<ParticleSystem>().Play();
		}
		
		if(StartedFollowing) {
			transform.position = initialPosition;
			PlayerController playerReference = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
			StartedFollowing = false;
			StopAllCoroutines();
			GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			if(Collected) {
				GetComponent<SpriteRenderer>().enabled = true;
				GetComponent<Collider2D>().enabled = true;
				Collected = false;
				SaveSystem.instance.RemovePizza();
				playerReference.UpdatePizzaCounter();
			}
		}
	}
}
