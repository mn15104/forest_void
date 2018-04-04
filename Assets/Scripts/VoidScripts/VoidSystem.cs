using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VoidSystem : MonoBehaviour
{

    public GameObject m_ForestVoid;
    private GameObject m_CryptVoid;
    public GameObject m_SpawnpointsHolder;
    private EventManager m_eventManager;
    private bool monsterAppeared = false;
    private List<Vector3> m_SpawnPositions = new List<Vector3>();
    private float gameTimer = 0f;
    private float[] m_DelayTimeToActive = { 120f, 210f, 330f };
    private EventManager.Stage m_MonsterStage;
    private MonsterState m_MonsterState = MonsterState.HIDDEN_IDLE;

    public NotifyEvent<EventManager.Stage> NotifyStage;
    //private Stage 

    private void OnEnable()
    {
        NotifyStage = new NotifyEvent<EventManager.Stage>();
        NotifyStage.Notify(m_ForestVoid.GetComponent<MonsterAI>().currentStage);
    }
    
    void Start()
    {
        if (m_ForestVoid)
            m_ForestVoid.SetActive(false);
        else
            m_MonsterStage = m_ForestVoid.GetComponent<MonsterAI>().currentStage;
        foreach (Transform trans in m_SpawnpointsHolder.GetComponentsInChildren<Transform>())
            m_SpawnPositions.Add(trans.position);
    }

    void Update()
    {
        gameTimer = m_eventManager.GetGameTime();
        switch (m_MonsterStage)
        {
            case EventManager.Stage.Intro:

                break;
            case EventManager.Stage.Stage1:
                UpdateStage1();
                break;
            case EventManager.Stage.Stage2:
                UpdateStage2();
                break;
            case EventManager.Stage.Stage3:
                UpdateStage3();
                break;
            default:
                break;
        }
    }
    
    void UpdateStageIntro()
    {
        if (!m_ForestVoid.activeSelf && gameTimer > 120f)
        {
            m_ForestVoid.SetActive(true);
            m_ForestVoid.transform.position = GetFurthestSpawnPoint();
            m_MonsterStage = EventManager.Stage.Stage1;
            NotifyStage.Notify(m_MonsterStage);
        }
    }

    void UpdateStage1()
    {
        if (!m_ForestVoid.activeSelf && gameTimer > 120f)
        {
            m_ForestVoid.SetActive(true);
            m_ForestVoid.transform.position = GetFurthestSpawnPoint();
        }
        if (gameTimer > 210f && 
            (   m_ForestVoid.GetComponent<MonsterAI>().GetMonsterState() != MonsterState.APPEAR &&
                m_ForestVoid.GetComponent<MonsterAI>().GetMonsterState() != MonsterState.APPROACH &&
                m_ForestVoid.GetComponent<MonsterAI>().GetMonsterState() != MonsterState.CHASE)
            )
        {
            m_ForestVoid.GetComponent<MonsterAI>().SetState(MonsterState.APPEAR);
        }
        if (m_ForestVoid.activeSelf && m_ForestVoid.GetComponent<MonsterAI>().GetMonsterState() == MonsterState.DISABLED)
        {
            SetVoidInactive();
            m_MonsterStage = EventManager.Stage.Stage2;
            NotifyStage.Notify(m_MonsterStage);
        }
    }
    void UpdateStage2()
    {
        if (!m_ForestVoid.activeSelf && gameTimer > 210f)
        {
            m_ForestVoid.SetActive(true);
            m_ForestVoid.transform.position = GetFurthestSpawnPoint();
        }
        if (gameTimer > 330f && (m_ForestVoid.GetComponent<MonsterAI>().GetMonsterState() != MonsterState.APPEAR &&
                m_ForestVoid.GetComponent<MonsterAI>().GetMonsterState() != MonsterState.APPROACH &&
                m_ForestVoid.GetComponent<MonsterAI>().GetMonsterState() != MonsterState.CHASE)
            )
        {
            m_ForestVoid.GetComponent<MonsterAI>().SetState(MonsterState.APPEAR);
        }
        if (m_ForestVoid.activeSelf && m_ForestVoid.GetComponent<MonsterAI>().GetMonsterState() == MonsterState.DISABLED)
        {
            SetVoidInactive();
            m_MonsterStage = EventManager.Stage.Stage3;
            NotifyStage.Notify(m_MonsterStage);
        }
    }
    void UpdateStage3()
    {
        if (!m_ForestVoid.activeSelf && gameTimer > 330f)
        {
            m_ForestVoid.SetActive(true);
            m_ForestVoid.transform.position = GetFurthestSpawnPoint();
        }
        if (gameTimer > 470f && 
            (m_ForestVoid.GetComponent<MonsterAI>().GetMonsterState() != MonsterState.APPEAR &&
                m_ForestVoid.GetComponent<MonsterAI>().GetMonsterState() != MonsterState.APPROACH &&
                m_ForestVoid.GetComponent<MonsterAI>().GetMonsterState() != MonsterState.CHASE )
            )
        {
            m_ForestVoid.GetComponent<MonsterAI>().SetState(MonsterState.APPEAR);
        }
        if (m_ForestVoid.activeSelf &&  m_ForestVoid.GetComponent<MonsterAI>().GetMonsterState() == MonsterState.DISABLED)
        {
            SetVoidInactive();
            m_MonsterStage = EventManager.Stage.GameOverStage;  /////////////////// Fix conditionals to gameover
            NotifyStage.Notify(m_MonsterStage);
        }
    }


    public void SetVoidActive(Vector3 position = new Vector3())
    {
        if (m_ForestVoid)
        {
            m_ForestVoid.transform.position = GetFurthestSpawnPoint();
            m_ForestVoid.SetActive(true);
        }
    }

    public void SetVoidInactive()
    {
        if (m_ForestVoid)
        {
            m_ForestVoid.SetActive(false);
        }
    }


    Vector3 GetFurthestSpawnPoint()
    {
        Vector3 playerPos = m_ForestVoid.GetComponent<MonsterAI>().player.transform.position;
        Vector3 furthestSpawnPoint = m_SpawnPositions[0];
        foreach (Vector3 pos in m_SpawnPositions)
        {
            if ((pos - playerPos).magnitude > (furthestSpawnPoint - playerPos).magnitude)
            {
                furthestSpawnPoint = pos;
            }
        }
        return furthestSpawnPoint;
    }
}
