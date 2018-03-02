using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidSystem : MonoBehaviour {
    public GameObject m_ForestVoid;
    public GameObject m_CryptVoid;
    public GameObject m_SpawnpointsHolder;
    public  bool m_VoidSetActive  = false;
    private List<Vector3> m_SpawnPositions = new List<Vector3>();
    private bool m_VoidEnabled = false;
    private float m_DelayTimeToActive = 90f;


    private float gameTimer = 0f;
 
    private void OnEnable()
    {
        m_ForestVoid.GetComponent<MonsterAI>().OnMonsterStateChange += NotifyStateChange;
    }

    void Start () {
        if (m_ForestVoid)
            m_ForestVoid.SetActive(false);
        if (m_CryptVoid)
            m_CryptVoid.SetActive(false);
        foreach(Transform trans in m_SpawnpointsHolder.GetComponentsInChildren<Transform>())
        {
            m_SpawnPositions.Add(trans.position);
        }
        Invoke("SetVoidActive", m_DelayTimeToActive);
        
    }
	
	
	void Update () {
		if(m_VoidSetActive && !m_VoidEnabled)
        {
            CancelInvoke();
            SetVoidActive();
        }
        if (!m_VoidSetActive && m_VoidEnabled)
        {
            SetVoidInactive();
            Invoke("SetVoidActive", m_DelayTimeToActive);
        }
    }

    void SetVoidActive()
    {
        if (!m_VoidEnabled)
        {
            m_VoidSetActive = true;
            m_VoidEnabled = true;
            if (m_ForestVoid)
            {
                m_ForestVoid.transform.position = GetFurthestSpawnPoint();
                m_ForestVoid.SetActive(true);
            }
            if (m_CryptVoid)
                m_CryptVoid.SetActive(true);
        }
    }
    void SetVoidInactive()
    {
        if (m_VoidEnabled)
        {
            m_VoidSetActive = false;
            m_VoidEnabled = false;
            if (m_ForestVoid)
                m_ForestVoid.SetActive(false);
            if (m_CryptVoid)
                m_CryptVoid.SetActive(false);
        }
    }

    void NotifyStateChange(MonsterState state)
    {
        switch (state)
        {
            case MonsterState.HIDDEN_IDLE:
                break;
            case MonsterState.HIDDEN_MOVING:
                break;
            case MonsterState.FOLLOW:
                break;
            case MonsterState.APPEAR:
                break;
            case MonsterState.APPROACH:
                break;
            case MonsterState.CHASE:
                break;
            case MonsterState.GAMEOVER:
                SetVoidInactive();
                break;
            default:
                break;
        }
    }


    Vector3 GetFurthestSpawnPoint()
    {
        Vector3 playerPos = m_ForestVoid.GetComponent<MonsterAI>().player.transform.position;
        Vector3 furthestSpawnPoint = m_SpawnPositions[0];
        foreach(Vector3 pos in m_SpawnPositions)
        {
            if((pos - playerPos).magnitude > (furthestSpawnPoint - playerPos).magnitude)
            {
                furthestSpawnPoint = pos;
            }
        }
        return furthestSpawnPoint;
    }
}
