using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour {

    private Light m_Light;
    private EventManager eventManager;
    public  bool m_FlashlightActive = false;
    private bool m_FlashLightActiveBeforeCrypt = false;
    private bool insideCrypt = false;

    private void Awake()
    {
        eventManager = FindObjectOfType<EventManager>();
    }

    void OnEnable()
    {
        HumanEventManager.OnUseItem += Switch;
        eventManager.StructureZoneTriggerEvent.TriggerEnterEvent += CryptTorchLightOff;
        eventManager.StructureZoneTriggerEvent.TriggerExitEvent += CryptTorchLightOn;
    }
    void OnDisable()
    {
        HumanEventManager.OnUseItem -= Switch;
        eventManager.StructureZoneTriggerEvent.TriggerEnterEvent -= CryptTorchLightOff;
        eventManager.StructureZoneTriggerEvent.TriggerExitEvent -= CryptTorchLightOn;
    }

    // Use this for initialization
    void Start () {
        m_Light = GetComponentInChildren<Light>();
        m_Light.intensity = 0;
	}

    void CryptTorchLightOff(GameObject gameObject)
    {
        if(gameObject.GetComponent<StructureZone>().location == EventManager.Location.Crypt)
        {
            if (m_FlashlightActive)
            {
                m_FlashLightActiveBeforeCrypt = true;
                Switch(gameObject);

            }
            insideCrypt = true;
        }
    }

    void CryptTorchLightOn(GameObject gameObject)
    {
        if (gameObject.GetComponent<StructureZone>().location != EventManager.Location.Forest)
        {
            insideCrypt = false;
            if (m_FlashLightActiveBeforeCrypt)
            {
                Switch(gameObject);
            }
           
        }
       
    }

	
    public void FV_Rotate(Vector3 rotation, float angle)
    {
        transform.Rotate(rotation, angle);
    }

    public void Switch(GameObject t)
    {
        if (!insideCrypt)
        {
            if (m_FlashlightActive && (GetComponentInParent<HumanController>() || true))
            {
                m_Light.intensity = 0;
                m_FlashlightActive = false;
                eventManager.NotifyTorchPressed.Notify(false);
            }
            else if (!m_FlashlightActive && (GetComponentInParent<HumanController>() || true))
            {
                m_Light.intensity = 4;
                m_FlashlightActive = true;
                eventManager.NotifyTorchPressed.Notify(true);
            }
        }
        else
        {
            eventManager.NotifyTorchPressed.Notify(false);
        }
    }
    
}
