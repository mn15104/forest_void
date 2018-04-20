using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CryptShadowScript: MonoBehaviour {

    protected EventManager eventManager;
    public GameObject cryptShadow;

    private void Awake()
    {
        eventManager = FindObjectOfType<EventManager>();
        eventManager.NotifyYellowKeyPickup.NotifyEventOccurred += activateCryptShadow;

    }

    // Use this for initialization
    void Start()
    {
        cryptShadow.SetActive(false);
    }

    void activateCryptShadow(bool PickedKey)
    {
        cryptShadow.SetActive(true);
        //GetComponentInChildren<MeshRenderer>().enabled = false;

    }

    // Update is called once per frame
    void Update()
    {

    }
}
