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
    MonsterAI m_monsterAI;
    int collisionCount = 0;
	// Use this for initialization
	void Start () {
        m_monsterAI = GetComponentInParent<MonsterAI>();
    }
	
	// Update is called once per frame
	void Update () {
		if(collisionCount == 0)
        {
            m_monsterAI.NotifyCollisionAhead(m_CollisionSide, false);
        }
	}

    private void OnTriggerEnter(Collider other)
    {
       if(other.gameObject.layer >= 13 && other.gameObject.layer <= 15)
        {
            if (collisionCount == 0)
            {
                m_monsterAI.NotifyCollisionAhead(m_CollisionSide, true);
            }
            collisionCount++;
        }
    }
    private void OnTriggerExit()
    {
        collisionCount--;
    }
}
