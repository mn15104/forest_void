using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorEvent : EventTrigger {

    public AudioClip m_audio;
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
            Debug.Log("I need the keys");
            AudioSource.PlayClipAtPoint(m_audio, transform.position);
            m_TriggerController.EnableKeyEvents();
        }
    }
}
