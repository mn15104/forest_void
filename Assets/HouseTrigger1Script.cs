using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseTrigger1Script : MonoBehaviour {

    private bool triggered = true;
    public AudioSource doorSound;
    public bool PlayedMusic = false;
    public GameObject NextTrigger;
    public GameObject DoorClosed;

	// Use this for initialization
	void Start () {
        PlayedMusic = false;
        triggered = false;
        NextTrigger.GetComponent<BoxCollider>().enabled = false;
        DoorClosed.GetComponent<Animator>().enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
       

        
        if (!PlayedMusic && !triggered)
        {
            
            doorSound.Play();
            DoorClosed.GetComponent<Animator>().enabled = true;

            triggered = true;
            PlayedMusic = true;
            NextTrigger.GetComponent<BoxCollider>().enabled = true;

        }
    }
}
