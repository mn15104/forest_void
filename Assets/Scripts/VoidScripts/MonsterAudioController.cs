using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
//using UnityEditor;

public class MonsterAudioController : MonoBehaviour
{
    private Dictionary<int, TerrainType> m_TerrainTypeDictionary = new Dictionary<int, TerrainType>();
    private Dictionary<MonsterState, float> m_StateVolumeDictionary = new Dictionary<MonsterState, float>();
    private Terrain m_CurrentTerrain;
    private MonsterAI m_Monster;
    private MonsterState m_MonsterState;
    private MonsterState m_MonsterPrevState;
    private float m_MonsterSpeed;
    public AudioSource MonsterSFX1AudioSrc;
    public AudioSource MonsterSFX2AudioSrc;
    public AudioSource MonsterSFX3AudioSrc;
    public AudioSource MonsterSFX4AudioSrc;
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
    public AudioClip m_Jumpscare_Stage2;
    public AudioClip m_Jumpscare_Stage3;
    private Camera m_PlayerCamera;

    private bool m_MotionEnabled = true;

    private void OnEnable()
    {

        MonsterMotionAudioSrc.Stop();
        MonsterSFX1AudioSrc.Stop();
        MonsterSFX2AudioSrc.Stop();
        MonsterMotionAudioSrc.clip = m_GrassWalk;
        MonsterMotionAudioSrc.loop = true;
        MonsterSFX1AudioSrc.clip = null;
        MonsterSFX1AudioSrc.loop = true;
        MonsterSFX2AudioSrc.clip = null;
        MonsterSFX2AudioSrc.loop = false;
    }

    private void OnDisable()
    {
    }

    // Use this for initialization
    void Start()
    {
        m_Monster = GetComponentInParent<MonsterAI>();
        m_MonsterState = m_Monster.GetMonsterState();
        m_PlayerCamera = m_Monster.GetPlayer().GetComponentInChildren<Camera>();
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
                        || m_Monster.GetMonsterState() == MonsterState.APPROACH
                        || m_Monster.GetMonsterState() == MonsterState.FOLLOW))
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
                        || m_Monster.GetMonsterState() == MonsterState.APPROACH || m_Monster.GetMonsterState() == MonsterState.FOLLOW))
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
                        || m_Monster.GetMonsterState() == MonsterState.APPROACH || m_Monster.GetMonsterState() == MonsterState.FOLLOW))
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
                        || m_Monster.GetMonsterState() == MonsterState.APPROACH || m_Monster.GetMonsterState() == MonsterState.FOLLOW))
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

    bool monsterScream = false;
    void UpdateMonsterAudioState()
    {
        if (m_Monster.GetMonsterState() != m_MonsterState)
        {
            monsterScream = false;
            m_MonsterState = m_Monster.GetMonsterState();

            if(m_MonsterState != MonsterState.CHASE && m_MonsterState != MonsterState.ATTACK )    // Avoid chase state, as audio transitions from approach state 
            {
                MonsterSFX2AudioSrc.clip = null;
                MonsterSFX2AudioSrc.loop = false;
                MonsterSFX2AudioSrc.volume = 1f;
                MonsterSFX1AudioSrc.clip = null;
                MonsterSFX3AudioSrc.clip = null;
                MonsterSFX4AudioSrc.clip = null;
                MonsterSFX1AudioSrc.loop = false;
                MonsterSFX4AudioSrc.loop = false;
                MonsterSFX3AudioSrc.loop = false;
                MonsterSFX1AudioSrc.volume = 1f;
                MonsterSFX3AudioSrc.volume = 1f;
                MonsterSFX4AudioSrc.volume = 1f;
            }
        }

        switch (m_MonsterState)
        {
            case MonsterState.HIDDEN_IDLE:
                StopAllCoroutines();
                if (MonsterSFX1AudioSrc.clip != m_GasLoop)
                {
                    MonsterSFX1AudioSrc.clip = (m_GasLoop);
                    MonsterSFX1AudioSrc.Play();
                    MonsterSFX1AudioSrc.loop = true;

                }
                break;
            case MonsterState.HIDDEN_MOVING:
                StopAllCoroutines();
                if (MonsterSFX1AudioSrc.clip != m_GasLoop)
                {
                    MonsterSFX1AudioSrc.clip = (m_GasLoop);
                    MonsterSFX1AudioSrc.Play();
                    MonsterSFX1AudioSrc.loop = true;
                }
                break;
            case MonsterState.FOLLOW:
                StopAllCoroutines();
                if (MonsterSFX1AudioSrc.clip != m_GasLoop)
                {
                    MonsterSFX1AudioSrc.clip = m_GasLoop;
                    MonsterSFX1AudioSrc.Play();
                    MonsterSFX1AudioSrc.loop = true;
                }
                break;
            case MonsterState.APPEAR:
                {
                    float playerSpeed = m_Monster.GetPlayer().GetComponentInChildren<HumanVRAudioController>().GetSpeed();
                    if (m_Monster.GetMonsterStage() == EventManager.Stage.Stage1 || m_Monster.GetMonsterStage() == EventManager.Stage.Intro)
                    {
                        if (MonsterSFX1AudioSrc.clip != null)
                        {
                            MonsterSFX1AudioSrc.clip = null;
                            StopAllCoroutines();
                        }
                    }
                    else if (m_Monster.GetMonsterStage() == EventManager.Stage.Stage2)
                    {
                        if (m_Monster.stage2_playerTorchOff && !monsterScream)
                        {
                                MonsterSFX2AudioSrc.loop = false;
                                MonsterSFX2AudioSrc.volume = 1f;
                                MonsterSFX2AudioSrc.clip = (m_Jumpscare_Stage2);
                                MonsterSFX2AudioSrc.Play();
                                monsterScream = true;
                        }
                        else if ((MonsterSFX3AudioSrc.clip != m_MudWalk || MonsterSFX1AudioSrc.clip != m_GasLoop
                                  || !MonsterSFX1AudioSrc.isPlaying || !MonsterSFX3AudioSrc.isPlaying)
                                  && m_Monster.stage2_coroutine1_finished && !m_Monster.stage2_playerTorchOff2)
                        {
                            if (MonsterSFX1AudioSrc.clip != m_GasLoop || !MonsterSFX1AudioSrc.isPlaying)
                            {
                                MonsterSFX1AudioSrc.clip = m_GasLoop;
                                MonsterSFX1AudioSrc.volume = 1f;
                                MonsterSFX1AudioSrc.loop = true;
                                MonsterSFX1AudioSrc.Play();
                            }
                            if (MonsterSFX3AudioSrc.clip != m_MudWalk || !MonsterSFX3AudioSrc.isPlaying)
                            {
                                MonsterSFX3AudioSrc.clip = m_MudWalk;
                                MonsterSFX3AudioSrc.volume = 0.3f;
                                MonsterSFX3AudioSrc.loop = true;
                                MonsterSFX3AudioSrc.Play();
                                StartCoroutine(FadeInAudioSource(MonsterSFX3AudioSrc, 1f, 1f));
                            }
                        }
                    }
                    else if (m_Monster.GetMonsterStage() == EventManager.Stage.Stage3)
                    {
                        if (!m_Monster.Stage3_Appeared) {
                            if (MonsterSFX1AudioSrc.clip != m_GasLoop || !MonsterSFX1AudioSrc.isPlaying)
                            {
                                MonsterSFX1AudioSrc.clip = m_GasLoop;
                                MonsterSFX1AudioSrc.loop = true;
                                MonsterSFX1AudioSrc.Play();
                                MonsterSFX1AudioSrc.volume = 0.2f;
                                StartCoroutine(FadeInAudioSource(MonsterSFX1AudioSrc, 0.9f, 0.2f));
                            }
                            if ((MonsterSFX2AudioSrc.clip != m_BreathingBehind || !MonsterSFX2AudioSrc.isPlaying))
                            {
                                MonsterSFX2AudioSrc.clip = m_BreathingBehind;
                                MonsterSFX2AudioSrc.loop = true;
                                MonsterSFX2AudioSrc.Play();
                                MonsterSFX2AudioSrc.volume = 0.0f;
                                StartCoroutine(FadeInAudioSource(MonsterSFX2AudioSrc, 0.425f, 0.1f));//josh sucks
                            }
                            if ((MonsterSFX3AudioSrc.clip != m_GravelWalk || !MonsterSFX3AudioSrc.isPlaying))
                            {
                                MonsterSFX3AudioSrc.clip = m_GravelWalk;
                                MonsterSFX3AudioSrc.loop = true;
                                MonsterSFX3AudioSrc.volume = 0f;
                                MonsterSFX3AudioSrc.Play();
                            }
                            if (playerSpeed > 0.6f &&
                                playerSpeed < 3f)            // Must be walking
                            {
                                StartCoroutine(DelayClipPlay(MonsterSFX3AudioSrc, true, 1.2f));
                            }
                            else if (playerSpeed > 0.6f)    // Must be running
                            {
                                MonsterSFX3AudioSrc.volume = 0;
                            }
                            else if (playerSpeed < 0.6f)    // Must be idle
                            {
                                StartCoroutine(DelayClipPlay(MonsterSFX3AudioSrc, false, 1.2f));
                            }
                        }
                        if (!monsterScream && m_Monster.Stage3_Appeared)
                        {
                            StopAllCoroutines();
                            MonsterSFX3AudioSrc.clip = null;
                            MonsterSFX2AudioSrc.clip = m_Jumpscare_Stage3;
                            MonsterSFX2AudioSrc.volume = 1f;
                            MonsterSFX2AudioSrc.loop = false;
                            MonsterSFX2AudioSrc.Play();
                            monsterScream = true;
                        }
                    }
                }
                break;
            case MonsterState.APPROACH:
                {
                    if (MonsterSFX1AudioSrc.clip != m_BreathingFront || !MonsterSFX1AudioSrc.isPlaying)
                    {
                        MonsterSFX1AudioSrc.clip = m_BreathingFront;
                        MonsterSFX1AudioSrc.loop = true;
                        MonsterSFX1AudioSrc.volume = 1f;
                        MonsterSFX1AudioSrc.Play();
                    }
                    if (!monsterScream)
                    {
                        MonsterSFX2AudioSrc.loop = true;
                        MonsterSFX2AudioSrc.volume = 0;
                        MonsterSFX2AudioSrc.clip = m_Chase;
                        MonsterSFX2AudioSrc.Play();
                        StartCoroutine(FadeInAudioSource(MonsterSFX2AudioSrc, 0.85f, 0.25f));
                        monsterScream = true;
                    }
                    if (MonsterSFX3AudioSrc.clip != m_GasLoop || !MonsterSFX3AudioSrc.isPlaying)
                    {
                        MonsterSFX3AudioSrc.loop = true;
                        MonsterSFX3AudioSrc.volume = 1f;
                        MonsterSFX3AudioSrc.clip = (m_GasLoop);
                        MonsterSFX3AudioSrc.Play();
                    }
                }
                break;
            case MonsterState.CHASE:
                if (MonsterSFX1AudioSrc.clip != m_BreathingFront || !MonsterSFX1AudioSrc.isPlaying)
                {
                    MonsterSFX1AudioSrc.clip = m_BreathingFront;
                    MonsterSFX1AudioSrc.loop = true;
                    MonsterSFX1AudioSrc.volume = 1f;
                    MonsterSFX1AudioSrc.Play();
                }
                if (MonsterSFX3AudioSrc.clip != m_GasLoop || !MonsterSFX3AudioSrc.isPlaying)
                {
                    MonsterSFX3AudioSrc.loop = true;
                    MonsterSFX3AudioSrc.volume = 1f;
                    MonsterSFX3AudioSrc.clip = (m_GasLoop);
                    MonsterSFX3AudioSrc.Play();
                }
                break;
            case MonsterState.ATTACK:
                if (MonsterSFX4AudioSrc.clip != m_Jumpscare_Stage2)
                {
                    MonsterSFX4AudioSrc.loop = false;
                    MonsterSFX4AudioSrc.volume = 0.9f;
                    MonsterSFX4AudioSrc.clip = (m_Jumpscare_Stage2);
                    MonsterSFX4AudioSrc.Play();
                }
                break;
            case MonsterState.DISABLED:
            case MonsterState.STAGE_COMPLETE:
                StopAllCoroutines();
                monsterScream = false;
                m_MotionEnabled = true;
                MonsterSFX1AudioSrc.Stop();
                MonsterSFX2AudioSrc.Stop();
                MonsterSFX3AudioSrc.Stop();
                MonsterSFX4AudioSrc.Stop();
                MonsterSFX1AudioSrc.clip = null;
                MonsterSFX2AudioSrc.clip = null;
                MonsterSFX3AudioSrc.clip = null;
                MonsterSFX4AudioSrc.clip = null;
                MonsterSFX1AudioSrc.loop = false;
                MonsterSFX2AudioSrc.loop = false;
                MonsterSFX3AudioSrc.loop = false;
                MonsterSFX4AudioSrc.loop = false;
                break;
            case MonsterState.GAMEOVER:
                if (MonsterSFX2AudioSrc.volume > 0)
                {
                    MonsterSFX2AudioSrc.volume -= Time.deltaTime * 0.3f;
                }
                break;
            default:
                break;
        }
    }
    IEnumerator DelayClipPlay(AudioSource audsrc, bool playing, float delaytime)
    {
        yield return new WaitForSeconds(delaytime);
        if (playing)
            audsrc.volume = 1f;
        else
            audsrc.volume = 0f;
    }
    public void Reset()
    {
        MonsterMotionAudioSrc.Stop();
        MonsterSFX1AudioSrc.Stop();
        MonsterSFX2AudioSrc.Stop();
        MonsterSFX4AudioSrc.Stop();
        MonsterSFX1AudioSrc.volume = 1f;
        MonsterSFX2AudioSrc.volume = 1f;
        MonsterSFX3AudioSrc.volume = 1f;
        MonsterSFX4AudioSrc.volume = 1f;
        MonsterMotionAudioSrc.clip = m_GrassWalk;
        MonsterMotionAudioSrc.loop = true;
        MonsterSFX1AudioSrc.clip = null;
        MonsterSFX1AudioSrc.loop = true;
        MonsterSFX2AudioSrc.clip = null;
        MonsterSFX2AudioSrc.loop = false;
        MonsterSFX4AudioSrc.clip = null;
        MonsterSFX4AudioSrc.loop = false;
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 vel = m_Monster.GetComponent<Rigidbody>().velocity;
        m_MonsterSpeed = new Vector2(vel.x, vel.z).magnitude;
        ////////////////////////////////////////////////////////////////////////////////////////
        UpdateMonsterMotion();
        UpdateMonsterAudioState();

        if (MonsterMotionAudioSrc.clip != null)
        {
            if (!MonsterMotionAudioSrc.isPlaying)
                MonsterMotionAudioSrc.Play();
            if (m_MonsterSpeed < 0.1f)
                MonsterMotionAudioSrc.volume = 0;
            else
            {
                MonsterMotionAudioSrc.volume = 1f;
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        float dist = Mathf.Sqrt(Mathf.Pow(m_Monster.GetPlayer().transform.position.x - transform.position.x, 2)
                                + Mathf.Pow(m_Monster.GetPlayer().transform.position.z - transform.position.z, 2));

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
        foreach (AudioClip t_clip in clip_list)
        {
            if (clip == t_clip)
                return true;
        }
        return false;
    }

    void ResetAudioStates()
    {
        monsterScream = false;
        m_MotionEnabled = true;
        MonsterSFX2AudioSrc.clip = null;
        MonsterSFX2AudioSrc.loop = false;
        MonsterSFX3AudioSrc.clip = null;
        MonsterSFX3AudioSrc.loop = false;
    }
    IEnumerator FadeInAudioSource(AudioSource aud, float max_vol, float fade_in_rate)
    {
        while (aud.volume < max_vol)
        {
            yield return null;
            aud.volume = Mathf.Lerp(aud.volume, max_vol, Time.deltaTime * fade_in_rate);
        }
    }
    IEnumerator FadeOutAudioSource(AudioSource aud, float fade_out_rate)
    {
        while (aud.volume > 0)
        {
            yield return null;
            aud.volume = Mathf.Lerp(aud.volume, 0, Time.deltaTime * fade_out_rate);

        }
    }
}
