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
    //Audio

    public Material m_DaylightSkyboxMaterial;
    public Material m_DarknessSkyboxMaterial;
    public AudioClip m_AmbienceDaylight;
    public AudioClip m_AmbienceDarkness;
    public AudioClip m_EnterFarm;
    public AudioClip m_Chase;
    public AudioClip m_EndChase;
    public AudioClip m_EnteringDarkness;
    public AudioClip[] soundTrack;
    public AudioSource m_Ambience1AudioSrc;
    public AudioSource m_Ambience2AudioSrc;
    public AudioSource m_SFXAudioSrc;
    private int playlistIndex = 0;
    //Light
    public PostProcessingProfile DaylightProfile;
    public PostProcessingProfile DarknessProfile;
    public Color m_DaylightLighting;
    public Color m_DarknessLighting;
    private Light forestLight;
    private float skyboxRotationRate = 4f;
    //Players
    private HumanController human;
    private GameEventStage m_GameEventStage;
    //System
    private EventController m_EventController;
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
        m_GameEventStage = m_EventController.GetGameEventStage();
        m_Canvas = FindObjectOfType<Canvas>();
        human = FindObjectOfType<HumanController>();
        forestLight = GetComponentInChildren<Light>();
        m_Ambience1AudioSrc.volume = 1f;
        m_Ambience2AudioSrc.volume = 1f;
        m_SFXAudioSrc.volume = 1f;
        SetLightSettingsToDark(false);
    }
	
	// Update is called once per frame
	void Update () {

        m_GameEventStage = m_EventController.GetGameEventStage();

        switch (m_GameEventStage)
        {
            case GameEventStage.WalkingThroughDaylightForest:
                if(m_Ambience1AudioSrc.clip != m_AmbienceDaylight)
                {
                    m_Ambience1AudioSrc.clip = m_AmbienceDaylight;
                    m_Ambience1AudioSrc.loop = true;
                    m_Ambience1AudioSrc.Play();
                }
                break;
            case GameEventStage.EnterFarm:
                if (m_Ambience2AudioSrc.clip != m_EnterFarm)
                {
                    m_Ambience2AudioSrc.clip = m_EnterFarm;
                    m_Ambience2AudioSrc.loop = true;
                    m_Ambience2AudioSrc.volume = 0;
                    m_Ambience2AudioSrc.Play();
                }
                m_Ambience1AudioSrc.volume -= 0.01f;
                m_Ambience2AudioSrc.volume += 0.01f;
                break;
            case GameEventStage.ChasedToBridge:
                if (m_Ambience1AudioSrc.clip != m_Chase)
                {
                    m_Ambience1AudioSrc.volume = 1f;
                    m_Ambience2AudioSrc.Stop(); 
                    m_Ambience1AudioSrc.clip = m_Chase;
                    m_Ambience1AudioSrc.loop = true;
                    m_Ambience1AudioSrc.Play();
                    Debug.Log("PLAYING");
                }
                break;
            case GameEventStage.EndOfChase:
                if (m_Ambience1AudioSrc.clip == m_Chase)
                {
                    m_Ambience1AudioSrc.volume -= 0.02f;
                }
                break;
            case GameEventStage.EnteringDarknessForest:
                if (m_Ambience1AudioSrc.clip != m_AmbienceDarkness)
                {
                    m_SFXAudioSrc.Stop();
                    m_SFXAudioSrc.clip = m_EnteringDarkness;
                    m_SFXAudioSrc.loop = false;
                    m_SFXAudioSrc.Play();
                    m_Ambience1AudioSrc.volume = 1f;
                    m_Ambience1AudioSrc.Stop();
                    m_Ambience1AudioSrc.clip = m_AmbienceDarkness;
                    m_Ambience1AudioSrc.loop = true;
                    m_Ambience1AudioSrc.Play();
                }
                break;
        }
    }

    void PlayNextClip(){
        if (soundTrack.Length != 0)
        {
            playlistIndex = playlistIndex < soundTrack.Length - 1 ? playlistIndex + 1 : 0;
            m_Ambience1AudioSrc.clip = soundTrack[playlistIndex];
            m_Ambience1AudioSrc.Play();
        }
    }
    
    public void SetLightSettingsToDark(bool setDark)
    {
        if (setDark)
        {
      
            human.GetComponentInChildren<PostProcessingBehaviour>().profile = DarknessProfile;
            forestLight.color = m_DarknessLighting;
            RenderSettings.skybox = m_DarknessSkyboxMaterial;
            RenderSettings.fog = true;
            RenderSettings.fogDensity = 0.2f;
        }
        else
        {
            human.GetComponentInChildren<PostProcessingBehaviour>().profile = DaylightProfile;
            forestLight.color = m_DaylightLighting;
            RenderSettings.skybox = m_DaylightSkyboxMaterial;
            RenderSettings.fog = false;
        }
    }


}
