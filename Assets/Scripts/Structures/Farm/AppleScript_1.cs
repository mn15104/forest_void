using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Triggers narrative event on first apple pick
public class AppleScript_1 : MonoBehaviour  {

    bool m_HumanInProximity = false;

    void OnEnable()
    {
        HumanEventManager.OnInteract += PickApple;
    }
    void OnDisable()
    {
        HumanEventManager.OnInteract -= PickApple;
    }
    void Start () {
		
	}

	public void PickApple(GameObject obj)
    {
        Debug.Log((obj.transform.position - transform.position).magnitude);
        if(m_HumanInProximity)
        {
            GetComponent<NarrativeEventTrigger>().TriggerEvent();
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update () {
        
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
}
