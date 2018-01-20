using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleScript_2 : MonoBehaviour  {

    EventController m_EventController;
    NarrativeController m_NarrativeController;
    HumanController human = null;
    bool m_HumanInProximity = false;
    int attempts = 0;

    //On two attempts of picking apple, triggers human to fall over, and globally triggers other events of beginning of chase scene
    public delegate void Alert(GameObject farmer);
    public static event Alert OnAlert;

    void OnEnable()
    {
        HumanEventManager.OnInteract += PickApple;
        m_NarrativeController = FindObjectOfType<NarrativeController>();
    }
    void OnDisable()
    {
        HumanEventManager.OnInteract -= PickApple;
    }
    void Start () {
        m_EventController = FindObjectOfType<EventController>();

    }

	public void PickApple(GameObject obj)
    {

        if (obj.GetComponent<HumanController>())
            human = obj.GetComponent<HumanController>();
        if (m_HumanInProximity)
        {
            if (attempts == 0)
            {
                m_NarrativeController.QueueText("Not quite, try again");
                attempts++;
            }
            else
            {
                human.FallOver();
                OnAlert(human.gameObject);
                m_NarrativeController.QueueText("Fuck, get out of here, quick, quick!!");
                m_EventController.TriggerEvent(GameEventStage.ChasedToBridge);
                gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<HumanController>())
        {
            m_HumanInProximity = true;
        }   
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<HumanController>())
        {
            m_HumanInProximity = false;
        }
    }

    // Update is called once per frame
    void Update () {
        
	}
}
