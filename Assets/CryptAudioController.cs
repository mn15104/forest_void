using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CryptAudioController : MonoBehaviour {

    public AudioSource ambientMusic;
    private EventManager eventManager;

	// Use this for initialization
	void Start () {
        eventManager = FindObjectOfType<EventManager>();
        eventManager.NotifyYellowKeyPickup.NotifyEventOccurred += PlayCryptAudio;
    }

    // Update is called once per frame
    void Update () {
		
	}

    void PlayCryptAudio(bool lightOn)
    {
        ambientMusic.Play(44100);
    }
}
