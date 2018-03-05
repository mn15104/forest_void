using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CabinToilet : MonoBehaviour {

    GameObject human;
    GameObject m_Door;
    public GameObject m_FrontWall;
    public AudioSource m_Banging;
    public AudioSource m_Squeaking;
    Quaternion m_OriginalRotation = new Quaternion();
    Vector3 m_OriginalPosition;
    EventStage eventStage = EventStage.Default;
    float turnedAwayMargin = 0f;
    float timer = 0f;
    bool Up = true;
    
    enum EventStage
    {
        Default,
        InRadius,
        LookedAtFrontWall,
        TurnedAwayBriefly,
        TurnedAway,
        Complete
    }

	// Use this for initialization
	void Start () {
        m_Banging.Stop();
        m_Banging.loop = false;
        m_Squeaking.Stop();
        m_Squeaking.loop = false;
        m_Door = GetComponentInChildren<DoorScript>().gameObject;
        m_Door.SetActive(false);
        m_OriginalPosition = transform.position;
        m_OriginalRotation = transform.rotation;

     

    }

	// Update is called once per frame
	void Update () {
        
        if (eventStage == EventStage.InRadius)
        {
                if (Vector3.Dot(human.GetComponentInChildren<Camera>().transform.forward, (transform.position - human.GetComponentInChildren<Camera>().transform.position).normalized) > 0)
                {
                    eventStage = EventStage.LookedAtFrontWall;
                }
        }
        if (eventStage == EventStage.LookedAtFrontWall)
        {
                if (Vector3.Dot(human.GetComponentInChildren<Camera>().transform.forward, (transform.position - human.GetComponentInChildren<Camera>().transform.position).normalized) < 0)
                {
                    eventStage = EventStage.TurnedAwayBriefly;
                }
        }
        if (eventStage == EventStage.TurnedAwayBriefly)
        {
            if (turnedAwayMargin > 2f)
            {
                foreach (Collider collider in GetComponentsInChildren<Collider>())
                {
                    if (!collider.isTrigger)
                    {
                        collider.enabled = false;
                    }
                }
                eventStage = EventStage.TurnedAway;
                m_Banging.Play();
            }
            else
            {
                turnedAwayMargin += Time.deltaTime;
                if (Vector3.Dot(human.GetComponentInChildren<Camera>().transform.forward, (transform.position - human.GetComponentInChildren<Camera>().transform.position).normalized) > 0)
                {
                    turnedAwayMargin = 0f;
                    eventStage = EventStage.LookedAtFrontWall;
                }
            }
        }
        if(eventStage == EventStage.TurnedAway)
        {
            if (timer <= 5f)
            {
                Shake(0.1f, 6f);
            }
            else
            {
                transform.position = m_OriginalPosition;
                transform.rotation = m_OriginalRotation;
                m_FrontWall.SetActive(false);
                m_Door.SetActive(true);
                GetComponentInChildren<DoorScript>().enabled = true;

                foreach (Collider collider in GetComponentsInChildren<Collider>())
                {
                    if (!collider.isTrigger)
                    {
                        collider.enabled = true;
                    }
                }
                
                m_Squeaking.Play();
                eventStage = EventStage.Complete;
            }
        }
    }




    void Shake(float translationDisplacement, float rotationDisplacement)
    {
        Vector3 objectPos = transform.position;
        Vector3 objectRot = transform.eulerAngles;
        timer += Time.deltaTime;

        objectRot.x += 1f * rotationDisplacement * Mathf.Sin(timer * 100f);
        objectRot.y += 1f * rotationDisplacement * Mathf.Sin(timer * 100f);
        objectRot.z += 1f * rotationDisplacement * Mathf.Sin(timer * 100f);
        transform.position = objectPos;
        transform.rotation = Quaternion.Euler(objectRot);
    }

  
    private void OnTriggerEnter(Collider other)
    {

        if ((other.gameObject.transform.root.GetComponent<HumanController>() || other.gameObject.transform.root.GetComponent<HumanVRController>()) && eventStage == EventStage.Default)
        {
            human = other.gameObject.transform.root.gameObject;
            eventStage = EventStage.InRadius;
        }
    }

}
