using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HumanEventManager : MonoBehaviour {

    public delegate void Glow(GameObject item);
    public static event Glow OnGlow;
    public delegate void Interact(GameObject human);
    public static event Interact OnInteract;
    public delegate void UseItem(GameObject human);
    public static event UseItem OnUseItem;
    
    private InventoryScript m_Inventory;
    private Camera m_Cam;
    private Flashlight flashlight;
    private Canvas m_Canvas;

    private GameObject m_ObjectFocus;

    private void OnEnable()
    {
        m_Inventory = new InventoryScript();
        Flashlight maybeFlashlight = GetComponentInChildren<Flashlight>();
        if (maybeFlashlight != null)
        {
            flashlight = maybeFlashlight;
        }
        m_Cam = transform.GetComponentInChildren<Camera>();
        m_Canvas = GameObject.FindObjectOfType<Canvas>();
    }

    // Use this for initialization
    void Start () {

    }
	
	//Update is called once per frame
	void Update () {
        /////////////////////////////////
        // Interactions
        /////////////////////////////////
        /* Use Held Item */
        if (Input.GetKeyDown("f"))
        {
           OnUseItem(gameObject);
        }
        ///* Interact With Object */
        if (Input.GetKeyDown("e"))
        {
           OnInteract(gameObject);
        }
        if (Input.GetKeyDown("k"))
        {
            GetComponent<HumanController>().FallOver();
        }
	}
    
}
