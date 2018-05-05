﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HouseCellarScript : MonoBehaviour
{


    protected EventManager eventManager;
    private ulong delayTime;
    private GameObject player;
    private bool fadedOut;
    Color cameraColor;
    float power;
    float fogIntensity;
    float timeStart;
    float textTimeStart;
    private Color previousColour;
    private Color textColor;
    private bool triggered;
    public GameObject houseMonster;
    public GameObject houseLight;
    public GameObject houseAnimation;
    public TrackingSpaceController trackingController;
    public GameObject LookBack;
    public AudioSource DeathAudio;

    public GameObject flashlight;
    
    public Camera cameraLeft;
    public Camera cameraRight;
    public GameObject audioController;
    public GameObject subtitles;
    public Text subtitleText;

    private bool subsOn;
    private bool disabled;
    private bool firstBool;
    private bool secondBool;
    private bool thirdBool;
    private bool fourthBool;

    public Light MainLight;
    public Light RimLight;

    private bool flashlightTrickery;
    private bool flashlightTrickery2;
    private bool flashlightTrickery3;
    private bool flashlightTrickery4;
    private bool flashlightTrickery5;

    private bool viv1;
    private bool viv2;
    private bool viv3;
    private bool viv4;
    private bool viv5;
    private bool viv6;
    private bool viv7;
    private bool viv8;

    private float flashlightStartTime;
    private float flashlightCurrentTime;
    private bool vivving;

    private float cellarStartTime;
    private float cellarCurrentTime;
    // Use this for initialization
    void Start()
    {
        vivving = false;
        disabled = false;
        subsOn = false;
        triggered = false;
        delayTime = 1;
        fadedOut = false;
        timeStart = 0;
        textTimeStart = 0;
        eventManager = FindObjectOfType<EventManager>();
        player = eventManager.player;
        subtitleText = subtitles.GetComponentInChildren<Text>();
        cellarStartTime = 10000f;
        houseMonster.SetActive(false);
        houseLight.SetActive(false);
        houseAnimation.GetComponent<Animator>().enabled = false;
        
        firstBool = false;
        secondBool = false;
        thirdBool = false;
        fourthBool = false;
        flashlightTrickery = false;
        flashlightTrickery2 = false;
        flashlightTrickery3 = false;
        flashlightTrickery4 = false;
        flashlightTrickery5 = false;

        viv1 = true;
        viv2 = true;
        viv3 = true;
        viv4 = true;
        viv5 = true;
        viv6 = true;
        viv7 = true;
        viv8 = true;

        flashlightStartTime = 10000f;

        cameraLeft.nearClipPlane = 0.01f;
        //cameraLeft.farClipPlane = 0.2f;
        cameraRight.nearClipPlane = 0.01f;
        //cameraRight.farClipPlane = 0.2f;
        //previousColour = cameraLeft.backgroundColor; //(18,18,18)
        //cameraLeft.backgroundColor = Color.black;
        //cameraRight.backgroundColor = Color.black;



    }

    private void Update()
    {

        if (vivving)
        {
            OculusHaptics[] OculusHapticsComponent;
            OculusHapticsComponent = player.GetComponentsInChildren<OculusHaptics>();
            OculusHapticsComponent[0].Vibrate(VibrationForce.Hard);
            OculusHapticsComponent[1].Vibrate(VibrationForce.Hard);

        }


        cellarCurrentTime = Time.time;
        if ( triggered && !disabled)
        {
            cellarStartTime = Time.time;
            flashlight.SetActive(false);
            player.GetComponent<OVRPlayerController>().Acceleration = 0.05f;
            previousColour = cameraLeft.backgroundColor;
            DeathAudio.Play();
            disabled = true;


        }


        if ((triggered && !fadedOut) && (cellarCurrentTime > cellarStartTime + 3) && !firstBool)
        {
            houseLight.SetActive(true);
            houseAnimation.GetComponent<Animator>().enabled = true;

            trackingController.VRLookAT(houseMonster.transform, 0.5f);
            firstBool = true;
        }
        if ((triggered && !fadedOut) && (cellarCurrentTime > cellarStartTime + 3.25) && !secondBool)
        {
            houseMonster.SetActive(true);
            //houseLight.SetActive(true);
            //trackingController.VRLookAT(LookBack.transform, 0.75f);
            secondBool = true;
        }
        if ( (triggered && !fadedOut) && (cellarCurrentTime > cellarStartTime + 4.0) )
        {
            //Debug.Log("time: " + cellarCurrentTime);
            //Debug.Log("viv: " + vivving);
            //vivving = true;
            //trackingController.VRLookAT(LookBack.transform, 0.75f);
            Debug.Log("fading");
            fogIntensity = Mathf.Lerp(0.25f, 1f, timeStart);
            cameraColor = Color.Lerp(previousColour, Color.black, timeStart);
            power = Mathf.Lerp(20.0f, 0.015f, timeStart);
            textColor = Color.Lerp(Color.clear, subtitleText.color, textTimeStart);

            timeStart += Time.deltaTime * 0.6f;
            textTimeStart += Time.deltaTime * 0.01f;

            cameraLeft.farClipPlane = power;
            cameraRight.farClipPlane = power;
            cameraLeft.backgroundColor = cameraColor;
            cameraRight.backgroundColor = cameraColor;
            RenderSettings.fogColor = cameraColor;
            subtitleText.color = textColor;
            RenderSettings.fogDensity = fogIntensity;

            Debug.Log(RenderSettings.fogDensity);
            if (fogIntensity > 0.7 && !subsOn)
            {
                subtitles.SetActive(true);
                houseLight.SetActive(true);


                subsOn = true;
            }
            if (fogIntensity > 0.95 && cameraColor == Color.black)
            {
                houseMonster.SetActive(false);
                houseLight.SetActive(false);
                player.GetComponent<OVRPlayerController>().Acceleration = 0f;
                fadedOut = true;
                
            }
        }
        if ((triggered && fadedOut) && (cellarCurrentTime > cellarStartTime + 22.5) && !thirdBool)
        {
            eventManager.cameraLeft.GetComponent<GameOverGlitching>().startGlitching();
            eventManager.cameraRight.GetComponent<GameOverGlitching>().startGlitching();
            eventManager.GameOver();
            thirdBool = true;
        }

        if ( (cellarCurrentTime > cellarStartTime + 16.4f) && viv1)
        {
            vivving = true;
            viv1 = false;
        }
        if ((cellarCurrentTime > cellarStartTime + 16.8f) && viv2)
        {
            vivving = false;
            viv2 = false;
        }
        if ((cellarCurrentTime > cellarStartTime + 17.5f) && viv3)
        {
            vivving = true;
            viv3 = false;
        }
        if ((cellarCurrentTime > cellarStartTime + 18f) && viv4)
        {
            vivving = false;
            viv4 = false;
        }
        if ((cellarCurrentTime > cellarStartTime + 19.7f) && viv5)
        {
            vivving = true;
            viv5 = false;
        }
        if ((cellarCurrentTime > cellarStartTime + 21f) && viv6)
        {
            vivving = false;
            viv6 = false;
        }




    }


    private void OnTriggerEnter(Collider other)
    {


        if (other == GameObject.FindGameObjectWithTag("Player").GetComponent<Collider>())
        {
            if (!triggered)
            {
                triggered = true;
            }
        }
        
    }


}
