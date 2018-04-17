using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainAudioController : MonoBehaviour {

    private EventManager eventManager;
    public AudioSource m_Aud_1;
    public AudioSource m_Aud_2;
    public AudioSource m_Aud_SFX;
    public AudioSource m_Aud_Wind;
    public AudioClip m_WindClip;
    public AudioClip m_Stage1Clip;
    public AudioClip m_Stage2Clip;
    public AudioClip m_Stage3Clip;

    public float m_MaxVolume = 0.3f;
    public float m_FadeSpeed = 0.5f;

    private AudioClip currentlySelected;
    private EventManager.Stage currentAudioStage = EventManager.Stage.Intro;

    private void Awake() 
    {
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
        //FadeInAudioSource(m_Aud_Wind);
        //----------------
        m_Aud_2.Stop();
        m_Aud_2.clip = null;
        m_Aud_2.loop = true;
        //----------------
        m_Aud_SFX.Stop();
        m_Aud_SFX.clip = null;
        m_Aud_SFX.loop = false;
    }

    void StageAudio(EventManager.Stage stage)
    {
        Debug.Log("Stage Audio changing");
        switch (stage)
        {
            case EventManager.Stage.Intro:

                break;
            case EventManager.Stage.Stage1:
                if (currentAudioStage == EventManager.Stage.Intro)
                {
                    TransitionClip(m_Stage1Clip);
                    currentAudioStage = EventManager.Stage.Stage1;
                }
                break;
            case EventManager.Stage.Stage2:
                if(currentAudioStage == EventManager.Stage.Stage1)
                {
                    TransitionClip(m_Stage2Clip);
                    currentAudioStage = EventManager.Stage.Stage2;
                }
                break;
            case EventManager.Stage.Stage3:
                if (currentAudioStage == EventManager.Stage.Stage2)
                {
                    m_Aud_1.clip = m_Stage3Clip;
                    m_Aud_1.Play();
                    currentAudioStage = EventManager.Stage.Stage3;
                }
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
            case EventManager.Location.ToolShed:
                windOn = true;
                break;
        }
        if(windOn && !m_Aud_Wind.isPlaying)
        {
            StopAllCoroutines();
           // StartCoroutine(FadeInAudioSource(m_Aud_Wind));
        }
        else if(!windOn && m_Aud_Wind.isPlaying)
        {
            StopAllCoroutines();
         //   StartCoroutine(FadeOutAudioSource(m_Aud_Wind));
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

            StartCoroutine(FadeOutAudioSource(m_Aud_1, m_FadeSpeed));
            //FadeInAudioSource(m_Aud_2);
            StartCoroutine(FadeInAudioSource(m_Aud_2, m_MaxVolume, m_FadeSpeed));
        }
        else if (m_Aud_2.isPlaying)
        {
            m_Aud_1.clip = t_clip;
            StopAllCoroutines();
            StartCoroutine(FadeOutAudioSource(m_Aud_2, m_FadeSpeed));
            StartCoroutine(FadeInAudioSource(m_Aud_1, m_MaxVolume, m_FadeSpeed));
        }
    }

    //parms[0] is audiosource, parms[1] is max volume
    IEnumerator FadeInAudioSource(AudioSource aud, float max_vol, float fade_in_rate)
    {
        Debug.Log("Fading in");
        aud.Play();
        while (aud.volume < max_vol)
        {
            yield return null;
            aud.volume = Mathf.Lerp(aud.volume, max_vol, Time.deltaTime * fade_in_rate);
        }
    }
    IEnumerator FadeOutAudioSource(AudioSource aud, float fade_out_rate)
    {
        Debug.Log("Fading out");
        while (aud.volume > 0)
        {
            yield return null;
            aud.volume = Mathf.Lerp(aud.volume, 0, Time.deltaTime * fade_out_rate);

        }
        aud.Stop();
    }

}
