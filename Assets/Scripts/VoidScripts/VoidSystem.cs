using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidSystem : MonoBehaviour {
    public MonsterAI m_ForestVoid;
    public MonsterAICrypt m_CryptVoid;

    public  bool m_VoidSetActive  = false;
    private bool m_VoidEnabled = false;
    private float m_DelayTimeToActive = 90f;

    void Start () {
        if (m_ForestVoid)
            m_ForestVoid.gameObject.SetActive(false);
        if (m_CryptVoid)
            m_CryptVoid.gameObject.SetActive(false);

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
            m_VoidEnabled = true;
            if (m_ForestVoid)
                m_ForestVoid.gameObject.SetActive(true);
            if (m_CryptVoid)
                m_CryptVoid.gameObject.SetActive(true);
        }
    }
    void SetVoidInactive()
    {
        if (m_VoidEnabled)
        {
            m_VoidEnabled = false;
            if (m_ForestVoid)
                m_ForestVoid.gameObject.SetActive(false);
            if (m_CryptVoid)
                m_CryptVoid.gameObject.SetActive(false);
        }
    }
}
