using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaygroundScriptA : MonoBehaviour {


    public GameObject teleportDest;
    public AudioSource audioSrc;

    Transform mannequin;
    HumanController human;
    private bool trigger_one = false;
    
	// Use this for initialization
	void Start () {
        mannequin = transform.GetChild(0);
        transform.GetChild(0).gameObject.SetActive(false);
        audioSrc = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
        
        if (human != null)
        {
            if (Vector3.Dot(human.transform.forward, (mannequin.position - human.transform.position).normalized) > 0 && !trigger_one)
            {
                audioSrc.PlayOneShot(audioSrc.clip);
                trigger_one = true;
            }
            else if (trigger_one && !(Vector3.Dot(human.transform.forward, (mannequin.position - human.transform.position).normalized) > 0))
            {
                human.gameObject.transform.position = teleportDest.transform.position;
                trigger_one = false;
                human = null;
                mannequin.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        human = other.gameObject.GetComponent<HumanController>();
        if(human != null)
        {
            
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
