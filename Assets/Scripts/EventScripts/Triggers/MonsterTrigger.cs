using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MonsterTrigger : MonoBehaviour {

    public delegate void HumanDetected(Transform human);
    public static event HumanDetected OnHumanDetected;

    public GameObject player;
	public MonsterAI monster;
	private Vector3 appearPosition;
	private Renderer rend;
	private Collider m_Collider;

	// Use this for initialization
	void Start () {
        appearPosition = transform.position;
        m_Collider = GetComponent<Collider>();
		m_Collider.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other) {
		if(other.tag == player.tag){
            OnHumanDetected(transform);
            m_Collider.enabled = false;
        }
	}
}