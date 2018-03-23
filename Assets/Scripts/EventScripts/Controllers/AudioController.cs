using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.PostProcessing;
using System.Linq;
using System;
using Component = UnityEngine.Component;
using Object = UnityEngine.Object;
using System.ComponentModel;

public enum AudioSourceType {
    AMBIENCE,
    WIND,
    BRIDGE,
    CARAVAN,
    SITE,
    HOUSE, 
    CHAPEL
}

public class AudioController : MonoBehaviour {

    public delegate void AudioUpdate(AudioSourceType aud);
    public static event AudioUpdate Notify;

    
    //Ambient Audio

    public AudioSource m_Ambience1Background;
    public AudioSource m_Ambience2Background;
    public AudioSource m_WindBackground;
    public AudioSource m_BridgeAmbience;
    public AudioSource m_CaravanAmbience;
    public AudioSource m_SiteAmbience;
    public AudioSource m_HouseAmbience;
    public AudioSource m_ChapelAmbience;
    public AudioSource m_SFXAudioSrc;
    public AudioClip m_ChaseBuildUp;
    public AudioClip m_Chase;
    public AudioClip m_GameOver;

    //Players
    private HumanController human;
    private HumanVRController humanvr;
    //Monster
    private MonsterState m_MonsterState;
    private MonsterAI m_Monster;




    private void OnEnable()
    {
        
        m_Monster = FindObjectOfType<MonsterAI>();
        if(m_Monster)
            m_Monster.OnMonsterStateChange += MonsterStateChange;
    }

   
    void Start () {
        human = FindObjectOfType<HumanController>();
        humanvr = FindObjectOfType<HumanVRController>();

    
        //m_AmbienceBacklay
    }
    

    void Update()
    {
        switch (m_MonsterState)
        {
            case MonsterState.HIDDEN_IDLE:
                UpdateHiddenMusic();
                break;
            case MonsterState.HIDDEN_MOVING:
                UpdateHiddenMusic();
                break;
            case MonsterState.FOLLOW:
                UpdateFollowMusic();
                break;
            case MonsterState.APPEAR:
                UpdateAppearMusic();
                break;
            case MonsterState.CHASE:
                UpdateChaseMusic();
                break;
        }
    }

    private void MonsterStateChange(MonsterState monsterStateChange)
    {
        if (m_MonsterState != MonsterState.FOLLOW && monsterStateChange == MonsterState.FOLLOW)
        {
            m_Ambience2Background.enabled = true;
            m_Ambience2Background.volume = 0;
            m_Ambience2Background.clip = m_ChaseBuildUp;
            m_Ambience2Background.Play();
        }
        else if (m_MonsterState != MonsterState.APPEAR && monsterStateChange == MonsterState.APPEAR)
        {
            foreach (AudioSource aud in GetComponentsInChildren<AudioSource>())
            {
                if (aud.volume != 0)
                    aud.volume = 0;
            }
        }
        else if (m_MonsterState != MonsterState.APPROACH && monsterStateChange == MonsterState.APPROACH)
        {
            foreach (AudioSource aud in GetComponentsInChildren<AudioSource>())
            {
                if (aud.volume != 0)
                    aud.volume = 0;
            }
        }
        else if (m_MonsterState != MonsterState.CHASE && monsterStateChange == MonsterState.CHASE)
        {
            if (m_Ambience2Background.enabled == false)
            {
                m_Ambience2Background.enabled = true;
            }
            m_Ambience2Background.clip = m_Chase;
            m_Ambience2Background.volume = 0;
            m_Ambience2Background.Play();
        }

        m_MonsterState = monsterStateChange;
    }

    void UpdateHiddenMusic()
    {
        foreach (AudioSource aud in GetComponentsInChildren<AudioSource>())
        {
            if ((aud == m_WindBackground || aud == m_Ambience1Background))
            {
                if (m_WindBackground.volume < 1f && m_Ambience1Background.volume < 1f)
                {
                    m_Ambience1Background.volume += Time.deltaTime * 0.1f;
                    m_WindBackground.volume += Time.deltaTime * 0.1f;
                }
            }
            else if (aud == m_Ambience2Background)
            {
                if (aud.volume > 0f )
                {
                    aud.volume -= Time.deltaTime * 0.1f;
                }
            }
            else
            {
                if (aud.volume < 1f)
                {
                    aud.volume += Time.deltaTime * 0.1f;
                }
            }
        }
    }

    void UpdateFollowMusic()
    {
        foreach (AudioSource aud in GetComponentsInChildren<AudioSource>())
        {
                
            if ((aud == m_WindBackground || aud == m_Ambience1Background))
            {
                if (m_WindBackground.volume > 0f && m_Ambience1Background.volume > 0f)
                {
                    m_Ambience1Background.volume -= Time.deltaTime * 0.1f;
                    m_WindBackground.volume -= Time.deltaTime * 0.1f;
                }
            }
            else if (aud == m_Ambience2Background)
            {
                if (m_Ambience2Background.volume < 1f)
                {
                    m_Ambience2Background.volume += Time.deltaTime * 0.1f;
                }
            }
            else 
            {
                if (aud.volume > 0f)
                    aud.volume -= Time.deltaTime * 0.1f;
            }
        }
        
    }
    void UpdateAppearMusic()
    {
        foreach (AudioSource aud in GetComponentsInChildren<AudioSource>())
        {
            if(aud.volume > 1f)
                aud.volume -= Time.deltaTime;
        }
    }
    void UpdateApproachMusic()
    {
       
    }
    void UpdateChaseMusic()
    {
        while (m_Ambience2Background.volume < 1)
        {
            m_Ambience2Background.volume += Time.deltaTime;
        }
    }
    


}
