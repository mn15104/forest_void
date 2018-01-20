using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterFarm : MonoBehaviour {

    EventController m_EventController;

	// Use this for initialization
	void Start () {
        m_EventController = FindObjectOfType<EventController>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<HumanController>())
        {
            m_EventController.TriggerEvent(GameEventStage.EnterFarm);
        }   
    }
}
