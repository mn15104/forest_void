using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HumanVRLeftHand : MonoBehaviour {

    public delegate void Interact(GameObject item, GameObject human);
    public static event Interact OnInteract;
    private HumanVRController m_HumanVRController;
    private int id = 0;
    private GameObject m_ObjectFocus = null;
    private InventoryScript PlayerInventory;


    private void OnEnable()
    {
    }

    // Use this for initialization
    void Start () {
        m_HumanVRController = GetComponentInParent<HumanVRController>();
        PlayerInventory = GetComponentInChildren<InventoryScript>();
    }
	
	//Update is called once per frame
	void Update () {
        if (SixenseInput.Controllers[id].GetButton(SixenseButtons.TRIGGER))
        {
            
        }
        if (SixenseInput.Controllers[id].GetButton(SixenseButtons.BUMPER))
        {
            if (m_ObjectFocus != null)
            {
                OnInteract(m_ObjectFocus, gameObject);
                m_ObjectFocus = null;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        RaycastHit hit;
        if (!Physics.Linecast(other.transform.position, transform.position, out hit, 11))
        {
            if (other.gameObject != gameObject && other.gameObject.layer == 11)
            {
                m_ObjectFocus = other.gameObject;
            }
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject == m_ObjectFocus)
        {
            m_ObjectFocus = null;
        }
    }
}
