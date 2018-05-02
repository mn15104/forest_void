using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTriggerKeyScript : MonoBehaviour
{


    public GameObject TurnKeyOff;
    public GameObject OffChild1;
    public GameObject OffChild2;
    public GameObject TurnKeyOn;
    public GameObject OnChild1;
    public GameObject OnChild2;

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (OnChild1.GetComponent<MeshRenderer>().enabled == false)
        {
            
            TurnKeyOff.GetComponent<KeyGrabbable>().enabled = true;
            TurnKeyOff.GetComponent<BoxCollider>().enabled = true;
            OnChild1.GetComponent<MeshRenderer>().enabled = true;
            OnChild2.GetComponent<MeshRenderer>().enabled = true;
        }

        if (OffChild1.GetComponent<MeshRenderer>().enabled == true)
        {
            Debug.Log("TurningOFF");
            OffChild1.GetComponent<MeshRenderer>().enabled = false;
            OffChild2.GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
