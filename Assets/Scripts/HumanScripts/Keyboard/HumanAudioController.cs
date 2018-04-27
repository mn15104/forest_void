﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class HumanAudioController : MonoBehaviour {


    public delegate void HumanAudioEmitter(float volumeEmitted);
    public static event HumanAudioEmitter OnHumanAudioEmission;

    
    private Dictionary<int, TerrainType> m_TerrainTypeDictionary = new Dictionary<int, TerrainType>();
    private Dictionary<TerrainType, float> m_TerrainVolumeDictionary = new Dictionary<TerrainType, float>();
    private Terrain m_CurrentTerrain;
    public AudioSource HumanMotion;
    public AudioSource m_Breathing;
    public AudioClip m_GrassRun;
    public AudioClip m_StoneRun;
    public AudioClip m_GrassWalk;
    public AudioClip m_StoneWalk;
    public AudioClip m_GravelWalk;
    public AudioClip m_GravelRun;
    public AudioClip m_MudWalk;
    public AudioClip m_MudRun;
    public AudioClip m_WoodRun;
    public AudioClip m_WoodWalk;
    public AudioClip m_CryptWalk;
    public AudioClip m_CryptRun;

    public float walkingVelocityTrigger = 1f;
    public float runningVelocityTrigger = 2.3f;
    private Transform m_Transform;
    private EventManager m_EventManager;
    private Rigidbody m_ParentRigidBody;
    private HumanController m_humanController;

    private void OnEnable()
    {
        m_TerrainTypeDictionary.Add(0, TerrainType.GRASS);
        m_TerrainTypeDictionary.Add(1, TerrainType.GRASS);
        m_TerrainTypeDictionary.Add(2, TerrainType.STONE);
        m_TerrainTypeDictionary.Add(3, TerrainType.STONE);
        m_TerrainTypeDictionary.Add(4, TerrainType.GRASS);
        m_TerrainTypeDictionary.Add(5, TerrainType.STONE);
        m_TerrainTypeDictionary.Add(6, TerrainType.STONE);
        m_TerrainTypeDictionary.Add(7, TerrainType.STONE);
        m_TerrainTypeDictionary.Add(8, TerrainType.MUD);
        m_TerrainTypeDictionary.Add(9, TerrainType.MUD);
        m_TerrainTypeDictionary.Add(10, TerrainType.GRASS);
        m_TerrainTypeDictionary.Add(11, TerrainType.GRASS);
        m_TerrainTypeDictionary.Add(12, TerrainType.GRAVEL);
        m_TerrainTypeDictionary.Add(13, TerrainType.GRASS);
        m_TerrainTypeDictionary.Add(14, TerrainType.STONE);
        m_TerrainTypeDictionary.Add(15, TerrainType.GRASS);

        HumanMotion.clip = m_StoneRun;
        HumanMotion.loop = true;
        m_EventManager = FindObjectOfType<EventManager>();
    }

    // Use this for initialization
    void Start () {
        m_CurrentTerrain = Terrain.activeTerrain;
        m_Transform = transform;
        m_ParentRigidBody = transform.root.GetComponentInChildren<Rigidbody>();
        m_Breathing.enabled = true;
        m_Breathing.loop = true;
        m_Breathing.volume = 0;
        m_Breathing.Play();
    }
  
    void LocationChange(EventManager.Location s)
    {

    }

    private void UpdateHumanMotion()
    {
        EventManager.Location currentLocation = m_EventManager.GetLocation();
        float parent_velocity = m_ParentRigidBody.velocity.magnitude;

        if (currentLocation != EventManager.Location.Forest &&
            currentLocation != EventManager.Location.Generator)
        {
            switch (currentLocation)
            {
                case EventManager.Location.Bridge:
                case EventManager.Location.Caravan:
                case EventManager.Location.ToolShed:
                    if (HumanMotion.clip != m_WoodWalk || HumanMotion.clip != m_WoodRun)
                    {
                        if (HumanMotion.clip != m_WoodWalk && parent_velocity > walkingVelocityTrigger && parent_velocity < runningVelocityTrigger)
                        {
                            HumanMotion.clip = m_WoodWalk;
                        }
                        else if (HumanMotion.clip != m_WoodRun && parent_velocity > runningVelocityTrigger)
                        {
                            HumanMotion.clip = m_WoodRun;
                        }
                    }
                    break;
                case EventManager.Location.Chapel:
                    if (HumanMotion.clip != m_StoneRun || HumanMotion.clip != m_StoneWalk)
                    {
                        if (HumanMotion.clip != m_StoneWalk && parent_velocity > walkingVelocityTrigger && parent_velocity < runningVelocityTrigger)
                        {
                            HumanMotion.clip = m_StoneWalk;
                        }
                        else if (HumanMotion.clip != m_StoneRun && parent_velocity > runningVelocityTrigger)
                        {
                            HumanMotion.clip = m_StoneRun;
                        }
                    }
                    break;
                case EventManager.Location.Crypt:
                    if (HumanMotion.clip != m_CryptWalk || HumanMotion.clip != m_CryptRun)
                    {
                        if (HumanMotion.clip != m_CryptWalk && parent_velocity > walkingVelocityTrigger && parent_velocity < runningVelocityTrigger)
                        {
                            HumanMotion.clip = m_CryptWalk;
                        }
                        else if (HumanMotion.clip != m_CryptRun && parent_velocity > runningVelocityTrigger)
                        {
                            HumanMotion.clip = m_CryptRun;
                        }
                    }
                    break;
                default:
                    Debug.Log("What location is this?!");
                    break;
            }
        }
        else
        {
            int textureIndex = GetMainTexture(transform.position);
            TerrainType currentTerrainType = m_TerrainTypeDictionary[textureIndex];
            switch (currentTerrainType)
            {
                case TerrainType.GRASS:
                    if (HumanMotion.clip != m_GrassWalk || HumanMotion.clip != m_GrassRun)
                    {
                        if (HumanMotion.clip != m_GrassWalk && parent_velocity > walkingVelocityTrigger && parent_velocity < runningVelocityTrigger)
                        {
                            HumanMotion.clip = m_GrassWalk;
                        }
                        else if (HumanMotion.clip != m_GrassRun && parent_velocity > runningVelocityTrigger)
                        {
                            HumanMotion.clip = m_GrassRun;
                        }
                    }
                    break;
                case TerrainType.STONE:
                    if (HumanMotion.clip != m_StoneRun || HumanMotion.clip != m_StoneWalk)
                    {
                        if (HumanMotion.clip != m_StoneWalk && parent_velocity > walkingVelocityTrigger && parent_velocity < runningVelocityTrigger)
                        {
                            HumanMotion.clip = m_StoneWalk;
                        }
                        else if (HumanMotion.clip != m_StoneRun && parent_velocity > runningVelocityTrigger)
                        {
                            HumanMotion.clip = m_StoneRun;
                        }
                    }
                    break;
                case TerrainType.GRAVEL:
                    if (HumanMotion.clip != m_GravelWalk || HumanMotion.clip != m_GravelRun)
                    {
                        if (HumanMotion.clip != m_GrassWalk && parent_velocity > walkingVelocityTrigger && parent_velocity < runningVelocityTrigger)
                        {
                            HumanMotion.clip = m_GravelWalk;
                        }
                        else if (HumanMotion.clip != m_GravelRun && parent_velocity > runningVelocityTrigger)
                        {
                            HumanMotion.clip = m_GravelRun;
                        }
                    }
                    break;
                case TerrainType.MUD:
                    if (HumanMotion.clip != m_MudRun || HumanMotion.clip != m_MudWalk)
                    {
                        if (HumanMotion.clip != m_MudWalk && parent_velocity > walkingVelocityTrigger && parent_velocity < runningVelocityTrigger)
                        {
                            HumanMotion.clip = m_MudWalk;
                        }
                        else if (HumanMotion.clip != m_MudRun && parent_velocity > runningVelocityTrigger)
                        {
                            HumanMotion.clip = m_MudRun;
                        }
                    }
                    break;
                default:
                    Debug.Log("WARNING: NO AUDIO SOURCE FOR CURRENT TERRAIN TYPE BELOW PLAYER.");
                    break;
            }
        }
    }

    void Update ()
    {
        
        UpdateHumanMotion();
      
        double horizontalSpeed = m_ParentRigidBody.velocity.magnitude;
        if (horizontalSpeed < (walkingVelocityTrigger/2f) )
        {
            HumanMotion.volume = 0f;
        }
        else if(horizontalSpeed > walkingVelocityTrigger)
        {
            if (!HumanMotion.isPlaying)
            {
                HumanMotion.Play();
            }
            if (HumanMotion.volume < 1f)
                HumanMotion.volume = 1f;
        }

        //Breathing
        if (m_Breathing.isPlaying)
        {
            if(horizontalSpeed > runningVelocityTrigger)
            {
                m_Breathing.volume = Mathf.Lerp(m_Breathing.volume, 1f, Time.deltaTime * 0.3f);
            }
            else
            {
                m_Breathing.volume = Mathf.Lerp(m_Breathing.volume, 0f, Time.deltaTime * 0.3f);
            }
        }
    }

    float[] GetTextureMix(Vector3 worldPos)
    {

        TerrainData terrainData = m_CurrentTerrain.terrainData;
        Vector3 terrainPos = m_CurrentTerrain.transform.position;

        int mapX = (int)(((worldPos.x - terrainPos.x) / terrainData.size.x) * terrainData.alphamapWidth);
        int mapZ = (int)(((worldPos.z - terrainPos.z) / terrainData.size.z) * terrainData.alphamapHeight);

        float[,,] splatmapData = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);

        float[] cellMix = new float[splatmapData.GetUpperBound(2) + 1];
        for (int n = 0; n < cellMix.Length; ++n)
        {
            cellMix[n] = splatmapData[0, 0, n];
        }

        return cellMix;
    }

    int GetMainTexture(Vector3 worldPos)
    {
        float[] mix = GetTextureMix(worldPos);
        float maxMix = 0;
        int maxIndex = 0;
        for (int n = 0; n < mix.Length; ++n)
        {
            if (mix[n] > maxMix)
            {
                maxIndex = n;
                maxMix = mix[n];
            }
        }
        return maxIndex;
    }


    double Normalize3Dto2D(Vector3 vector3)
    {
        return Math.Sqrt(Math.Pow(vector3.x, 2) + Math.Pow(vector3.z, 2));
    }

}