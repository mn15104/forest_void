using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothScript : MonoBehaviour {

    protected EventManager eventManager;
    public GameObject otherCloth;

    private void Awake()
    {
        eventManager = FindObjectOfType<EventManager>();
        eventManager.NotifyYellowKeyPickup.NotifyEventOccurred += SwapCloth;

    }

    // Use this for initialization
    void Start () {
        otherCloth.GetComponentInChildren<MeshRenderer>().enabled = false;
    }

    void SwapCloth(bool PickedKey)
    {
        Debug.Log("clothing");
        otherCloth.GetComponentInChildren<MeshRenderer>().enabled = true;
        GetComponentInChildren<MeshRenderer>().enabled = false;

    }

    // Update is called once per frame
    void Update () {
		
	}
}
