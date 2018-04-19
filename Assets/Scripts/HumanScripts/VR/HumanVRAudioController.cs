﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public enum TerrainType
{
    GRASS,
    STONE,
    MUD,
    GRAVEL,
    WOOD
}

public class HumanVRAudioController : MonoBehaviour {


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
    private Transform m_Transform;
    private Rigidbody m_ParentRigidBody;
    private HumanVRController m_humanVRController;
    private float m_introTimeForBreathing;
    private float m_maxRunTime;
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
        m_TerrainTypeDictionary.Add(8, TerrainType.GRAVEL);
        m_TerrainTypeDictionary.Add(9, TerrainType.MUD);
        m_TerrainTypeDictionary.Add(10, TerrainType.GRASS);
        m_TerrainTypeDictionary.Add(11, TerrainType.GRASS);
        m_TerrainTypeDictionary.Add(12, TerrainType.GRAVEL);
        m_TerrainTypeDictionary.Add(13, TerrainType.STONE);
        m_TerrainTypeDictionary.Add(14, TerrainType.STONE);
        m_TerrainTypeDictionary.Add(15, TerrainType.GRASS);

        HumanMotion.clip = m_StoneRun;
        HumanMotion.loop = true;
    }

    // Use this for initialization
    void Start () {
        m_CurrentTerrain = Terrain.activeTerrain;
        m_humanVRController = GetComponentInParent<HumanVRController>();
        m_Transform = transform;
        m_ParentRigidBody = GetComponentInParent<Rigidbody>();
        m_Breathing.enabled = false;
        m_Breathing.loop = true;
        m_Breathing.volume = 0;
        m_maxRunTime = m_humanVRController.GetMaxRunTime();
        m_introTimeForBreathing = m_maxRunTime / 2;
    }
  
    private void UpdateHumanMotion()
    {

        int textureIndex = GetMainTexture(transform.position);
        TerrainType currentTerrainType = m_TerrainTypeDictionary[textureIndex];
        switch (currentTerrainType)
        {
            case TerrainType.GRASS:
                if (HumanMotion.clip != m_GrassWalk || HumanMotion.clip != m_GrassRun)
                {
                    if (HumanMotion.clip != m_GrassWalk && (m_humanVRController.GetPlayerMoveState() == PlayerMoveState.WALKING || m_humanVRController.GetPlayerMoveState() == PlayerMoveState.CROUCHING))
                    {
                        HumanMotion.clip = m_GrassWalk;
                    }
                    else if (HumanMotion.clip != m_GrassRun && m_humanVRController.GetPlayerMoveState() == PlayerMoveState.RUNNING)
                    {
                        HumanMotion.clip = m_GrassRun;
                    }
                }
                break;
            case TerrainType.STONE:
                if (HumanMotion.clip != m_StoneRun || HumanMotion.clip != m_StoneWalk)
                {
                    if (HumanMotion.clip != m_StoneWalk && m_humanVRController.GetPlayerMoveState() == PlayerMoveState.WALKING)
                    {
                        HumanMotion.clip = m_StoneWalk;
                    }
                    else if(HumanMotion.clip != m_StoneRun && m_humanVRController.GetPlayerMoveState() == PlayerMoveState.RUNNING)
                    {
                        HumanMotion.clip = m_StoneRun;
                    }
                }
                break;
            case TerrainType.GRAVEL:
                if (HumanMotion.clip != m_GravelWalk || HumanMotion.clip != m_GravelRun)
                {
                    if (HumanMotion.clip != m_GrassWalk && m_humanVRController.GetPlayerMoveState() == PlayerMoveState.WALKING)
                    {
                        HumanMotion.clip = m_GravelWalk;
                    }
                    else if (HumanMotion.clip != m_GravelRun && m_humanVRController.GetPlayerMoveState() == PlayerMoveState.RUNNING)
                    {
                        HumanMotion.clip = m_GravelRun;
                    }
                }
                break;
            case TerrainType.MUD:
                if (HumanMotion.clip != m_MudRun || HumanMotion.clip != m_MudWalk)
                {
                    if (HumanMotion.clip != m_MudWalk && m_humanVRController.GetPlayerMoveState() == PlayerMoveState.WALKING)
                    {
                        HumanMotion.clip = m_MudWalk;
                    }
                    else if (HumanMotion.clip != m_MudRun && m_humanVRController.GetPlayerMoveState() == PlayerMoveState.RUNNING)
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

    void Update ()
    {
        UpdateHumanMotion();

        double horizontalSpeed = Normalize3Dto2D(m_ParentRigidBody.velocity);
        
        if (HumanMotion.isPlaying && horizontalSpeed < 0.2f)
        {
            if (HumanMotion.volume != 0f)
            {
                OnHumanAudioEmission(0.5f);
            }
            HumanMotion.volume = 0f;
        }
        else if(HumanMotion.clip != null && horizontalSpeed > 0.4f)
        {
            if (!HumanMotion.isPlaying)
            {
                HumanMotion.Play();
            }
            if(HumanMotion.volume == 0f)
            {
                if (m_humanVRController.GetPlayerMoveState() == PlayerMoveState.WALKING)
                {
                    OnHumanAudioEmission(0.5f);
                }
                else if (m_humanVRController.GetPlayerMoveState() == PlayerMoveState.RUNNING)
                {
                    OnHumanAudioEmission(1f);
                }
            }
            if (HumanMotion.volume < 1f)
                HumanMotion.volume += Time.deltaTime * 0.5f;
        }

        //Breathing
        float timeSpentRunning = m_humanVRController.GetTimeSpentRunning();
        if (timeSpentRunning > m_introTimeForBreathing 
           && !m_Breathing.isPlaying)
        {
            Debug.Log("Breathing is playing!");
            m_Breathing.enabled = true;
            m_Breathing.loop = true;
            m_Breathing.Play();
            m_Breathing.volume = 0;
        }
        if (m_Breathing.isPlaying)
        {
             m_Breathing.volume = ((timeSpentRunning - m_introTimeForBreathing) / m_maxRunTime) * 2;
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
