using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour {

    private Light m_Light;
    private EventManager eventManager;
    public  bool m_FlashlightActive = false;
    private bool m_FlashLightActiveBeforeCrypt = false;
    private bool insideCrypt = false;
    public bool disableFlashlight = false;
    public bool disableFlicker = false;
    public AudioSource m_FlashlightAudio;
    public AudioClip m_FlashlightOn;
    public AudioClip m_FlashlightOff;
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
        m_FlashlightAudio.loop = false;
        m_FlashlightAudio.volume = 1f;
        StartCoroutine(StartRandomFlicker());
    }
    private void Update()
    {
        if (disableFlashlight && m_FlashlightActive)
        {
            m_FlashlightAudio.clip = m_FlashlightOff;
            m_FlashlightAudio.Play();
            m_Light.intensity = 0;
            m_FlashlightActive = false;
            eventManager.NotifyTorchPressed.Notify(false);
        }
    }

    IEnumerator StartRandomFlicker()
    {
        float timeTilFlicker = Random.Range(2, 3);
        float flickerTimer = 0f;
        while (true)
        {
            if (!disableFlicker && !disableFlashlight && !insideCrypt)
            {

                flickerTimer += Time.deltaTime;
                if (flickerTimer > timeTilFlicker && m_FlashlightActive)
                {
                    for (int i = 0; i < (int)Random.Range(2,5); i++)
                    {
                        m_Light.intensity = 1f;
                        yield return new WaitForSeconds(0.01f);
                        m_Light.intensity = 3f;
                        yield return new WaitForSeconds(0.01f);
                    }
                    m_Light.intensity = 4f;
                    flickerTimer = 0f;
                    timeTilFlicker = Random.Range(20, 60);
                }
                
            }
            yield return null;
        }
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

    public void Switch(GameObject t)
    {
       
        if (m_FlashlightActive && (GetComponentInParent<HumanController>() || true))
        {
            m_FlashlightAudio.clip = m_FlashlightOff;
            m_FlashlightAudio.Play();
            if (!insideCrypt && !disableFlashlight)
            {
                m_Light.intensity = 0;
                m_FlashlightActive = false;
                eventManager.NotifyTorchPressed.Notify(false);
            }
        }
        else if (!m_FlashlightActive && (GetComponentInParent<HumanController>() || true))
        {
            m_FlashlightAudio.clip = m_FlashlightOn;
            m_FlashlightAudio.Play();
            if (!insideCrypt && !disableFlashlight)
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
    public void SetDisableFlashlight(bool off)
    {
        disableFlashlight = off;
        if (off)
        {
            m_FlashlightActive = false;
            m_Light.intensity = 0;
        }
    }
    public bool GetDisableFlashlight()
    {
        return disableFlashlight;
    }
    public void SetDisableFlicker(bool off)
    {
        if(off != disableFlicker)
        {
            disableFlicker = off;
            if (off)
            {
                StopAllCoroutines();
            }
            else
            {
                StartCoroutine(StartRandomFlicker());
            }
        }
    }
    public bool GetDisableFlicker()
    {
        return disableFlicker;
    }
}
