﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VoidSystem : MonoBehaviour
{

    public GameObject m_ForestVoid;
    public GameObject m_SpawnpointsHolder;
    public float m_MonsterGameTimer = 0f;
    private EventManager m_eventManager;
    private List<Vector3> m_SpawnPositions = new List<Vector3>();
    private float[] m_DelayTimeToActive = { 100f, 300f, 420f, 480f };
    private EventManager.Stage m_MonsterStage;
    public NotifyEvent<EventManager.Stage> NotifyStage = new NotifyEvent<EventManager.Stage>();
    private bool debugResettingStage = false;
    private bool monsterAppeared = false;

    private void OnEnable()
    {
        m_eventManager = FindObjectOfType<EventManager>();
        m_MonsterStage = m_ForestVoid.GetComponent<MonsterAI>().currentStage;
        NotifyStage.Notify(m_ForestVoid.GetComponent<MonsterAI>().currentStage);
    }
    
    void Start()
    {
        foreach (Transform trans in m_SpawnpointsHolder.GetComponentsInChildren<Transform>())
            m_SpawnPositions.Add(trans.position);
    }

    void Update()
    {
        if (!debugResettingStage)
        {
            if (m_eventManager.GetIsInForest())
            {
                m_MonsterGameTimer += Time.deltaTime;

                if (m_ForestVoid.GetComponent<MonsterAI>().GetMonsterState() == MonsterState.HUMAN_IN_STRUCT)
                    m_ForestVoid.GetComponent<MonsterAI>().SetState(MonsterState.HIDDEN_MOVING);

                switch (m_MonsterStage)
                {
                    case EventManager.Stage.Intro:
                        UpdateStageIntro();
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
            else
            {
                if(m_ForestVoid.GetComponent<MonsterAI>().GetMonsterState() != MonsterState.HUMAN_IN_STRUCT)
                    m_ForestVoid.GetComponent<MonsterAI>().SetState(MonsterState.HUMAN_IN_STRUCT);
            }
        }
    }
    
    void UpdateStageIntro()
    {
        if (m_ForestVoid.GetComponent<MonsterAI>().GetMonsterState() == MonsterState.DISABLED 
            && m_MonsterGameTimer > m_DelayTimeToActive[0])
        {
            m_ForestVoid.transform.position = GetFurthestSpawnPoint();
            m_ForestVoid.GetComponent<MonsterAI>().SetStage(EventManager.Stage.Stage1);
            m_ForestVoid.GetComponent<MonsterAI>().SetState(MonsterState.APPEAR);
            m_MonsterStage = EventManager.Stage.Stage1;
            NotifyStage.Notify(m_MonsterStage);
        }
    }

    void UpdateStage1()
    {
        if (m_ForestVoid.GetComponent<MonsterAI>().GetMonsterState() == MonsterState.DISABLED 
            && m_MonsterGameTimer > m_DelayTimeToActive[0])
        {
            m_ForestVoid.transform.position = GetFurthestSpawnPoint();
            m_ForestVoid.GetComponent<MonsterAI>().SetState(MonsterState.APPEAR);
        }

        if (m_MonsterGameTimer > m_DelayTimeToActive[1])
        {
            m_ForestVoid.GetComponent<MonsterAI>().SetState(MonsterState.STAGE_COMPLETE);
        }

        if (m_ForestVoid.GetComponent<MonsterAI>().GetMonsterState() == MonsterState.STAGE_COMPLETE)
        {
            m_ForestVoid.transform.position = GetFurthestSpawnPoint();
            m_ForestVoid.GetComponent<MonsterAI>().SetState(MonsterState.DISABLED);
            m_ForestVoid.GetComponent<MonsterAI>().SetStage(EventManager.Stage.Stage2);
            m_MonsterStage = EventManager.Stage.Stage2;
            NotifyStage.Notify(m_MonsterStage);
        }
    }

    void UpdateStage2()
    {
        if (m_ForestVoid.GetComponent<MonsterAI>().GetMonsterState() == MonsterState.DISABLED &&
            m_MonsterGameTimer > m_DelayTimeToActive[1])
        {
            m_ForestVoid.transform.position = GetFurthestSpawnPoint();
            m_ForestVoid.GetComponent<MonsterAI>().SetState(MonsterState.HIDDEN_MOVING);
        }

        if (m_MonsterGameTimer > m_DelayTimeToActive[2] && 
                                (m_ForestVoid.GetComponent<MonsterAI>().GetMonsterState() != MonsterState.APPEAR &&
                                 m_ForestVoid.GetComponent<MonsterAI>().GetMonsterState() != MonsterState.APPROACH &&
                                 m_ForestVoid.GetComponent<MonsterAI>().GetMonsterState() != MonsterState.CHASE &&
                                 m_ForestVoid.GetComponent<MonsterAI>().GetMonsterState() != MonsterState.ATTACK))
        {
            m_ForestVoid.GetComponent<MonsterAI>().SetState(MonsterState.APPEAR);
        }

        if (m_ForestVoid.GetComponent<MonsterAI>().GetMonsterState() == MonsterState.STAGE_COMPLETE)
        {
            m_ForestVoid.transform.position = GetFurthestSpawnPoint();
            m_ForestVoid.GetComponent<MonsterAI>().SetState(MonsterState.DISABLED);
            m_ForestVoid.GetComponent<MonsterAI>().SetStage(EventManager.Stage.Stage3);
            m_MonsterStage = EventManager.Stage.Stage3;
            NotifyStage.Notify(m_MonsterStage);
        }
    }

    void UpdateStage3()
    {
        if (m_ForestVoid.GetComponent<MonsterAI>().GetMonsterState() == MonsterState.DISABLED 
            && m_MonsterGameTimer > m_DelayTimeToActive[2])
        {
            m_ForestVoid.transform.position = GetFurthestSpawnPoint();
            m_ForestVoid.GetComponent<MonsterAI>().SetState(MonsterState.HIDDEN_MOVING);
        }

        if (m_MonsterGameTimer > m_DelayTimeToActive[3] && 
                                (   m_ForestVoid.GetComponent<MonsterAI>().GetMonsterState() != MonsterState.APPEAR &&
                                    m_ForestVoid.GetComponent<MonsterAI>().GetMonsterState() != MonsterState.APPROACH &&
                                    m_ForestVoid.GetComponent<MonsterAI>().GetMonsterState() != MonsterState.ATTACK &&
                                    m_ForestVoid.GetComponent<MonsterAI>().GetMonsterState() != MonsterState.CHASE ))
        {
            m_ForestVoid.GetComponent<MonsterAI>().SetState(MonsterState.APPEAR);
        }

        if (m_ForestVoid.GetComponent<MonsterAI>().GetMonsterState() == MonsterState.GAMEOVER)
        {
            m_ForestVoid.GetComponent<MonsterAI>().SetStage(EventManager.Stage.GameOverStage);
            m_MonsterStage = EventManager.Stage.GameOverStage;
            NotifyStage.Notify(m_MonsterStage);
        }
    }
    
    public void ResetVoidToStage(EventManager.Stage stage)
    {
        debugResettingStage = true;
        SetGameTimeFromStage(stage);
        m_ForestVoid.GetComponent<MonsterAI>().SetStage(stage);
        if (stage == EventManager.Stage.Stage1)
        {
            m_ForestVoid.GetComponent<MonsterAI>().SetState(MonsterState.APPEAR);
        }
        else if (stage == EventManager.Stage.Stage2 || stage == EventManager.Stage.Stage3)
        {
            m_ForestVoid.GetComponent<MonsterAI>().SetState(MonsterState.HIDDEN_IDLE);
        }
        else
        {
            m_ForestVoid.GetComponent<MonsterAI>().SetState(MonsterState.DISABLED);
        }
        m_MonsterStage = stage;
        NotifyStage.Notify(m_MonsterStage);
        debugResettingStage = false;
    }

    public void SetGameTimeFromStage(EventManager.Stage stage)
    {
        switch (stage)
        {
            case EventManager.Stage.Intro:
                m_MonsterGameTimer = 0f;
                break;
            case EventManager.Stage.Stage1:
                m_MonsterGameTimer = m_DelayTimeToActive[0];
                break;
            case EventManager.Stage.Stage2:
                m_MonsterGameTimer = m_DelayTimeToActive[1];
                break;
            case EventManager.Stage.Stage3:
                m_MonsterGameTimer = m_DelayTimeToActive[2];
                break;
            case EventManager.Stage.GameOverStage:
                m_MonsterGameTimer = 0f;
                break;
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
