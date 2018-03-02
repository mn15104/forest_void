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
    private int collisionCount;
    // Use this for initialization
    private void OnEnable()
    {
        m_monsterAI = GetComponentInParent<MonsterAI>();
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
        if(collisionCount > 0)
            collisionCount--;
    }
}
