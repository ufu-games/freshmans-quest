using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TIBossBehavior : MonoBehaviour {

	public TIBaseState MyState;

	void Start () {
		MyState = new TIGenius();
	}

	void Update () {

	}
}
