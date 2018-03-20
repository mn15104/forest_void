using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour {

    private Light m_Light;
    public bool m_FlashlightActive = false;

    void OnEnable()
    {
        HumanEventManager.OnUseItem += Switch;
    }
    void OnDisable()
    {
        HumanEventManager.OnUseItem -= Switch;
    }

    // Use this for initialization
    void Start () {
        m_Light = GetComponentInChildren<Light>();
        m_Light.intensity = 0;
        m_Light.gameObject.SetActive(false);

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
            m_Light.gameObject.SetActive(false);
        }
        else if (!m_FlashlightActive && (GetComponentInParent<HumanController>() || GetComponentInParent<OVRPlayerController>()))
        {
            m_Light.intensity = 3;
            m_FlashlightActive = true;
            m_Light.gameObject.SetActive(true);

        }
    }
    

	// Update is called once per frame
	void Update () {
		
	}
}
