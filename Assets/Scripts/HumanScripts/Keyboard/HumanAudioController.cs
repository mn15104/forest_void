using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class HumanAudioController : MonoBehaviour {

    private Dictionary<int, TerrainType> m_TerrainTypeDictionary = new Dictionary<int, TerrainType>();
    private Dictionary<TerrainType, float> m_TerrainVolumeDictionary = new Dictionary<TerrainType, float>();
    private Terrain m_CurrentTerrain;
    public AudioSource HumanMotion;
    public AudioClip m_GrassRun;
    public AudioClip m_StoneRun;
    public AudioClip m_GrassWalk;
    public AudioClip m_StoneWalk;
    public AudioClip m_GravelWalk;
    public AudioClip m_GravelRun;
    public AudioClip m_MudWalk;
    public AudioClip m_MudRun;
    public AudioClip m_WoodRun;
    public AudioSource m_Breathing;
    private Transform m_Transform;
    private Rigidbody m_ParentRigidBody;
    private HumanController m_humanController;
    private bool m_airborne = false;
    private float m_timeSpentRunning;
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
        m_TerrainTypeDictionary.Add(15, TerrainType.MUD);

        HumanMotion.clip = m_StoneRun;
        HumanMotion.loop = true;
    }

    // Use this for initialization
    void Start () {
        m_humanController = GetComponentInParent<HumanController>();
        m_Transform = transform;
        m_ParentRigidBody = GetComponentInParent<Rigidbody>();
        m_Breathing.volume = 0;
        m_Breathing.loop = true;
        m_CurrentTerrain = Terrain.activeTerrain;
        m_maxRunTime = m_humanController.GetMaxRunTime();
        m_introTimeForBreathing = m_maxRunTime / 2;
    }
    
    double Normalize3Dto2D(Vector3 vector3)
    {
        return Math.Sqrt(Math.Pow(vector3.x, 2) + Math.Pow(vector3.z, 2));
    }

    private void FixedUpdate()
    {
       
    }

    // GetTextureMix
    // returns an array containing the relative mix of textures
    // on the main terrain at this world position.
    // The number of values in the array will equal the number
    // of textures added to the terrain.
    // calculate which splat map cell the worldPos falls within (ignoring y)
    // get the splat data for this cell as a 1x1xN 3d array (where N = number of textures)
    // extract the 3D array data to a 1D array
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

    
    // GetMainTexture
    // returns the zero-based index of the most dominant texture
    // on the main terrain at this world position.
    // loop through each mix value and find the maximum
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
  
    private void UpdateHumanMotion()
    {

        int textureIndex = GetMainTexture(transform.position);
        TerrainType currentTerrainType = m_TerrainTypeDictionary[textureIndex];
        switch (currentTerrainType)
        {
            case TerrainType.GRASS:
                if (HumanMotion.clip != m_GrassWalk || HumanMotion.clip != m_GrassRun)
                {
                    if (HumanMotion.clip != m_GrassWalk && (m_humanController.GetPlayerMoveState() == PlayerMoveState.WALKING || m_humanController.GetPlayerMoveState() == PlayerMoveState.CROUCHING))
                    {
                        HumanMotion.clip = m_GrassWalk;
                    }
                    else if (HumanMotion.clip != m_GrassRun && m_humanController.GetPlayerMoveState() == PlayerMoveState.RUNNING)
                    {
                        HumanMotion.clip = m_GrassRun;
                    }
                }
                break;
            case TerrainType.STONE:
                if (HumanMotion.clip != m_StoneRun || HumanMotion.clip != m_StoneWalk)
                {
                    if (HumanMotion.clip != m_StoneWalk && (m_humanController.GetPlayerMoveState() == PlayerMoveState.WALKING || m_humanController.GetPlayerMoveState() == PlayerMoveState.CROUCHING))
                    {
                        HumanMotion.clip = m_StoneWalk;
                    }
                    else if(HumanMotion.clip != m_StoneRun && m_humanController.GetPlayerMoveState() == PlayerMoveState.RUNNING)
                    {
                        HumanMotion.clip = m_StoneRun;
                    }
                }
                break;
            case TerrainType.GRAVEL:
                if (HumanMotion.clip != m_GravelWalk || HumanMotion.clip != m_GravelRun)
                {
                    if (HumanMotion.clip != m_GrassWalk && m_humanController.GetPlayerMoveState() == PlayerMoveState.WALKING)
                    {
                        HumanMotion.clip = m_GravelWalk;
                    }
                    else if (HumanMotion.clip != m_GravelRun && m_humanController.GetPlayerMoveState() == PlayerMoveState.RUNNING)
                    {
                        HumanMotion.clip = m_GravelRun;
                    }
                }
                break;
            case TerrainType.MUD:
                if (HumanMotion.clip != m_MudRun || HumanMotion.clip != m_MudWalk)
                {
                    if (HumanMotion.clip != m_MudWalk && m_humanController.GetPlayerMoveState() == PlayerMoveState.WALKING)
                    {
                        HumanMotion.clip = m_MudWalk;
                    }
                    else if (HumanMotion.clip != m_MudRun && m_humanController.GetPlayerMoveState() == PlayerMoveState.RUNNING)
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
            HumanMotion.volume = 0f;
        }
        else if(HumanMotion.clip != null && horizontalSpeed > 0.4f)
        {
            if(!HumanMotion.isPlaying)
                HumanMotion.Play();
            if (HumanMotion.volume < 1f)
                HumanMotion.volume = 1f;
        }

        //Breathing
        float timeSpentRunning = m_humanController.GetTimeSpentRunning();
        if (m_Breathing.isPlaying)
        {
            m_Breathing.volume = ((timeSpentRunning - m_introTimeForBreathing) / m_maxRunTime)*2;
        }
        if (!m_Breathing.isPlaying)
        {
            m_Breathing.Play();
        }
    }


}
