using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//Subtitle trigger triggered solo through a trigger component on an object
public class NarrativeEvent : MonoBehaviour {

    NarrativeController m_EventController;
    public string NarrativeSubtitle;

    public void Start()
    {
        m_EventController = FindObjectOfType<NarrativeController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<HumanController>())
        {
            m_EventController.QueueText(NarrativeSubtitle);
            gameObject.SetActive(false);
        }
    }
}
