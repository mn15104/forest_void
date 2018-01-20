using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbandonedHouse : MonoBehaviour {

    Mannequin[] m_Mannequins;
    BoxCollider m_boxCollider;
    int playersInHouse = 0;

	// Use this for initialization
	void Start () {
        m_Mannequins = GetComponentsInChildren<Mannequin>();
        m_boxCollider = GetComponent<BoxCollider>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        HumanController humanCont = other.gameObject.GetComponentInChildren<HumanController>();
        HumanVRController playerOneCont = other.gameObject.GetComponentInChildren<HumanVRController>();
        if (humanCont != null || playerOneCont != null)
        {
            playersInHouse++;
            Camera playerCamera;
            GameObject otherObj = other.gameObject;
            Camera[] playerCams = otherObj.GetComponentsInChildren<Camera>();

            foreach (var cam in playerCams)
            {
                if (cam.isActiveAndEnabled)
                {
                    playerCamera = cam;
                    foreach (var mannequin in m_Mannequins)
                    {
                        mannequin.AlertMannequin(playerCamera);
                    }
                    break;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {

        HumanController humanCont = other.gameObject.GetComponentInChildren<HumanController>();
        HumanVRController playerOneCont = other.gameObject.GetComponentInChildren<HumanVRController>();
        if (humanCont != null || playerOneCont != null)
        {
            playersInHouse--;
            GameObject otherObj = other.gameObject;
            Camera[] playerCams = otherObj.GetComponentsInChildren<Camera>();
            foreach (var cam in playerCams)
            {
                if (cam.isActiveAndEnabled)
                {
                    foreach (var mannequin in m_Mannequins)
                    {
                        mannequin.DisableMannequin(cam);
                    }
                    break;
                }
            }

        }
    }
    
}
