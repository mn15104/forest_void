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
    public AudioClip m_GameOver;
    public AudioSource m_MainAmbienceAudioSrc;
    public AudioSource m_EventAmbienceAudioSrc;
    public AudioSource m_SFXAudioSrc;
    //Light
    private float skyboxRotationRate = 4f;
    //Players
    private HumanController human;
    private HumanVRController humanvr;
    //Monster
    private MonsterState m_MonsterState;
    private MonsterAI m_Monster;

    private void OnEnable()
    {
        m_Monster = FindObjectOfType<MonsterAI>();
        m_Monster.OnMonsterStateChange += MonsterStateChange;
    }

    private void OnGUI()
    {
        //Intro
        if (alpha > 0f)
        {
            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
            GUI.depth = -1000;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeIntro);
            alpha += fadeDir * fadeSpeed * Time.deltaTime;
            alpha = Mathf.Clamp01(alpha);
        }
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * skyboxRotationRate);

        Texture2D t;
        Color currentBlendColor;
        Color toColor;

        if (m_MonsterState == MonsterState.CHASE)
        {
            t = new Texture2D(1, 1);
            t.SetPixel(0, 0, Color.white);
            currentBlendColor = new Color(1, 0, 0, 0.25f);
            //Color fromColor = new Color( 1, 0, 0, 1 ); 
            toColor = new Color(0, 0, 0, 0);

            // Now each GUI draw the texture and blend it in. 
            currentBlendColor = Color.Lerp(currentBlendColor, toColor, Mathf.PingPong(Time.time, 0.75f));
            // Set GUI color 
            GUI.color = currentBlendColor;
            // Draw fade 
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), t, ScaleMode.StretchToFill);
        }

        if (m_MonsterState == MonsterState.GAMEOVER)
        {
            t = new Texture2D(1, 1);
            t.SetPixel(0, 0, Color.white);
            currentBlendColor = new Color(0, 0, 0, 0);
            //Color fromColor = new Color( 1, 0, 0, 1 ); 
            toColor = new Color(0, 0, 0, 1);

            // Now each GUI draw the texture and blend it in. 
            currentBlendColor = Color.Lerp(currentBlendColor, toColor, 1);
            // Set GUI color 
            GUI.color = currentBlendColor;
            // Draw fade 
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), t, ScaleMode.StretchToFill);
        }
    }

  
    // Use this for initialization
    void Start () {
        m_Canvas = FindObjectOfType<Canvas>();
        human = FindObjectOfType<HumanController>();
        humanvr = FindObjectOfType<HumanVRController>();
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

    private void MonsterStateChange(MonsterState monsterStateChange)
    {
        if (monsterStateChange == MonsterState.CHASE)
            TriggerMonsterChase();
        else if (monsterStateChange == MonsterState.HIDDEN_MOVING)
            TriggerMonsterHidden();
        else if (monsterStateChange == MonsterState.APPEAR)
            TriggerMonsterAppear();
        else if (monsterStateChange == MonsterState.GAMEOVER)
            TriggerMonsterGameOver();
    }

    // Update is called once per frame
    void Update () {
        
    }

    public void TriggerMonsterGameOver()
    {
        if (m_MonsterState != MonsterState.GAMEOVER)
        {
            m_MainAmbienceAudioSrc.enabled = true;
            m_MainAmbienceAudioSrc.clip = m_GameOver;
            m_MainAmbienceAudioSrc.Play();
            m_MonsterState = MonsterState.GAMEOVER;
        }
    }

    public void TriggerMonsterHidden()
    {
        if (m_MonsterState != MonsterState.HIDDEN_IDLE && m_MonsterState != MonsterState.HIDDEN_MOVING)
        {
            StartCoroutine(HiddenMusic());
            m_MonsterState = MonsterState.HIDDEN_MOVING;
        }
    }

    public void TriggerMonsterChase()
    {
        if (m_MonsterState != MonsterState.CHASE)
        {
            m_SFXAudioSrc.enabled = true;
            m_SFXAudioSrc.clip = m_MonsterScare;
            m_SFXAudioSrc.Play();
            StartCoroutine(ChaseMusic());
            m_MonsterState = MonsterState.CHASE;
        }
    }

    public void TriggerMonsterAppear()
    {
        if (m_MonsterState != MonsterState.APPEAR)
        {
            m_EventAmbienceAudioSrc.enabled = true;
            m_EventAmbienceAudioSrc.clip = m_Tense;
            m_EventAmbienceAudioSrc.Play();
            StartCoroutine(ChaseMusic());
            m_MonsterState = MonsterState.APPEAR;
        }
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
