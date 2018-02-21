using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Heartbeat : MonoBehaviour
{
    public float m_Heartbeat;
    public AudioClip m_BeatA;
    public AudioClip m_BeatB;
    public AudioSource m_HeartbeatAudioSrc;
    private float clipTimeA;
    private float clipTimeB;
    private int m_BaseHeartrate = 140;

    // Use this for initialization
    void Start () {
        clipTimeA = m_BeatA.length;
        clipTimeB = m_BeatB.length;
        m_Heartbeat = 65;
        m_HeartbeatAudioSrc.enabled = true;
        m_HeartbeatAudioSrc.Stop();
        m_HeartbeatAudioSrc.playOnAwake = false;
        m_HeartbeatAudioSrc.loop = false;
        StartCoroutine(HeartbeatCoroutine());
        StartCoroutine(getHeartRate());
    }

    IEnumerator getHeartRate()
    {
        Debug.Log("Doing Something");
        using (UnityWebRequest webaddress = UnityWebRequest.Get("172.23.184.234:5005"))
        {
            yield return webaddress.SendWebRequest();

            if (webaddress.isNetworkError || webaddress.isHttpError)
            {
                Debug.Log("Get Request Error");
            }
            else
            {
                Debug.Log("Data Receieved");
                byte[] results = webaddress.downloadHandler.data;
                Debug.Log(results);
            }
        }
    }


    // Update is called once per frame
    void Update () {
	}

    IEnumerator HeartbeatCoroutine()
    {
        m_HeartbeatAudioSrc.loop = false;
        while (true)
        {
            m_HeartbeatAudioSrc.clip = m_BeatA;
            m_HeartbeatAudioSrc.Play();
            yield return new WaitForSeconds(clipTimeA);
            m_HeartbeatAudioSrc.Stop();
            float delayTimeA;
            if (((60/m_Heartbeat) - clipTimeA) <= 0)
            {
                delayTimeA = m_BaseHeartrate;
            }
            else
            {
                delayTimeA = (60 / m_Heartbeat) - clipTimeA;
            }
            yield return new WaitForSeconds((Mathf.Pow(delayTimeA, 2)));

            m_HeartbeatAudioSrc.clip = m_BeatB;
            m_HeartbeatAudioSrc.Play();
            yield return new WaitForSeconds(clipTimeB);
            m_HeartbeatAudioSrc.Stop();
            float delayTimeB;
            //Debug.Log(m_Heartbeat-clipTimeB);
            if (((60/m_Heartbeat) - clipTimeB) <= 0)
            {
                delayTimeB = m_BaseHeartrate;
            }
            else
            {
                delayTimeB = (60 / m_Heartbeat) - clipTimeB;
            }
            yield return new WaitForSeconds(delayTimeB*(7/8));
        }

    }

}
