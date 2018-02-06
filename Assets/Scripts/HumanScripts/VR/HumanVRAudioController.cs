using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public enum TerrainType
{
    GRASS,
    STONE
}

public class HumanVRAudioController : MonoBehaviour {


    public delegate void HumanAudioEmitter(float volumeEmitted);
    public static event HumanAudioEmitter OnHumanAudioEmission;

    
    private Dictionary<int, TerrainType> m_TerrainTypeDictionary = new Dictionary<int, TerrainType>();
    private Dictionary<TerrainType, float> m_TerrainVolumeDictionary = new Dictionary<TerrainType, float>();
    private Terrain m_CurrentTerrain;
    public AudioSource HumanMotion;
    public AudioClip m_GrassRun;
    public AudioClip m_StoneRun;
    public AudioClip m_GrassWalk;
    public AudioClip m_StoneWalk;
    public AudioClip m_WoodRun;
    public AudioSource m_Breathing;
    private Transform m_Transform;
    private Rigidbody m_ParentRigidBody;
    private HumanVRController m_humanVRController;
    private bool m_airborne = false;
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
        m_TerrainTypeDictionary.Add(8, TerrainType.STONE);

        HumanMotion.clip = m_StoneRun;
        HumanMotion.loop = true;
    }

    // Use this for initialization
    void Start () {
        m_humanVRController = GetComponentInParent<HumanVRController>();
        m_Transform = transform;
        m_ParentRigidBody = GetComponentInParent<Rigidbody>();
        m_Breathing.enabled = false;
        m_Breathing.loop = true;
        m_Breathing.volume = 0;
        m_CurrentTerrain = m_humanVRController.GetCurrentTerrain();
        m_maxRunTime = m_humanVRController.GetMaxRunTime();
        m_introTimeForBreathing = m_maxRunTime / 2;
    }
    
    double Normalize3Dto2D(Vector3 vector3)
    {
        return Math.Sqrt(Math.Pow(vector3.x, 2) + Math.Pow(vector3.z, 2));
    }

    private void FixedUpdate()
    {
        if (HumanMotion.isPlaying)
        {
            if(m_humanVRController.GetPlayerMoveState() == PlayerMoveState.WALKING || 
               m_humanVRController.GetPlayerMoveState() == PlayerMoveState.CROUCHING)
            {
                OnHumanAudioEmission(0.3f);
            }
            else if (m_humanVRController.GetPlayerMoveState() == PlayerMoveState.RUNNING)
            {
                OnHumanAudioEmission(1f);
            }
        }
        else
        {
            OnHumanAudioEmission(0);
        }
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
        m_CurrentTerrain = m_humanVRController.GetCurrentTerrain();
        if (m_CurrentTerrain == null)
        {
            return;
        }
        CurrentGroundCollision CurrentGround = m_humanVRController.GetCurrentGroundCollision();
        Debug.Log(CurrentGround);
        if (CurrentGround == CurrentGroundCollision.WOOD)
        {
            if (HumanMotion.clip != m_WoodRun)
            {
                HumanMotion.clip = m_WoodRun;
            }
        }
        else if (CurrentGround == CurrentGroundCollision.AIR)
        {
            if (HumanMotion.clip != null)
            {
                HumanMotion.clip = null;
            }
        }
        else if (CurrentGround == CurrentGroundCollision.TERRAIN)
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
                        if (HumanMotion.clip != m_StoneWalk && (m_humanVRController.GetPlayerMoveState() == PlayerMoveState.WALKING || m_humanVRController.GetPlayerMoveState() == PlayerMoveState.CROUCHING))
                        {
                            HumanMotion.clip = m_StoneWalk;
                        }
                        else if(HumanMotion.clip != m_StoneRun && m_humanVRController.GetPlayerMoveState() == PlayerMoveState.RUNNING)
                        {
                            HumanMotion.clip = m_StoneRun;
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
        float timeSpentRunning = m_humanVRController.GetTimeSpentRunning();
        if (timeSpentRunning > m_introTimeForBreathing 
           && !m_Breathing.isPlaying)
        {
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


}
