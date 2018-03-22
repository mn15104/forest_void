using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeEvent : MonoBehaviour
{

    private AudioSource m_audio;
    private EventManager eventManager;

    public void Awake()
    {
        m_audio = GetComponent<AudioSource>();
        eventManager = FindObjectOfType<EventManager>();
    }

    public void OnEnable()
    {
    }

    public void Start()
    {
        eventManager.BridgeCrossedEvent.TriggerEnterEvent += PlayVoiceRecording;
    }

    public void OnDisable()
    {
        eventManager.BridgeCrossedEvent.TriggerEnterEvent -= PlayVoiceRecording;
    }


    public void OnTriggerEnter(Collider other)
    {
        if (other == GameObject.FindGameObjectWithTag("Player").GetComponent<Collider>())
        {
            eventManager.BridgeCrossedEvent.TriggerEnter(other.gameObject);
           
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GetComponent<Collider>().enabled = false;
    }


    public void PlayVoiceRecording(GameObject collider)
    {
        m_audio.Play();
    }
    

}