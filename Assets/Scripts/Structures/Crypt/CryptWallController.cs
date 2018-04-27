using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CryptWallController : MonoBehaviour {

    public List<GameObject> toTurnOn;
    public List<GameObject> toTurnOff;
    private EventManager eventManager;
    // Use this for initialization
    void Start () {
        eventManager = FindObjectOfType<EventManager>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void TurnWallsOff()
    {
        foreach (GameObject wall in toTurnOff)
        {
            if(wall.activeSelf && wall.name == "md_KeySetYellowV1" && !wall.GetComponent<KeyGrabbable>().hasBeenCollected)
            {
                wall.SetActive(false);
            }

            else if (wall.activeSelf && wall.name != "md_KeySetYellowV1")
            {
                wall.SetActive(false);
                //if (wall.GetComponent<MeshCollider>())
                //{
                 //   wall.GetComponent<MeshCollider>().enabled = false;
                //}
            }
        }
    }

    private void TurnWallsOn()
    {
        foreach (GameObject wall in toTurnOn)
        {
            if (!wall.activeSelf)
            {
                wall.SetActive(true);
                //wall.GetComponent<MeshRenderer>().enabled = true;
                //if (wall.GetComponent<MeshCollider>())
                //{
                 //   wall.GetComponent<MeshCollider>().enabled = true;
                //}
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == eventManager.player)
        {
            //Debug.Log("REaching here!");
            TurnWallsOff();
            TurnWallsOn();
        }
    }
}
