using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Triggers narrative event on the player crouching within the trigger
public class BarrelCrouch : MonoBehaviour {

    NarrativeEventTrigger m_EventTrigger;
    HumanController human = null;
    private PlayerMoveState state;

    private bool subtitleQueued = false;

    public void Start()
    {
        m_EventTrigger = GetComponent<NarrativeEventTrigger>();
    }

    private void Update()
    {
        if ((human != null) && (human.GetPlayerMoveState() == PlayerMoveState.CROUCHING) && !subtitleQueued)
        {
            m_EventTrigger.TriggerEvent();
            subtitleQueued = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<HumanController>() && !subtitleQueued)
        {
            human = other.gameObject.GetComponent<HumanController>();
        }
    }
}
