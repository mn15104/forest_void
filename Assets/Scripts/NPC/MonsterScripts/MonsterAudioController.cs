using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MonsterAudioController : MonoBehaviour
{
    private Dictionary<int, TerrainType> m_TerrainTypeDictionary = new Dictionary<int, TerrainType>();
    private Dictionary<MonsterState, float> m_StateVolumeDictionary = new Dictionary<MonsterState, float>();
    private Terrain m_CurrentTerrain;
    private MonsterAI m_Monster;
    private MonsterState m_MonsterState;
    public AudioSource MonsterSFXAudioSrc;
    public AudioSource MonsterMotionAudioSrc;
    public AudioClip m_GrassRun;
    public AudioClip m_StoneRun;
    public AudioClip m_GrassWalk;
    public AudioClip m_StoneWalk;
    public AudioClip m_Demigorgon;
    public AudioClip m_Appear;
    public AudioClip m_Approach;
    public AudioClip m_Chase;


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
        m_TerrainTypeDictionary.Add(9, TerrainType.STONE);
        m_TerrainTypeDictionary.Add(10, TerrainType.GRASS);
        m_StateVolumeDictionary.Add(MonsterState.HIDDEN_IDLE, 0f);
        m_StateVolumeDictionary.Add(MonsterState.HIDDEN_MOVING, 0.5f);
        m_StateVolumeDictionary.Add(MonsterState.APPEAR, 0f);
        m_StateVolumeDictionary.Add(MonsterState.APPROACH, 0.8f);
        m_StateVolumeDictionary.Add(MonsterState.CHASE, 1f);
        m_StateVolumeDictionary.Add(MonsterState.GAMEOVER, 0f);
        MonsterMotionAudioSrc.clip = m_StoneRun;
        MonsterMotionAudioSrc.loop = true;
        MonsterSFXAudioSrc.loop = false;
        MonsterSFXAudioSrc.clip = null;
        MonsterSFXAudioSrc.Stop();
    }

    // Use this for initialization
    void Start()
    {
        m_Monster = GetComponentInParent<MonsterAI>();
        m_MonsterState = m_Monster.GetMonsterState();
    }
    private void UpdateMonsterMotion()
    {
        Terrain m_CurrentTerrain = Terrain.activeTerrain;
        int textureIndex = GetMainTexture(transform.position);
        TerrainType currentTerrainType = m_TerrainTypeDictionary[textureIndex];
        switch (currentTerrainType)
        {
            case TerrainType.GRASS:
                if (MonsterMotionAudioSrc.clip != m_GrassWalk || MonsterMotionAudioSrc.clip != m_GrassRun)
                {
                    if (MonsterMotionAudioSrc.clip != m_GrassWalk && 
                        (m_Monster.GetMonsterState() == MonsterState.HIDDEN_MOVING
                        || m_Monster.GetMonsterState() == MonsterState.APPROACH))
                    {
                        MonsterMotionAudioSrc.clip = m_GrassWalk;
                    }
                    else if (MonsterMotionAudioSrc.clip != m_GrassRun && 
                        m_Monster.GetMonsterState() == MonsterState.CHASE)
                    {
                        MonsterMotionAudioSrc.clip = m_GrassRun;
                    }
                }
                break;
            case TerrainType.STONE:
                if (MonsterMotionAudioSrc.clip != m_StoneRun || MonsterMotionAudioSrc.clip != m_StoneWalk)
                {
                    if (MonsterMotionAudioSrc.clip != m_GrassWalk && 
                        (m_Monster.GetMonsterState() == MonsterState.HIDDEN_MOVING
                        || m_Monster.GetMonsterState() == MonsterState.APPROACH))
                    {
                        MonsterMotionAudioSrc.clip = m_StoneWalk;
                    }
                    else if (MonsterMotionAudioSrc.clip != m_GrassRun 
                            && m_Monster.GetMonsterState() == MonsterState.CHASE)
                    {
                        MonsterMotionAudioSrc.clip = m_StoneRun;
                    }
                }
                break;
            default:
                Debug.Log("WARNING: NO AUDIO SOURCE FOR CURRENT TERRAIN TYPE BELOW PLAYER.");
                break;
        }
        
    }

    void UpdateMonsterAudioState()
    {
        switch (m_MonsterState)
        {
            case MonsterState.APPEAR:
                if (MonsterSFXAudioSrc.clip != m_Appear)
                {
                    MonsterSFXAudioSrc.clip = (m_Appear);
                    MonsterSFXAudioSrc.Play();
                }
                break;
            case MonsterState.APPROACH:
                if (MonsterSFXAudioSrc.clip != m_Approach)
                {
                    MonsterSFXAudioSrc.clip = (m_Approach);
                    MonsterSFXAudioSrc.Play();
                }
                break;
            case MonsterState.CHASE:
                if (MonsterSFXAudioSrc.clip != m_Chase)
                {
                    MonsterSFXAudioSrc.clip = (m_Chase);
                    MonsterSFXAudioSrc.Play();
                }
                break;
            case MonsterState.HIDDEN_IDLE:
                break;
            case MonsterState.HIDDEN_MOVING:
                break;
        }
    }


    // Update is called once per frame
    void Update()
    {
        m_MonsterState = m_Monster.GetMonsterState();
        ////////////////////////////////////////////////////////////////////////////////////////
        UpdateMonsterMotion();
        UpdateMonsterAudioState();
        float monsterSpeed = m_Monster.GetMonsterSpeed(); 
        if (MonsterMotionAudioSrc.clip != null)
        {
            if (!MonsterMotionAudioSrc.isPlaying)
                MonsterMotionAudioSrc.Play();
            if(m_StateVolumeDictionary[m_MonsterState] != MonsterMotionAudioSrc.volume)
            {
                MonsterMotionAudioSrc.volume = m_StateVolumeDictionary[m_MonsterState];
            }
        }
        float dist = (Mathf.Abs(m_Monster.transform.position.z - m_Monster.player.transform.position.z) +
            Mathf.Abs(m_Monster.transform.position.x - m_Monster.player.transform.position.x));
        if ( dist  > 0)
        {
            //Debug.Log(dist);
        }
    }

    float[] GetTextureMix(Vector3 worldPos)
    {

        TerrainData terrainData = Terrain.activeTerrain.terrainData;
        Vector3 terrainPos = Terrain.activeTerrain.transform.position;

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
}
