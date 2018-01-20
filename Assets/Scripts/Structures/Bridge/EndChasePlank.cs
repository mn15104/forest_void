using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndChasePlank : MonoBehaviour {

    EventController m_EventController;

	// Use this for initialization
	void Start () {
        m_EventController = FindObjectOfType<EventController>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<HumanController>())
        {
            m_EventController.TriggerEvent(GameEventStage.EndOfChase);
        }
    }
}
