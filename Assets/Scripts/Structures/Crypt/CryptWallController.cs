using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CryptWallController : MonoBehaviour {

    public List<GameObject> toTurnOn;
    public List<GameObject> toTurnOff;
    public GameObject key;
    public Vector3 plinthLocation;
    public Vector3 outsideLocation;
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
            if (wall.activeSelf)
            {
                wall.SetActive(false);
                //if (wall.GetComponent<MeshCollider>())
                //{
                 //   wall.GetComponent<MeshCollider>().enabled = false;
                //}
            }
        }
        key.transform.position = outsideLocation;
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
        key.transform.position = plinthLocation;
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
