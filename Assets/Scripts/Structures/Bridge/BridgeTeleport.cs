using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Teleports player from current transform to a teleport destination; bridge dest.

public class BridgeTeleport : MonoBehaviour {

    public Transform m_TeleportDestination;
    public Terrain m_TeleportTerrain;
    private EventController m_EventController;
    // Use this for initialization
    void Start () {
        m_EventController = FindObjectOfType<EventController>();
    }

	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        HumanController maybeHuman = collision.gameObject.GetComponent<HumanController>();
        if (maybeHuman)
        {
            if (m_TeleportDestination)
            {
                maybeHuman.gameObject.GetComponent<Transform>().position = m_TeleportDestination.position;
                maybeHuman.SetCurrentTerrain(m_TeleportTerrain);
                m_EventController.SetSceneToDarkness(true);
                
            }
            else
                Debug.Log("ERROR, NO TELEPORT DESTINATION PASSED INTO VARIABLE");


            if (m_TeleportTerrain)
                maybeHuman.SetCurrentTerrain(m_TeleportTerrain);
            else
                Debug.Log("ERROR, NO NEW TERRAIN PASSED INTO VARIABLE");
        }

    }
}
