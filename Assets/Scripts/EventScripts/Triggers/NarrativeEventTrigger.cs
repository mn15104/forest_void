using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Subtitle trigger, triggered externally by another script's conditions
public class NarrativeEventTrigger : MonoBehaviour
{

    NarrativeController m_EventController;
    HumanController human = null;
    public string NarrativeSubtitle;
    private PlayerMoveState state;
    
    private bool subtitleQueued = false;

    public void Start()
    {
        m_EventController = FindObjectOfType<NarrativeController>();
    }

    private void Update()
    {
            
    }

    public void TriggerEvent()
    {
        m_EventController.QueueText(NarrativeSubtitle);
        subtitleQueued = true;
    }
}