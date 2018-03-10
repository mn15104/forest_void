using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTrigger : MonoBehaviour {

    public GameObject successorTrigger;
    protected TriggerController m_TriggerController;

    public virtual void Start()
    {
        m_TriggerController = FindObjectOfType<TriggerController>();
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        // Only if the player collider hits the trigger
        if (other == GameObject.FindGameObjectWithTag("Player").GetComponent<Collider>())
        {
            TriggerController.OnTriggerActivate += TriggerAction;
            m_TriggerController.EnableTrigger();
        }
    }

    public void TriggerAction()
    {
        // Sets next event to active if it exists
        if(successorTrigger != null)
        {
            successorTrigger.SetActive(true);

            //Activate children if it has any
            if(successorTrigger.transform.childCount > 0)
            {
                //Doesn't do grandchildren
                foreach (Transform child in successorTrigger.transform)
                {
                    child.gameObject.SetActive(true);
                }
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Only if the player collider hits the trigger
        if (other == GameObject.FindGameObjectWithTag("Player").GetComponent<Collider>())
        {
            Debug.Log("Removed trigger from delegate");
            TriggerController.OnTriggerActivate -= TriggerAction;
            gameObject.GetComponent<Collider>().enabled = false;
        }
    }
}
