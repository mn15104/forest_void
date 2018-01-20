using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour {


    [Range(0, 60)] public float fireIntensity;
    [Range(0f, 0.2f)] private float fireSize;
    [Range(0f, 2f)] private float lightIntensity;
    [Range(0f, 50f)] private float lightSize;

    ParticleSystem m_particleSystem;
    Light m_light;
    float fireIntensityMax = 60f;
    float fireSizeMax = 0.2f;
    float lightIntensityMax = 2f;
    float lightSizeMax = 50f;
    float witherScale = 1.2f;
    float timeSinceFuelled = 0f;
    private List<AudioSource> fireAuds = new List<AudioSource>();
    // Use this for initialization
    void Start () {
        m_particleSystem = GetComponentInChildren<ParticleSystem>();
        m_light = GetComponentInChildren<Light>();
        var em = m_particleSystem.emission;
        em.enabled = true;
        var audio = GetComponents<AudioSource>();

        int delay = 0;
        foreach(var aud in audio)
        {
            var cliplength = aud.clip.length;
            fireAuds.Add(aud);
            aud.loop = true;
            aud.PlayDelayed(delay*(cliplength/2));
            delay++;
        }
    }

    void Wither()
    {
        float time = Time.deltaTime;
        fireIntensity -= time * witherScale;
    }

    void Fuel()
    {
        timeSinceFuelled = 0f;
        fireIntensity = fireIntensity > 40 ? 60 : fireIntensity + 20;
    }

	// Update is called once per frame
	void Update () {
        
        if(timeSinceFuelled > 20)
        {
            Wither();
        }
        
        var m_fireEmission = m_particleSystem.emission;
        var m_fireShape = m_particleSystem.shape;
        float normalizer = (fireIntensity / fireIntensityMax);
        m_fireEmission.rate = fireIntensity;
        var fireRange = fireSizeMax * normalizer;
        m_fireShape.radius = fireRange;
        //var light
        m_light.intensity = lightIntensityMax * normalizer;
        m_light.range = lightSizeMax * normalizer;
        foreach (var aud in fireAuds)
        {
            aud.volume = normalizer;
        }
        timeSinceFuelled += Time.deltaTime;


    }




}
