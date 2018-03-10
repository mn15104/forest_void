using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeEvent : EventTrigger {

    private AudioSource m_audio;

    public override void Start()
    {
        m_audio = GetComponent<AudioSource>();

    }

    public override void OnTriggerEnter(Collider other)
    {
        // Only if the player collider hits the trigger
        if (other == GameObject.FindGameObjectWithTag("Player").GetComponent<Collider>())
        {
            Debug.Log("Added trigger to delegate");
            m_audio.Play();
            TriggerController.OnTriggerActivate += TriggerAction;
            m_TriggerController.EnableTrigger(); //variable from parent class
        }
    }
}
