using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TIGenius : TIBaseState {

	private int[] sequence; 
	private Animator ani;
	private bool waitingPlayer;
	private int currentPosition;
	private bool missed;

	public void ButtonPressed(int i){
		if(i != sequence[currentPosition]){
			waitingPlayer = false;
			this.missed = true;
		} else{
			currentPosition ++;
		}

	}
	public IEnumerator Flow(){
		int j;
		for(int i=3;i<9;i+=2){
			for(j = 0; j < i; j++){
				if(this.sequence[j] == 0){
					ani.Play("Genius0");
				}
				if(this.sequence[j] == 1){
					ani.Play("Genius1");
				}
				yield return new WaitForSeconds(1f);
				ani.Play("Black");
				yield return new WaitForSeconds(0.5f);
			}
			this.waitingPlayer = true;
			this.currentPosition = 0;
			while(waitingPlayer){
				if(missed == true || currentPosition >= i){
					waitingPlayer = false;
				}
			}
			if(missed){
				GetComponent<TIBossBehavior>().DealDMGPlayer(1);
				i -= 2;
			}
		}
		if(missed == false){
			GetComponent<TIBossBehavior>().DealDMGBoss(1);
		}
	}
	public void Create(){
		this.ani = GetComponent<Animator>();
		GetComponent<TIBossBehavior>().FaceTextDeactivate();
		ani.Play("LogicInitialAnimation");
		this.missed = false;
		this.sequence = new int[7];
		for(int i=0;i<7;i++){
			this.sequence[i] = Random.Range(0,2);
		}
		StartCoroutine(Flow());
	}

	public void MyDestroy(){}
}
