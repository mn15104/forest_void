using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidSystem : MonoBehaviour {
    public GameObject m_ForestVoid;
    public GameObject m_CryptVoid;

    public  bool m_VoidSetActive  = false;
    private bool m_VoidEnabled = false;
    private float m_DelayTimeToActive = 90f;

    private void OnEnable()
    {
        m_ForestVoid.GetComponent<MonsterAI>().OnMonsterStateChange += NotifyStateChange;
    }

    void Start () {
        if (m_ForestVoid)
            m_ForestVoid.SetActive(false);
        if (m_CryptVoid)
            m_CryptVoid.SetActive(false);

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
                m_ForestVoid.SetActive(true);
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
                float new_x = Random.Range(870, 786);
                float new_z = Random.Range(535, 598);
                m_ForestVoid.transform.position = new Vector3(new_x, m_ForestVoid.transform.position.y , new_z);
                SetVoidInactive();
                break;
            default:
                break;
        }
    }
}
