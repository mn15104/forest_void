using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICollisionDetection : MonoBehaviour {

    public enum CollisionSide
    {
        LEFT,
        RIGHT
    }
    public CollisionSide m_CollisionSide;
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
       if(other.gameObject.layer == 13 || other.gameObject.layer == 14)
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
