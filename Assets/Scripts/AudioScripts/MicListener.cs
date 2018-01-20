using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class MicListener : MonoBehaviour {

    // Use this for initialization
    
    private AudioSource aud;
    private string mic;
    private int audSamples = 44100;
    private List<string> micOptions = new List<string>();
    void Start()
    {
        aud = GetComponent<AudioSource>();
        foreach (string device in Microphone.devices)
        {
            if (mic == null)
            {
                mic = device;
            }
            micOptions.Add(device);
        }
        StartMic();
    }

    void StartMic()
    {
        aud.Stop();
        aud.clip = Microphone.Start(mic, true, 10, audSamples);
        aud.loop = true;
        if (Microphone.IsRecording(mic))
        {
            while(!(Microphone.GetPosition(mic)> 0))
            {
                Debug.Log("Started Recording");
            }
            aud.Play();
        }
        else
        {
            Debug.Log("Mic error");
        }
    }

    // Update is called once per frame
    void Update () {
        
    }
    
}
