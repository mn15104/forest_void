using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainAudioController : MonoBehaviour {

    private EventManager eventManager;
    private MonsterAI monsterAI;
    public AudioSource m_Aud_1;
    public AudioSource m_Aud_2;
    public AudioSource m_Aud_SFX;
    public AudioSource m_Aud_Wind;
    public AudioClip m_WindClip;
    public AudioClip m_Stage1Clip;
    public AudioClip m_Stage2Clip;
    public AudioClip m_Stage3Clip;
    public AudioClip m_GameOverClip;
    public AudioClip[] m_IntenseNoises;
    private bool monsterStateInterrupt = false;
    public float m_MaxVolume = 0.3f;
    public float m_MaxWindVolume = 0.5f;
    public float m_FadeSpeed = 0.3f;

    private AudioClip currentlySelected;
    private EventManager.Stage currentAudioStage = EventManager.Stage.Intro;

    private void Awake() 
    {
        monsterAI = FindObjectOfType<MonsterAI>();
        monsterAI.OnMonsterStateChange += MonsterStateReact;
        eventManager = FindObjectOfType<EventManager>();
        eventManager.NotifyLocation.NotifyEventOccurred += StructureAudio;
        eventManager.NotifyStage.NotifyEventOccurred += StageAudio;
        eventManager.NotifyHeartRate.NotifyEventOccurred += HeartRateAudio;
        //eventManager.BridgeCrossedEvent.TriggerEnterEvent += BridgeTriggerAudio; 

    }
    private void Start()
    {
        m_MaxVolume = 0.3f;
        m_FadeSpeed = 0.5f;
        m_Aud_1.clip = m_Stage1Clip;
        m_Aud_1.loop = true;
        m_Aud_1.volume = 0;
        StartCoroutine(FadeInAudioSource(m_Aud_1, m_MaxVolume, m_FadeSpeed));
        //----------------
        m_Aud_Wind.clip = m_WindClip;
        m_Aud_Wind.loop = true;
        m_Aud_Wind.volume = 0;
        m_Aud_Wind.Play();
        StartCoroutine(FadeInAudioSource(m_Aud_Wind, m_MaxWindVolume, m_FadeSpeed));
        //----------------
        m_Aud_2.Stop();
        m_Aud_2.clip = null;
        m_Aud_2.loop = true;
        //----------------
        m_Aud_SFX.Stop();
        m_Aud_SFX.clip = null;
        m_Aud_SFX.loop = false;
    }
    private void Update()
    {
        if (!monsterStateInterrupt && !m_Aud_1.isPlaying && !m_Aud_2.isPlaying)
        {
            StopAllCoroutines();
            currentAudioStage = monsterAI.GetMonsterStage();
            StartCoroutine(FadeInAudioSource(m_Aud_1, m_MaxVolume, m_FadeSpeed));
        }
    }
    void MonsterStateReact(MonsterState state)
    {
        if (monsterAI.GetMonsterState() == MonsterState.FOLLOW)
        {
            monsterStateInterrupt = true;
            if (m_Aud_1.isPlaying || m_Aud_2.isPlaying)
            {
                StopAllCoroutines();
                StartCoroutine(FadeOutAudioSource(m_Aud_1, 0.0f, m_FadeSpeed));
                StartCoroutine(FadeOutAudioSource(m_Aud_2, 0.0f, m_FadeSpeed));
            }
        }
        else if (monsterAI.GetMonsterState() == MonsterState.APPROACH && !m_Aud_1.isPlaying || !m_Aud_2.isPlaying)
        {
            TransitionClip(m_Stage3Clip);
        }
        else
        {
            monsterStateInterrupt = false;
        }
    }

    void StageAudio(EventManager.Stage stage)
    {
        Debug.Log("Stage Audio changing to " + stage);
        switch (stage)
        {
            case EventManager.Stage.Intro:
                break;
            case EventManager.Stage.Stage1:
                TransitionClip(m_Stage1Clip);
                currentAudioStage = EventManager.Stage.Stage1;
                break;
            case EventManager.Stage.Stage2:
                TransitionClip(m_Stage2Clip);
                currentAudioStage = EventManager.Stage.Stage2;
                break;
            case EventManager.Stage.Stage3:
                TransitionClip(m_Stage3Clip);
                currentAudioStage = EventManager.Stage.Stage3;
                break;
            case EventManager.Stage.GameOverStage:
                TransitionClip(m_GameOverClip);
                currentAudioStage = EventManager.Stage.GameOverStage;
                break;
        }
    }

    void StructureAudio(EventManager.Location location)
    {
        
        bool windOn = false;
        switch (location)
        {
            case EventManager.Location.Forest:
                windOn = true;
                break;
            case EventManager.Location.Caravan:
                break;
            case EventManager.Location.Chapel:
                break;
            case EventManager.Location.Crypt:
                break;
            case EventManager.Location.Generator:
                windOn = true;
                break;
            case EventManager.Location.Bridge:
                windOn = true;
                break;
            case EventManager.Location.ToolShed:
                break;
        }
        if(windOn && !m_Aud_Wind.isPlaying)
        {
            StartCoroutine(FadeInAudioSource(m_Aud_Wind, m_MaxWindVolume, m_FadeSpeed));
        }
        else if(!windOn && m_Aud_Wind.isPlaying)
        {
            StartCoroutine(FadeOutAudioSource(m_Aud_Wind, 0.0f, m_FadeSpeed));
        }
    }

    void HeartRateAudio(float heartRate)
    {
        Debug.Log("Event: HeartRate=" + heartRate);
        //Heart Rate is send in - notifyed based on variable set in eventManager 
        //Currrently set to start notifying after 30sec, every 10sec
    }

    
    //If you want to also control audio for triggerevents(e.g BridgeTrigger(voice recording), CaravanTrigger(enter door), TextAppear etc
    //Do this e.g BridgeTrigger: uncomment is awake subscription to this event
     
    void BridgeTriggerAudio(GameObject colliderObject)
    {
        //Audio for bridge would go in here. Currently the audio is attached to the trigger itself. Found in the scene -> EventTriggers -> BridgeTrigger
        //Putting audio is here with run when trigger has been entered
    }

    void TransitionClip(AudioClip t_clip)
    {
        if (m_Aud_1.isPlaying)
        {
            m_Aud_2.clip = t_clip;
            StopAllCoroutines();

            StartCoroutine(FadeOutAudioSource(m_Aud_1, 0, m_FadeSpeed));
            m_Aud_2.volume = 0f;
            StartCoroutine(FadeInAudioSource(m_Aud_2, m_MaxVolume, m_FadeSpeed));
        }
        else if (m_Aud_2.isPlaying)
        {
            m_Aud_1.clip = t_clip;
            StopAllCoroutines();
            StartCoroutine(FadeOutAudioSource(m_Aud_2, 0, m_FadeSpeed));
            m_Aud_1.volume = 0f;
            StartCoroutine(FadeInAudioSource(m_Aud_1, m_MaxVolume, m_FadeSpeed));
        }
        else if (!m_Aud_2.isPlaying && !m_Aud_1.isPlaying)
        {
            m_Aud_1.clip = t_clip;
            StopAllCoroutines();
            StartCoroutine(FadeOutAudioSource(m_Aud_2, 0, m_FadeSpeed));
            m_Aud_1.volume = 0f;
            StartCoroutine(FadeInAudioSource(m_Aud_1, m_MaxVolume, m_FadeSpeed));
        }
    }

    //parms[0] is audiosource, parms[1] is max volume
    IEnumerator FadeInAudioSource(AudioSource aud, float max_vol, float fade_in_rate)
    {
        aud.Play();
        while (aud.volume < max_vol)
        {
            yield return null;
            aud.volume = Mathf.Lerp(aud.volume, max_vol, Time.deltaTime * fade_in_rate);
        }
    }
    IEnumerator FadeOutAudioSource(AudioSource aud, float min_vol, float fade_out_rate)
    {
        while (aud.volume > 0)
        {
            yield return null;
            aud.volume = Mathf.Lerp(aud.volume, min_vol, Time.deltaTime * fade_out_rate);

        }
        aud.Stop();
    }

}
