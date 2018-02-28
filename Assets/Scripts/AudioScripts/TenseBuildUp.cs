using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TenseBuildUp : MonoBehaviour {
    public AudioSource m_TenseBuildUp;
    private GameObject m_human = null;
    private SphereCollider m_Collider;
    private float m_ColliderRadius;
    // Use this for initialization
	void Start () {
        m_TenseBuildUp.Stop();
        m_Collider = GetComponent<SphereCollider>();
        m_ColliderRadius = GetComponent<SphereCollider>().radius;
    }
	
	// Update is called once per frame
	void Update () {
        if (m_human)
        {
            float vol = ((m_human.transform.position - m_Collider.center).sqrMagnitude)/m_ColliderRadius;
            m_TenseBuildUp.volume = vol;
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<OVRPlayerController>())
        {
            m_TenseBuildUp.Play();
            m_TenseBuildUp.volume = 0f;
            m_human = other.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<OVRPlayerController>())
        {
            m_TenseBuildUp.Stop();
            m_TenseBuildUp.volume = 0f;
            m_human = null;
        }
    }
   
}
