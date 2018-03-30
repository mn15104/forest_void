using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MonsterAI
{

    public class VoidSystem : MonoBehaviour
    {
        public GameObject m_ForestVoid;
        private GameObject m_CryptVoid;
        public GameObject m_SpawnpointsHolder;
        private bool monsterAppeared = false;
        private List<Vector3> m_SpawnPositions = new List<Vector3>();
        private float gameTimer = 0f;
        private float[] m_DelayTimeToActive = { 120f, 210f, 330f };
        private MonsterAppear m_MonsterAppear = MonsterAppear.STAGE1;
        private MonsterState m_MonsterState = MonsterState.HIDDEN_IDLE;


        private void OnEnable()
        {

        }

        void Start()
        {
            if (m_ForestVoid)
                m_ForestVoid.SetActive(false);

            foreach (Transform trans in m_SpawnpointsHolder.GetComponentsInChildren<Transform>())
                m_SpawnPositions.Add(trans.position);

        }

        void Update()
        {
            gameTimer += Time.deltaTime;
            switch (m_MonsterAppear)
            {
                case MonsterAppear.STAGE1:
                    UpdateStage1();
                    break;
                case MonsterAppear.STAGE2:
                    UpdateStage2();
                    break;
                case MonsterAppear.STAGE3:
                    UpdateStage3();
                    break;
                default:
                    break;
            }
        }

        void UpdateStage1()
        {
            if (!m_ForestVoid.activeSelf && gameTimer > 120f)
            {
                m_ForestVoid.SetActive(true);
            }
            if (!monsterAppeared && m_ForestVoid.GetComponent<MonsterAI>().GetMonsterState() == MonsterState.APPEAR)
            {
                monsterAppeared = true;
            }
            if (!monsterAppeared && gameTimer > 210f)
            {
                m_ForestVoid.GetComponent<MonsterAI>().m_MonsterStateMachine.SetState(MonsterState.APPEAR);
                monsterAppeared = true;
                m_MonsterAppear = MonsterAppear.STAGE2;
            }
            if (monsterAppeared && m_ForestVoid.GetComponent<MonsterAI>().GetMonsterState() == MonsterState.HIDDEN_IDLE)
            {
                monsterAppeared = false;
                SetVoidInactive();
            }
        }
        void UpdateStage2()
        {
            if (!m_ForestVoid.activeSelf && gameTimer > 210f)
            {
                m_ForestVoid.SetActive(true);
            }
            if (!monsterAppeared && m_ForestVoid.GetComponent<MonsterAI>().GetMonsterState() == MonsterState.APPEAR)
            {
                monsterAppeared = true;
            }
            if (!monsterAppeared && gameTimer > 330f)
            {
                m_ForestVoid.GetComponent<MonsterAI>().m_MonsterStateMachine.SetState(MonsterState.APPEAR);
                monsterAppeared = true;
            }
            if (monsterAppeared && m_ForestVoid.GetComponent<MonsterAI>().GetMonsterState() == MonsterState.HIDDEN_IDLE)
            {
                monsterAppeared = false;
                SetVoidInactive();
                m_MonsterAppear = MonsterAppear.STAGE3;
            }
        }
        void UpdateStage3()
        {
            if (!m_ForestVoid.activeSelf && gameTimer > 330f)
            {
                m_ForestVoid.SetActive(true);
            }
            if (!monsterAppeared && m_ForestVoid.GetComponent<MonsterAI>().GetMonsterState() == MonsterState.APPEAR)
            {
                monsterAppeared = true;
            }
            if (!monsterAppeared && gameTimer > 470f)
            {
                m_ForestVoid.GetComponent<MonsterAI>().m_MonsterStateMachine.SetState(MonsterState.APPEAR);
                monsterAppeared = true;
            }
            if (monsterAppeared && m_ForestVoid.GetComponent<MonsterAI>().GetMonsterState() == MonsterState.HIDDEN_IDLE)
            {
                monsterAppeared = false;
                SetVoidInactive();
                m_MonsterAppear = MonsterAppear.NONE;
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
                m_ForestVoid.SetActive(false);

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
}