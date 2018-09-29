using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBehaviour : MonoBehaviour {
	public GameObject galho;
    private HealthManager m_healthManager;
    // Use this for initialization
    void Start () {
        m_healthManager = GetComponent<HealthManager>();
    }
	
	// Update is called once per frame
	void Update () {
        if (this.m_healthManager.Hp <= 0)
            Destroy(gameObject);
	}

	public void changeOrientation(){
		this.galho.GetComponent<BehaviourGalho>().orientation *= -1;
	}

	public void fastRotation(){
		this.galho.GetComponent<BehaviourGalho>().SetHighSpeed();
	}

    public void UpSpeed()
    {
        this.galho.GetComponent<BehaviourGalho>().UpSpeed();
    }
}
