using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObject : MonoBehaviour {

    public AudioSource m_Aud;
    public AudioClip[] m_CollisionClip;
    private int clipIndex = 0;
	// Use this for initialization
	void Start () {
        m_Aud.Stop();
        m_Aud.loop = false;
        m_Aud.volume = 1f;
        m_Aud.clip = m_CollisionClip[0];
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if(!(collision.transform.root.gameObject.GetComponentInChildren<HumanController>() ||
           collision.transform.root.gameObject.GetComponentInChildren<HumanVRController>()))
        m_Aud.PlayOneShot(m_CollisionClip[clipIndex], collision.relativeVelocity.y);
        if (clipIndex < m_CollisionClip.Length - 1)
        {
            clipIndex++;
        }
        else
        {
            clipIndex = 0;
        }
        
    }
}
