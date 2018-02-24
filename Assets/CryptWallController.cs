using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CryptWallController : MonoBehaviour {

    public List<GameObject> toTurnOn;
    public List<GameObject> toTurnOff;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void TurnWallsOff()
    {
        foreach (GameObject wall in toTurnOff)
        {
            if (wall.GetComponent<MeshRenderer>())
            {
                Debug.Log("Fuck");
                wall.GetComponent<MeshRenderer>().enabled = false;
                if (wall.GetComponent<MeshCollider>())
                {
                    wall.GetComponent<MeshCollider>().enabled = false;
                }
            }
        }
    }

    private void TurnWallsOn()
    {
        foreach (GameObject wall in toTurnOn)
        {
            if (wall.GetComponent<MeshRenderer>())
            {
                wall.GetComponent<MeshRenderer>().enabled = true;
                if (wall.GetComponent<MeshCollider>())
                {
                    wall.GetComponent<MeshCollider>().enabled = true;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        TurnWallsOff();
        TurnWallsOn();
    }
}
