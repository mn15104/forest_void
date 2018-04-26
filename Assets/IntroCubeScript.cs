using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCubeScript : MonoBehaviour {

    protected EventManager eventManager;
    private GameObject spawnPoint;
    private ulong delayTime;
    private GameObject player;
    private AudioSource distressCall;
    private bool teleported;
    private Color previousColour;

    public GameObject flashlight;
    public Camera cameraLeft;
    public Camera cameraRight;
    public GameObject audioController;
    public GameObject subtitles;
    

    // Use this for initialization
    void Start () {
        delayTime = 1;
        teleported = false;
        audioController.SetActive(false);
        eventManager = FindObjectOfType<EventManager>();
        player = eventManager.player;
        spawnPoint = eventManager.playerSpawnPoint;
        distressCall = GetComponentInChildren<AudioSource>();
        distressCall.Play(44100 * delayTime);
        player.GetComponent<OVRPlayerController>().Acceleration = 0;
        flashlight.SetActive(false);

        cameraLeft.nearClipPlane = 0.01f;
        cameraLeft.farClipPlane = 0.5f;
        cameraRight.nearClipPlane = 0.01f;
        cameraRight.farClipPlane = 0.5f;
        previousColour = cameraLeft.backgroundColor;
        cameraLeft.backgroundColor = Color.black;
        cameraRight.backgroundColor = Color.black;



    }

    private void Update()
    {

        
        if (!(distressCall.isPlaying) && !teleported)
        {
            TeleportPlayer();
            player.GetComponent<OVRPlayerController>().Acceleration = 0.1f;
            flashlight.SetActive(true);
            audioController.SetActive(true);
            subtitles.SetActive(false);



            //            cameraLeft.nearClipPlane = 0.3f;
            //           cameraRight.nearClipPlane = 0.3f;


            //            float origtime = Time.time;
            //           float currentTime = origtime; 
            //          while(currentTime - origtime < 2f)
            //         {
            //            currentTime += Time.deltaTime;
            //       }

            //lerpFar();
            cameraLeft.backgroundColor = previousColour;
            cameraRight.backgroundColor = previousColour;

            StartCoroutine(LerpFar());


            //cameraLeft.farClipPlane = 20f;
            //cameraRight.farClipPlane = 20f;



            teleported = true;
        }
    }


    void TeleportPlayer()
    {
       player.transform.position = spawnPoint.transform.position;
    }

    //void lerpFar()
    //{
    //    float t = 0f;
    //    float duration = 300f;
    //    while (t <= 1)
    //    {
        
    //        float power = Mathf.Lerp(0.02f, 20f, t);
    //        //Debug.Log("Power" + power);
    //        t += Time.deltaTime / duration;
    //        cameraLeft.farClipPlane = power;
    //        cameraRight.farClipPlane = power;
    //    }
     
    //}

    IEnumerator LerpFar()
    {
        //print(Time.time);
        for (int i = 2; i < 2000; i ++)
        {
        cameraLeft.farClipPlane = i/100;
        cameraRight.farClipPlane = i/100;
        yield return new WaitForSeconds(0.2f);
        }

    }
}
