using System.Collections;
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
    public float walkingVelocityTrigger = 1f;
    public float runningVelocityTrigger = 2f;
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
        m_Transform = transform;
        m_ParentRigidBody = transform.root.GetComponentInChildren<Rigidbody>();
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
        float parent_velocity = m_ParentRigidBody.velocity.magnitude;
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
                    else if(HumanMotion.clip != m_StoneRun && parent_velocity > runningVelocityTrigger)
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

    void Update ()
    {
        UpdateHumanMotion();

        double horizontalSpeed = m_ParentRigidBody.velocity.magnitude;

        if (horizontalSpeed < walkingVelocityTrigger/2f)
        {
            if (HumanMotion.volume != 0f)
            {
                OnHumanAudioEmission(0.5f);
            }
            HumanMotion.volume = 0f;
        }
        else if(horizontalSpeed > walkingVelocityTrigger)
        {
            if (!HumanMotion.isPlaying)
            {
                HumanMotion.Play();
            }
            if(HumanMotion.volume == 0f)
            {
                if (horizontalSpeed < runningVelocityTrigger)
                {
                    OnHumanAudioEmission(0.5f);
                }
                else
                {
                    OnHumanAudioEmission(1f);
                }
            }
            if (HumanMotion.volume < 1f)
                HumanMotion.volume = 1f;
        }

        //Breathing
        if (horizontalSpeed > runningVelocityTrigger)
        {
            m_Breathing.enabled = true;
            m_Breathing.loop = true;
            m_Breathing.Play();
            m_Breathing.volume = 0;
        }
        
        if (m_Breathing.isPlaying)
        {
            if(horizontalSpeed > runningVelocityTrigger)
            {
                m_Breathing.volume = Mathf.Lerp(0f, 1f, Time.deltaTime * 0.3f);
            }
            else
            {
                m_Breathing.volume = Mathf.Lerp(1f, 0f, Time.deltaTime * 0.5f);
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
