using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroCubeScript : MonoBehaviour {

    protected EventManager eventManager;
    private GameObject spawnPoint;
    private ulong delayTime;
    private GameObject player;
    private AudioSource distressCall;
    private bool teleported;
    private bool fadedOut;
    Color cameraColor;
    float power;
    float volume;
    float fogIntensity;
    float timeStart;
    float textTimeStart;
    private Color previousColour;
    private bool subtitlesFinished = false;
    private Color textColor;

    public GameObject flashlight;
    public Camera cameraLeft;
    public Camera cameraRight;
    public MainAudioController audioController;
    public GameObject subtitles;
    public Text subtitleText;

    // Use this for initialization
    void Start () {
        delayTime = 1;
        teleported = false;
        fadedOut = false;
        timeStart = 0;
        textTimeStart = 0;
        //audioController.m_Aud_1.volume = 0f;
        eventManager = FindObjectOfType<EventManager>();
        player = eventManager.player;
        spawnPoint = eventManager.playerSpawnPoint;
        //distressCall = GetComponentInChildren<AudioSource>();
        //distressCall.Play(44100 * delayTime);
        player.GetComponent<OVRPlayerController>().Acceleration = 0;
        flashlight.SetActive(false);
        subtitleText = subtitles.GetComponentInChildren<Text>();

        cameraLeft.nearClipPlane = 0.01f;
        cameraLeft.farClipPlane = 0.2f;
        cameraRight.nearClipPlane = 0.01f;
        cameraRight.farClipPlane = 0.2f;
        previousColour = cameraLeft.backgroundColor; //(18,18,18)
        cameraLeft.backgroundColor = Color.black;
        cameraRight.backgroundColor = Color.black;

    }

    public void SubtitlesFinish()
    {
        subtitlesFinished = true;
    }

    private void Update()
    {

        float distressTime = Time.time;
        //if (!(distressCall.isPlaying) && !teleported)
        if (subtitlesFinished && !teleported)
        {
            TeleportPlayer();
            player.GetComponent<OVRPlayerController>().Acceleration = 0.1f;
            flashlight.SetActive(true);
            //audioController.SetActive(true);


            //cameraLeft.nearClipPlane = 0.3f;
            //cameraRight.nearClipPlane = 0.3f;

            teleported = true;
        }


        if (teleported && !fadedOut)
        {
            Debug.Log("fading");
            fogIntensity = Mathf.Lerp(1.0f, 0.25f, timeStart);
            cameraColor = Color.Lerp(Color.black, previousColour, timeStart);
            power = Mathf.Lerp(0.3f, 20.0f, timeStart);
            //volume = Mathf.Lerp(0.0f, 0.f, timeStart);
            textColor = Color.Lerp(subtitleText.color, Color.clear, textTimeStart);

            timeStart += Time.deltaTime * 0.2f;
            textTimeStart += Time.deltaTime * 0.01f;

            cameraLeft.farClipPlane = power;
            cameraRight.farClipPlane = power;
            cameraLeft.backgroundColor = cameraColor;
            cameraRight.backgroundColor = cameraColor;
            RenderSettings.fogColor = cameraColor;
            subtitleText.color = textColor;
            RenderSettings.fogDensity = fogIntensity;
            //audioController.m_Aud_1.volume = volume;

            //Debug.Log(RenderSettings.fogDensity);
            if (fogIntensity < 0.26 && cameraColor == previousColour)
            {
                subtitles.SetActive(false);
                Debug.Log(" done fading");
                fadedOut = true;
            }
        }
    }


    void TeleportPlayer()
    {
       player.transform.position = spawnPoint.transform.position;
       player.transform.rotation = spawnPoint.transform.rotation;
    }

    void lerpFar()
    {
        float t = 0f;
        float duration = 30000f;
        while (t <= 1)
        {
        
            float power = Mathf.Lerp(0.02f, 20f, t);
            //Debug.Log("Power" + power);
            t += Time.deltaTime / duration;
            cameraLeft.farClipPlane = power;
            cameraRight.farClipPlane = power;
        }
     
    }
}
