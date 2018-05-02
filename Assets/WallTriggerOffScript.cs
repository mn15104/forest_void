using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTriggerOffScript : MonoBehaviour {

    public GameObject TurnKeyOff;
    public GameObject OffChild1;
    public GameObject OffChild2;

    // Use this for initialization
    void Start()
    {

        TurnKeyOff.GetComponent<KeyGrabbable>().enabled = false;
        TurnKeyOff.GetComponent<BoxCollider>().enabled = false;
        OffChild1.GetComponent<MeshRenderer>().enabled = false;
        OffChild2.GetComponent<MeshRenderer>().enabled = false;

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (OffChild1.GetComponent<MeshRenderer>().enabled == true)
        {



            TurnKeyOff.GetComponent<KeyGrabbable>().enabled = false;
            TurnKeyOff.GetComponent<BoxCollider>().enabled = false;
            OffChild1.GetComponent<MeshRenderer>().enabled = false;
            OffChild2.GetComponent<MeshRenderer>().enabled = false;
        }


    }
}
