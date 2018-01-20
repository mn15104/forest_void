using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Narrative event enabled to be trigggered from beginning of chase scene
public class ChaseCheckpoint : MonoBehaviour {

    NarrativeEvent m_NarrativeEvent;
    SphereCollider m_SphereCollider;

    private void OnEnable()
    {
        AppleScript_2.OnAlert += EnableNarrativeEvent;
    }

    // Use this for initialization
    void Start () {
        m_NarrativeEvent = GetComponent<NarrativeEvent>();
        m_SphereCollider = GetComponent<SphereCollider>();
        m_NarrativeEvent.enabled = false;
        m_SphereCollider.enabled = false;
    }

	void EnableNarrativeEvent(GameObject human)
    {
        m_SphereCollider.enabled = true;
        m_NarrativeEvent.enabled = true;
    }

	// Update is called once per frame
	void Update () {
		
	}
}
