using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MonsterTrigger : MonoBehaviour {

	public GameObject player;
	public MonsterAI monster;
	public Vector3 appearPosition;

	private Renderer rend;
	private Collider m_Collider;

	// Use this for initialization
	void Start () {

		m_Collider = GetComponent<Collider>();
		m_Collider.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other) {
		if(other.tag == player.tag){ 
			//rend.material.SetColor("_Color", Color.green);
			//appearPosition = transform.position 
			monster.setState(MonsterAI.State.APPEAR);
			monster.setPostion (appearPosition);
			m_Collider.enabled = false;
		}
	}
}