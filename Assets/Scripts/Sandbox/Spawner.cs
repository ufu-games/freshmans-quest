using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

	public GameObject[] enemies;
	public float spawnTime = 4.5f;
	private float m_timeSpent;

	GameObject GetRandomEnemy() {
		return enemies[Random.Range(0, (enemies.Length - 1))];
	}

	void Update() {
		m_timeSpent += Time.deltaTime;

		if(m_timeSpent >= spawnTime) {
			m_timeSpent = 0f;
			Instantiate(GetRandomEnemy(), transform.position, Quaternion.identity);
		}
	}
}
