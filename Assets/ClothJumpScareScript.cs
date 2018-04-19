using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothJumpScareScript : MonoBehaviour {

    public AudioSource jumpScare;
    private bool hasPlayed;

    // Use this for initialization
    void Start()
    {
        hasPlayed = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasPlayed)
        {
           jumpScare.Play();
            hasPlayed = true;
        }
       
    }
}
