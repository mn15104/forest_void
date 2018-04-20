using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObject : MonoBehaviour {

    public AudioSource m_Aud;
    public AudioClip m_CollisionClip;

	// Use this for initialization
	void Start () {
        m_Aud.Stop();
        m_Aud.loop = false;
        m_Aud.volume = 1f;
        m_Aud.clip = m_CollisionClip;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision");
        m_Aud.Play();
    }
}
