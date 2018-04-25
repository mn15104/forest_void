using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOutIntroScript : MonoBehaviour {


    protected EventManager eventManager;
    public float BeepTime;
    public float StartClipTime;
    private GameObject spawnPoint;
    private GameObject player;
    public float fadeOutSpeed;
    private bool fadedOut;
    private OVRCameraRig cameraRig;
    private AudioSource introClip;
    public GameObject audioController;

	// Use this for initialization
	void Start () {
        eventManager = FindObjectOfType<EventManager>();
        introClip = GetComponentInChildren<AudioSource>();
        player = eventManager.player;

        audioController.SetActive(false);
        cameraRig = player.GetComponentInChildren<OVRCameraRig>();
        cameraRig.usePerEyeCameras = false;
        spawnPoint = eventManager.playerSpawnPoint;
        fadedOut = false;
        introClip.Play();
    }
	
	// Update is called once per frame
	void Update () {
        if (!fadedOut)
        {
            if (introClip.isPlaying)
            {
                //if (GetComponent<Image>().color != Color.clear)
                //{
                //    Color.Lerp(GetComponent<Image>().color, Color.clear, fadeOutSpeed * Time.deltaTime);
                //}
            }
            else
            {
                fadedOut = true;
                cameraRig.usePerEyeCameras = true;
                TeleportPlayer();
                //audioController.SetActive(true);
            }
        }
    }


    void TeleportPlayer()
    {
        player.transform.position = spawnPoint.transform.position;
    }

}
