using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseEndingScript : MonoBehaviour {


    private bool triggered = true;
    public GameObject EndingAnimation;
    public GameObject soundDistresses;
    public GameObject nextTrigger;
    public bool PlayedMusic = false;
    //public GameObject NextTrigger;
    //public TrackingSpaceController trackingSpace;

    // Use this for initialization
    void Start()
    {
        PlayedMusic = false;
        triggered = false;
        soundDistresses.SetActive(false);
        nextTrigger.GetComponent<BoxCollider>().enabled = false;
        //NextTrigger.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {



        if (!PlayedMusic && !triggered)
        {

            
            EndingAnimation.GetComponent<Animator>().enabled = true;

            soundDistresses.SetActive(true);
            nextTrigger.GetComponent<BoxCollider>().enabled = true;
            triggered = true;
            PlayedMusic = true;
            //NextTrigger.SetActive(true);

        }
    }
}
