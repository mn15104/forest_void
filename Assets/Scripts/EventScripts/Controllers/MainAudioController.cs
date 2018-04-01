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
    private EventManager.Stage currentAudioStage = EventManager.Stage.Stage1;
    private void Awake() 
    {
        eventManager = FindObjectOfType<EventManager>();
        eventManager.NotifyRunStamina.NotifyEventOccurred += RunAudio;
        eventManager.NotifyLocation.NotifyEventOccurred += StructureAudio;
        eventManager.NotifyStage.NotifyEventOccurred += StageAudio;
        eventManager.NotifyHeartRate.NotifyEventOccurred += HeartRateAudio;
        //eventManager.BridgeCrossedEvent.TriggerEnterEvent += BridgeTriggerAudio; 

    }
    private void Start()
    {
        m_Aud_1.clip = m_Stage1Clip;
        m_Aud_1.loop = true;
        m_Aud_1.volume = 0;
        FadeInAudioSource(m_Aud_1);
        //----------------
        m_Aud_Wind.clip = m_WindClip;
        m_Aud_Wind.loop = true;
        m_Aud_Wind.volume = 0;
        FadeInAudioSource(m_Aud_Wind);
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
        switch (stage)
        {
            case EventManager.Stage.Stage1:

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


    void RunAudio(bool runStamina)
    {
        if (runStamina)
        {
            //Means is able to run again - not exhausted audio
            Debug.Log("Event: Stamina=True");
        }
        else
        {
            //No running left Audio - panting
            Debug.Log("Event: Stamina=False");
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
            StartCoroutine(FadeInAudioSource(m_Aud_Wind));
        }
        else if(!windOn && m_Aud_Wind.isPlaying)
        {
            StopAllCoroutines();
            StartCoroutine(FadeOutAudioSource(m_Aud_Wind));
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
            FadeOutAudioSource(m_Aud_1);
            FadeInAudioSource(m_Aud_2);
        }
        else if (m_Aud_2.isPlaying)
        {
            m_Aud_1.clip = t_clip;
            StopAllCoroutines();
            FadeOutAudioSource(m_Aud_2);
            FadeInAudioSource(m_Aud_1);
        }
    }

    IEnumerator FadeInAudioSource(AudioSource aud)
    {
        aud.Play();
        while (aud.volume < 1)
        {
            yield return null; 
            aud.volume += Time.deltaTime;
        }
    }
    IEnumerator FadeOutAudioSource(AudioSource aud)
    {
        while (aud.volume > 0)
        {
            yield return null;
            aud.volume -= Time.deltaTime;
        }
        aud.Stop();
    }

}
