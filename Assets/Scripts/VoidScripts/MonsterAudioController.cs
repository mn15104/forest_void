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
    private bool m_Hidden = false;
    public AudioSource MonsterSFXAudioSrc;
    public AudioSource MonsterMotionAudioSrc;
    public AudioClip m_GrassRun;
    public AudioClip m_StoneRun;
    public AudioClip m_GrassWalk;
    public AudioClip m_StoneWalk;
    public AudioClip m_GravelWalk;
    public AudioClip m_GravelRun;
    public AudioClip m_MudWalk;
    public AudioClip m_MudRun;
    public AudioClip m_Appear;
    public AudioClip m_Approach;
    public AudioClip m_Chase;
    public AudioClip[] m_AlarmingNoises;

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

        m_StateVolumeDictionary.Add(MonsterState.HIDDEN_IDLE, 0f);
        m_StateVolumeDictionary.Add(MonsterState.HIDDEN_MOVING, 1f);
        m_StateVolumeDictionary.Add(MonsterState.APPEAR, 0f);
        m_StateVolumeDictionary.Add(MonsterState.APPROACH, 1f);
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
            case TerrainType.GRAVEL:
                if (MonsterMotionAudioSrc.clip != m_GravelWalk || MonsterMotionAudioSrc.clip != m_GravelRun)
                {
                    if (MonsterMotionAudioSrc.clip != m_GrassWalk &&
                        (m_Monster.GetMonsterState() == MonsterState.HIDDEN_MOVING
                        || m_Monster.GetMonsterState() == MonsterState.APPROACH))
                    {
                        MonsterMotionAudioSrc.clip = m_GravelWalk;
                    }
                    else if (MonsterMotionAudioSrc.clip != m_GravelRun &&
                        m_Monster.GetMonsterState() == MonsterState.CHASE)
                    {
                        MonsterMotionAudioSrc.clip = m_GravelRun;
                    }
                }
                break;
            case TerrainType.MUD:
                if (MonsterMotionAudioSrc.clip != m_MudRun || MonsterMotionAudioSrc.clip != m_MudWalk)
                {
                    if (MonsterMotionAudioSrc.clip != m_MudWalk &&
                        (m_Monster.GetMonsterState() == MonsterState.HIDDEN_MOVING
                        || m_Monster.GetMonsterState() == MonsterState.APPROACH))
                    {
                        MonsterMotionAudioSrc.clip = m_MudWalk;
                    }
                    else if (MonsterMotionAudioSrc.clip != m_MudRun
                            && m_Monster.GetMonsterState() == MonsterState.CHASE)
                    {
                        MonsterMotionAudioSrc.clip = m_MudRun;
                    }
                }
                break;
            default:
                Debug.Log("WARNING: NO AUDIO SOURCE FOR CURRENT TERRAIN TYPE BELOW PLAYER.");
                break;
        }
        
    }
    IEnumerator alarmingNoises()
    {
        while (true)
        {
            yield return new WaitUntil(() => !MonsterSFXAudioSrc.isPlaying);
            yield return new WaitForSeconds(UnityEngine.Random.Range(4, 15));
            int nextSoundIndex = UnityEngine.Random.Range(0, m_AlarmingNoises.Length);
            MonsterSFXAudioSrc.clip = m_AlarmingNoises[nextSoundIndex];
            MonsterSFXAudioSrc.Play();
            
        }
    }
    void UpdateMonsterAudioState()
    {
        switch (m_MonsterState)
        {
            case MonsterState.HIDDEN_IDLE:
            case MonsterState.HIDDEN_MOVING:
                if (!m_Hidden)
                {
                    StartCoroutine(alarmingNoises());
                    m_Hidden = true;
                }
                break;
            case MonsterState.FOLLOW:
                m_Hidden = false;
                break;
            case MonsterState.APPEAR:
                m_Hidden = false;
                StopAllCoroutines();
                if (MonsterSFXAudioSrc.clip != m_Appear)
                {
                    MonsterSFXAudioSrc.clip = (m_Appear);
                    MonsterSFXAudioSrc.Play();
                }
                break;
            case MonsterState.APPROACH:
                m_Hidden = false;
                StopAllCoroutines();
                if (MonsterSFXAudioSrc.clip != m_Approach)
                {
                    MonsterSFXAudioSrc.clip = (m_Approach);
                    MonsterSFXAudioSrc.Play();
                }
                break;
            case MonsterState.CHASE:
                m_Hidden = false;
                StopAllCoroutines();
                if (MonsterSFXAudioSrc.clip != m_Chase)
                {
                    MonsterSFXAudioSrc.clip = (m_Chase);
                    MonsterSFXAudioSrc.Play();
                }
                break;
            default:
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
        ///////////////////////////////////////////////////////////////////////////////////////////
        float dist = (Mathf.Abs(m_Monster.transform.position.z - m_Monster.player.transform.position.z) +
            Mathf.Abs(m_Monster.transform.position.x - m_Monster.player.transform.position.x));
   
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

    bool isMember(AudioClip clip, AudioClip[] clip_list)
    {
        foreach(AudioClip t_clip in clip_list)
        {
            if (clip == t_clip)
                return true;
        }
        return false;
    }
}
