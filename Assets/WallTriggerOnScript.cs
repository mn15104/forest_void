using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTriggerOnScript : MonoBehaviour
{

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

            TurnKeyOn.GetComponent<KeyGrabbable>().enabled = true;
            TurnKeyOn.GetComponent<BoxCollider>().enabled = true;
            OnChild1.GetComponent<MeshRenderer>().enabled = true;
            OnChild2.GetComponent<MeshRenderer>().enabled = true;
        }


    }
}
