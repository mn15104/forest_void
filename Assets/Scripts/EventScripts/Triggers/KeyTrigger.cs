using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyTrigger : MonoBehaviour {

    public GameObject key;
    public GameObject LED;
    private Collider keyCollider;
    private Renderer LEDRend;

	// Use this for initialization
	void Start () {
        keyCollider = key.GetComponent<Collider>();
        LEDRend = LED.GetComponent<Renderer>();
	}

    private void OnTriggerEnter(Collider keyCollider)
    {
        LEDRend.material.SetColor("_Color", Color.red);
        Debug.Log("Inserting Key");
    }
}
