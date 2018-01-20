using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MonsterTrigger : MonoBehaviour {

	public GameObject player;
	//public GameObject monster;
	public MonsterAI monster;
	private Renderer rend;
	private Collider m_Collider;

	// Use this for initialization
	void Start () {
		rend = GetComponent<Renderer>();
		m_Collider = GetComponent<Collider>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider player) {
		monster.setState(MonsterAI.State.APPEAR);
		m_Collider.enabled = false;
		rend.enabled = false;
//		rend.material.SetColor("_Color", Color.green);
	}
}