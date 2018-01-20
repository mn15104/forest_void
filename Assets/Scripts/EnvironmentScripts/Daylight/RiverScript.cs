using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Fog to be enabled to be trigggered from beginning of chase scene
public class RiverScript : MonoBehaviour {

    public AudioClip m_NormalRiverAudio;
    public AudioClip m_ViolentRiverAudio;
    private float m_minRiverHeight = 1.3f;
    private float m_maxRiverHeight = 3f;
    public GameObject m_Water;
    private AudioSource m_RiverAudio;
    void OnEnable()
    {
        AppleScript_2.OnAlert += EnableFog;
        AppleScript_2.OnAlert += EnableStorm;
    }
    void OnDisable()
    {
        AppleScript_2.OnAlert -= EnableFog;
        AppleScript_2.OnAlert -= EnableStorm;
    }

    // Use this for initialization
    void Start () {
        m_RiverAudio = GetComponent<AudioSource>();
        m_RiverAudio.clip = m_NormalRiverAudio;
        m_RiverAudio.volume = 1;
        m_RiverAudio.Play();
        Vector3 scale = m_Water.transform.localScale;
        scale.y = m_minRiverHeight;
        m_Water.transform.localScale = scale;
        foreach (ParticleSystem particle in GetComponentsInChildren<ParticleSystem>())
        {
            particle.enableEmission = false;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void EnableStorm(GameObject human)
    {
        m_RiverAudio.clip = m_ViolentRiverAudio;
        m_RiverAudio.volume = 1;
        m_RiverAudio.Play();
        Vector3 scale = m_Water.transform.localScale;
        scale.y = m_maxRiverHeight;
        m_Water.transform.localScale = scale;
    }

    void EnableFog(GameObject human)
    {
        foreach(ParticleSystem particle in GetComponentsInChildren<ParticleSystem>())
        {
            particle.enableEmission = true;
        }
    }
}
