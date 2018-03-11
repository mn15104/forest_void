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

    public AudioSource m_AmbienceBackground;
    public AudioSource m_WindBackground;
    public AudioClip m_Chase;
    public AudioClip m_GameOver;

    public AudioSource m_BridgeAmbience;
    public AudioSource m_CaravanAmbience;
    public AudioSource m_SiteAmbience;
    public AudioSource m_HouseAmbience;
    public AudioSource m_ChapelAmbience;
    public  AudioSource m_SFXAudioSrc;

    //Players
    private HumanController human;
    private HumanVRController humanvr;
    //Monster
    private MonsterState m_MonsterState;
    private MonsterAI m_Monster;




    private void OnEnable()
    {
        
        //foreach (){ 
        //{

        //}
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
        
    }

    private void MonsterStateChange(MonsterState monsterStateChange)
    {
        if(monsterStateChange != MonsterState.APPEAR && m_BridgeAmbience.enabled == false)
        {
            foreach (AudioSource aud in GetComponentsInChildren<AudioSource>())
            {
                aud.enabled = true;
                aud.Play();
            }
        }
        if (monsterStateChange == MonsterState.CHASE)
            TriggerMonsterChase();
        else if (monsterStateChange == MonsterState.HIDDEN_MOVING)
            TriggerMonsterHidden();
        else if (monsterStateChange == MonsterState.APPEAR)
            TriggerMonsterAppear();
        else if (monsterStateChange == MonsterState.GAMEOVER)
            TriggerMonsterGameOver();
    }

    

    public void TriggerMonsterGameOver()
    {
        //if (m_MonsterState != MonsterState.GAMEOVER)
        //{
        //    //m_MainAmbienceAudioSrc.enabled = true;
        //    //m_MainAmbienceAudioSrc.clip = m_GameOver;
        //    //m_MainAmbienceAudioSrc.Play();
        //    m_MonsterState = MonsterState.GAMEOVER;
        //}
    }

    public void TriggerMonsterHidden()
    {
        //if (m_MonsterState != MonsterState.HIDDEN_IDLE && m_MonsterState != MonsterState.HIDDEN_MOVING)
        //{
        //    StartCoroutine(HiddenMusic());
        //    m_MonsterState = MonsterState.HIDDEN_MOVING;
        //}
    }

    public void TriggerMonsterChase()
    {
        //if (m_MonsterState != MonsterState.CHASE)
        //{
        //    m_SFXAudioSrc.enabled = true;
        //    m_SFXAudioSrc.clip = m_MonsterScare;
        //    m_SFXAudioSrc.Play();
        //    StartCoroutine(ChaseMusic());
        //    m_MonsterState = MonsterState.CHASE;
        //}
    }

    public void TriggerMonsterAppear()
    {
        foreach(AudioSource aud in GetComponentsInChildren<AudioSource>())
        {
            aud.enabled = false;
        }
        //if (m_MonsterState != MonsterState.APPEAR)
        //{
        //    //m_EventAmbienceAudioSrc.enabled = true;
        //    //m_EventAmbienceAudioSrc.clip = m_Tense;
        //    //m_EventAmbienceAudioSrc.Play();
        //    //StartCoroutine(ChaseMusic());
        //    //m_MonsterState = MonsterState.APPEAR;
        //}
    }

    IEnumerator AppearMusic()
    {
        yield return new WaitForSeconds(0);
        //m_EventAmbienceAudioSrc.volume = 0;
        //m_EventAmbienceAudioSrc.Play();
        //while (m_MainAmbienceAudioSrc.volume > 0 && m_EventAmbienceAudioSrc.volume < 1)
        //{
        //    m_MainAmbienceAudioSrc.volume -= 0.05f;
        //    m_EventAmbienceAudioSrc.volume += 0.05f;
        //}
    }
    IEnumerator HiddenMusic()
    {
        yield return new WaitForSeconds(0);
        //while (m_MainAmbienceAudioSrc.volume < 1 || m_EventAmbienceAudioSrc.volume > 0)
        //{
        //    m_MainAmbienceAudioSrc.volume += 0.05f;
        //    m_EventAmbienceAudioSrc.volume -= 0.05f;
        //}
        //m_EventAmbienceAudioSrc.clip = null;
        //m_EventAmbienceAudioSrc.enabled = false;
    }

    IEnumerator ChaseMusic()
    {
        yield return new WaitForSeconds( 0.25f);
        //m_EventAmbienceAudioSrc.enabled = true;
        //m_EventAmbienceAudioSrc.clip = m_Chase;
        //m_EventAmbienceAudioSrc.volume = 0;
        //m_EventAmbienceAudioSrc.Play();
        //while (m_MainAmbienceAudioSrc.volume > 0 || m_EventAmbienceAudioSrc.volume < 1) {
        //    m_MainAmbienceAudioSrc.volume -= 0.05f;
        //    m_EventAmbienceAudioSrc.volume += 0.05f;
        //}
    }


}
