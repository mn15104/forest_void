using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.PostProcessing;

public class AudioAndLightController : MonoBehaviour {
    
    //Intro Fade In
    public Texture fadeIntro;
    private Canvas m_Canvas;
    private float alpha = 1.0f;
    private float fadeDir = -1;
    private float fadeSpeed = 0.05f;
    //SFX Audio
    public AudioClip m_MonsterScare;
    //Ambient Audio
    public AudioClip m_AmbienceDarkness;
    public AudioClip m_Tense;
    public AudioClip m_Chase;
    public AudioClip m_EndChase;
    public AudioClip m_EnteringDarkness;
    public AudioSource m_MainAmbienceAudioSrc;
    public AudioSource m_EventAmbienceAudioSrc;
    public AudioSource m_SFXAudioSrc;
    //Light
    private float skyboxRotationRate = 4f;
    //Players
    private HumanController human;
    private GameEventStage m_GameEventStage;
    //System
    private EventController m_EventController;

    private void OnEnable()
    {
        
    }

    private void OnGUI()
    {
        if (alpha > 0f)
        {
            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
            GUI.depth = -1000;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeIntro);
            alpha += fadeDir * fadeSpeed * Time.deltaTime;
            alpha = Mathf.Clamp01(alpha);
        }
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * skyboxRotationRate);
    }

  
    // Use this for initialization
    void Start () {
        m_EventController = FindObjectOfType<EventController>();
        m_Canvas = FindObjectOfType<Canvas>();
        human = FindObjectOfType<HumanController>();

        m_MainAmbienceAudioSrc.volume = 1f;
        m_MainAmbienceAudioSrc.loop = false;
        m_MainAmbienceAudioSrc.enabled = false;
        m_EventAmbienceAudioSrc.volume = 1f;
        m_EventAmbienceAudioSrc.loop = false;
        m_EventAmbienceAudioSrc.enabled = false;
        m_SFXAudioSrc.volume = 1f;
        m_SFXAudioSrc.loop = false;
        m_SFXAudioSrc.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
        
    }

    public void TriggerMonsterHidden()
    {
        if(m_EventAmbienceAudioSrc.clip == m_Chase || m_EventAmbienceAudioSrc.clip == m_Tense)
        {
            StartCoroutine(HiddenMusic());
        }
    }

    public void TriggerMonsterChase()
    {
        if (!m_EventAmbienceAudioSrc.clip == m_Chase || !m_EventAmbienceAudioSrc.enabled == true)
        {
            m_SFXAudioSrc.enabled = true;
            m_SFXAudioSrc.clip = m_MonsterScare;
            m_SFXAudioSrc.Play();
            StartCoroutine(ChaseMusic());
        }
    }

    public void TriggerMonsterAppear()
    {
        m_EventAmbienceAudioSrc.enabled = true;
        m_EventAmbienceAudioSrc.clip = m_Tense;
        m_EventAmbienceAudioSrc.Play();
        StartCoroutine(ChaseMusic());
    }

    IEnumerator AppearMusic()
    {
        yield return new WaitForSeconds(0);
        m_EventAmbienceAudioSrc.volume = 0;
        m_EventAmbienceAudioSrc.Play();
        while (m_MainAmbienceAudioSrc.volume > 0 && m_EventAmbienceAudioSrc.volume < 1)
        {
            m_MainAmbienceAudioSrc.volume -= 0.05f;
            m_EventAmbienceAudioSrc.volume += 0.05f;
        }
    }
    IEnumerator HiddenMusic()
    {
        yield return new WaitForSeconds(0);
        while (m_MainAmbienceAudioSrc.volume < 1 || m_EventAmbienceAudioSrc.volume > 0)
        {
            m_MainAmbienceAudioSrc.volume += 0.05f;
            m_EventAmbienceAudioSrc.volume -= 0.05f;
        }
        m_EventAmbienceAudioSrc.clip = null;
        m_EventAmbienceAudioSrc.enabled = false;
    }

    IEnumerator ChaseMusic()
    {
        yield return new WaitForSeconds(m_MonsterScare.length * 0.25f);
        m_EventAmbienceAudioSrc.enabled = true;
        m_EventAmbienceAudioSrc.clip = m_Chase;
        m_EventAmbienceAudioSrc.volume = 0;
        m_EventAmbienceAudioSrc.Play();
        while (m_MainAmbienceAudioSrc.volume > 0 || m_EventAmbienceAudioSrc.volume < 1) {
            m_MainAmbienceAudioSrc.volume -= 0.05f;
            m_EventAmbienceAudioSrc.volume += 0.05f;
        }
    }


}
