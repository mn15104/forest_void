using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour {

    private Light m_Light;
    private EventManager eventManager;
    public bool m_FlashlightActive = false;

    private void Awake()
    {
        eventManager = FindObjectOfType<EventManager>();
    }

    void OnEnable()
    {
        HumanEventManager.OnUseItem += Switch;
        eventManager.NotifyLocation.NotifyEventOccurred += CryptTorchLight;
    }
    void OnDisable()
    {
        HumanEventManager.OnUseItem -= Switch;
        eventManager.NotifyLocation.NotifyEventOccurred -= CryptTorchLight;
    }

    // Use this for initialization
    void Start () {
        m_Light = GetComponentInChildren<Light>();
        m_Light.intensity = 0;
	}

    void CryptTorchLight(EventManager.Location currentLocation)
    {
        if(currentLocation == EventManager.Location.Crypt)
        {
            m_Light.intensity = 0;
        }
        else
        {
            m_Light.intensity = 4;
        }
    }

	
    public void FV_Rotate(Vector3 rotation, float angle)
    {
        transform.Rotate(rotation, angle);
    }

    public void Switch(GameObject t)
    {
        if (m_FlashlightActive && (GetComponentInParent<HumanController>()  || GetComponentInParent<OVRPlayerController>()))
        {
            m_Light.intensity = 0;
            m_FlashlightActive = false;
        }
        else if (!m_FlashlightActive && (GetComponentInParent<HumanController>() || GetComponentInParent<OVRPlayerController>()))
        {
            m_Light.intensity = 4;
            m_FlashlightActive = true;
        }
    }
    

	// Update is called once per frame
	void Update () {
		
	}
}
