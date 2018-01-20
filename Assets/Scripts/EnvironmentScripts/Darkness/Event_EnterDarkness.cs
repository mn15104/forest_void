using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Event to be enabled to be trigggered from collider trigger, after entering darkness
public class Event_EnterDarkness : MonoBehaviour {

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
        if (other.gameObject.GetComponent<HumanController>())
        {
            m_EventController.TriggerEvent(GameEventStage.EnteringDarknessForest);
        }
    }


}
