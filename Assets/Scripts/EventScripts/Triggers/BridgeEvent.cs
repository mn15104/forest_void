using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeEvent : EventTrigger {

    private AudioSource m_audio;
    TriggerController m_TriggerController;

    void Start()
    {
        m_audio = GetComponent<AudioSource>();
        m_TriggerController = FindObjectOfType<TriggerController>();

    }

    void OnTriggerEnter(Collider other)
    {
        // Only if the player collider hits the trigger
        if (other == GameObject.FindGameObjectWithTag("Player").GetComponent<Collider>())
        {
            Debug.Log("I should go to the house");
            //AudioSource.PlayClipAtPoint(m_audio, transform.position);
            m_audio.Play();
           m_TriggerController.EnableTrigger(TriggerController.Triggers.HOUSE);
        }
    }
}
