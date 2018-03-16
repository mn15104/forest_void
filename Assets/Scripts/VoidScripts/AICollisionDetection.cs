using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AICollisionSide
{
    LEFT,
    RIGHT,
    NONE
}

public class AICollisionDetection : MonoBehaviour {


    public AICollisionSide m_CollisionSide;
    private MonsterAI m_monsterAI;
    private MonsterAIPassive m_monsterAIPassive;
    private int collisionCount;
    private bool passive = false;
    // Use this for initialization
    private void OnEnable()
    {
        m_monsterAI = GetComponentInParent<MonsterAI>();
        if (!m_monsterAI)
        {
            m_monsterAIPassive = GetComponentInParent<MonsterAIPassive>();
            passive = true;
        }
        collisionCount = 0;
    }
    private void OnDisable()
    {
        collisionCount = 0;
    }

    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		if(collisionCount == 0)
        {
            if(!passive)
                m_monsterAI.NotifyCollisionAhead(m_CollisionSide, false);
            else 
                m_monsterAIPassive.NotifyCollisionAhead(m_CollisionSide, false);
        }
	}

    private void OnTriggerEnter(Collider other)
    {
       if(other.gameObject.layer >= 13 && other.gameObject.layer <= 15)
        {
            if (collisionCount == 0)
            {
                if (!passive)
                    m_monsterAI.NotifyCollisionAhead(m_CollisionSide, true);
                else
                    m_monsterAIPassive.NotifyCollisionAhead(m_CollisionSide, true);
            }
            collisionCount++;
        }
    }
    private void OnTriggerExit()
    {
        if(collisionCount > 0)
            collisionCount--;
    }
}
