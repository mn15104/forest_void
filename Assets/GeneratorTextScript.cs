using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneratorTextScript : MonoBehaviour {

    public GameObject Lever;

    protected EventManager eventManager;
	// Use this for initialization
	void Start () {
        Lever.GetComponent<Animator>().enabled = false;
        eventManager = FindObjectOfType<EventManager>();
        eventManager.NotifyAllKeysInserted.NotifyEventOccurred += ChangeText;
    }

    void ChangeText(bool boolean)
    {
        Lever.GetComponent<Animator>().enabled = true;
        gameObject.GetComponent<Text>().text = "Generator turned off";
    }
	

}
