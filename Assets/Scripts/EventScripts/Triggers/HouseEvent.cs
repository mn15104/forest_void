using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseEvent : EventTrigger {
    
    TriggerController m_TriggerController;

    private void Start()
    {
        m_TriggerController = FindObjectOfType<TriggerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only if the player collider hits the trigger
        if (other == GameObject.FindGameObjectWithTag("Player").GetComponent<Collider>())
        {
            TriggerController.OnTriggerActivate += TriggerAction;
            m_TriggerController.EnableTrigger();
        }
    }

}
