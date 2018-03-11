using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

public class MonsterAudioController : MonoBehaviour
{
    private Dictionary<int, TerrainType> m_TerrainTypeDictionary = new Dictionary<int, TerrainType>();
    private Dictionary<MonsterState, float> m_StateVolumeDictionary = new Dictionary<MonsterState, float>();
    private Terrain m_CurrentTerrain;
    private MonsterAI m_Monster;
    private MonsterState m_MonsterState;
    private float m_MonsterSpeed;
    public AudioSource MonsterSFX1AudioSrc;
    public AudioSource MonsterSFX2AudioSrc;
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
    public AudioClip m_BreathingFront;
    public AudioClip m_BreathingBehind;
    public AudioClip m_GasLoop;
    public AudioClip m_Jumpscare;
    private Camera m_PlayerCamera;
    private void OnEnable()
    {
        
        MonsterMotionAudioSrc.Stop();
        MonsterSFX1AudioSrc.Stop();
        MonsterSFX2AudioSrc.Stop();
        MonsterMotionAudioSrc.clip = m_StoneRun;
        MonsterMotionAudioSrc.loop = true;
        MonsterSFX1AudioSrc.clip = null;
        MonsterSFX1AudioSrc.loop = true;
        MonsterSFX2AudioSrc.clip = null;
        MonsterSFX2AudioSrc.loop = false;
        m_Monster = GetComponentInParent<MonsterAI>();
        m_PlayerCamera = m_Monster.player.GetComponentInChildren<Camera>();
        m_MonsterState = m_Monster.GetMonsterState();
    }

    private void OnDisable()
    {
    }

    // Use this for initialization
    void Start()
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
        m_StateVolumeDictionary.Add(MonsterState.HIDDEN_MOVING, 0.4f);
        m_StateVolumeDictionary.Add(MonsterState.FOLLOW, 0.4f);
        m_StateVolumeDictionary.Add(MonsterState.APPEAR, 0f);
        m_StateVolumeDictionary.Add(MonsterState.APPROACH, 0.6f);
        m_StateVolumeDictionary.Add(MonsterState.CHASE, 0.8f);
        m_StateVolumeDictionary.Add(MonsterState.GAMEOVER, 0f);
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
    void UpdateMonsterAudioState()
    {
        switch (m_MonsterState)
        {
            case MonsterState.HIDDEN_IDLE:
                if (MonsterSFX1AudioSrc.clip != m_GasLoop)
                {
                    MonsterSFX1AudioSrc.clip = (m_GasLoop);
                    MonsterSFX1AudioSrc.Play();
                }
                break;
            case MonsterState.HIDDEN_MOVING:
                if (MonsterSFX1AudioSrc.clip != m_GasLoop)
                {
                    MonsterSFX1AudioSrc.clip = (m_GasLoop);
                    MonsterSFX1AudioSrc.Play();
                }
                break;
            case MonsterState.FOLLOW:
                MonsterSFX2AudioSrc.clip = null;
                if (MonsterSFX1AudioSrc.clip != m_BreathingBehind)
                {
                    MonsterSFX1AudioSrc.clip = (m_BreathingBehind);
                    MonsterSFX1AudioSrc.Play();
                }
                break;
            case MonsterState.APPEAR:
                {
                    if (m_PlayerCamera)
                    {
                        Plane[] cameraPlanes = GeometryUtility.CalculateFrustumPlanes(m_PlayerCamera);
                        if (MonsterSFX2AudioSrc.clip != m_Jumpscare && GeometryUtility.TestPlanesAABB(cameraPlanes, m_Monster.GetComponent<Collider>().bounds))
                        {
                            MonsterSFX2AudioSrc.clip = m_Jumpscare;
                            MonsterSFX2AudioSrc.Play();
                        }
                        if (MonsterSFX1AudioSrc.clip != m_BreathingFront && GeometryUtility.TestPlanesAABB(cameraPlanes, m_Monster.GetComponent<Collider>().bounds))
                        {
                            MonsterSFX1AudioSrc.clip = (m_BreathingFront);
                            MonsterSFX1AudioSrc.Play();
                        }
                        else if (MonsterSFX1AudioSrc.clip != m_BreathingBehind)
                        {
                            MonsterSFX1AudioSrc.clip = (m_BreathingBehind);
                            MonsterSFX1AudioSrc.Play();
                        }
                    }
                    else
                    {
                        if (MonsterSFX1AudioSrc.clip != m_BreathingFront)
                        {
                            MonsterSFX1AudioSrc.clip = (m_BreathingFront);
                            MonsterSFX1AudioSrc.Play();
                        }
                    }
                }
                break;
                
            case MonsterState.APPROACH:
                {
                    StopAllCoroutines();
                    MonsterSFX2AudioSrc.clip = null;
                    Plane[] cameraPlanes = GeometryUtility.CalculateFrustumPlanes(m_Monster.player.GetComponentInChildren<Camera>());
                    if (MonsterSFX1AudioSrc.clip != m_BreathingFront && GeometryUtility.TestPlanesAABB(cameraPlanes, m_Monster.GetComponent<Collider>().bounds))
                    {
                        MonsterSFX1AudioSrc.clip = (m_BreathingFront);
                        MonsterSFX1AudioSrc.Play();
                    }
                    else if (MonsterSFX1AudioSrc.clip != m_BreathingBehind)
                    {
                        MonsterSFX1AudioSrc.clip = (m_BreathingBehind);
                        MonsterSFX1AudioSrc.Play();
                    }
                }
                break;
                
            case MonsterState.CHASE:
                StopAllCoroutines();
                MonsterSFX2AudioSrc.clip = null;
                if (MonsterSFX1AudioSrc.clip != m_Chase)
                {
                    MonsterSFX1AudioSrc.loop = false;
                    MonsterSFX1AudioSrc.clip = (m_Chase);
                    MonsterSFX1AudioSrc.Play();
                }
                break;
            default:
                break;
        }
    }


    // Update is called once per frame
    void Update()
    {
        Vector3 vel = m_Monster.GetComponent<Rigidbody>().velocity ;
        m_MonsterSpeed = new Vector2(vel.x, vel.z).magnitude;
        
        m_MonsterState = m_Monster.GetMonsterState();
        ////////////////////////////////////////////////////////////////////////////////////////
        UpdateMonsterMotion();
        UpdateMonsterAudioState();
        float monsterSpeed = m_Monster.GetMonsterSpeed(); 
        if (MonsterMotionAudioSrc.clip != null)
        {
            if (!MonsterMotionAudioSrc.isPlaying)
                MonsterMotionAudioSrc.Play();
            if (m_MonsterSpeed < 0.1f)
                MonsterMotionAudioSrc.volume = 0;
            else if (m_StateVolumeDictionary[m_MonsterState] != MonsterMotionAudioSrc.volume)
            {
                MonsterMotionAudioSrc.volume = m_StateVolumeDictionary[m_MonsterState];
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        float dist = Mathf.Sqrt(Mathf.Pow(m_Monster.player.transform.position.x - transform.position.x, 2)
                                + Mathf.Pow(m_Monster.player.transform.position.z - transform.position.z, 2));

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
