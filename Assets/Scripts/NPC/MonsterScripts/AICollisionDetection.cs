using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICollisionDetection : MonoBehaviour {

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
            m_monsterAI.NotifyCollisionAhead(false);
        }
	}

    private void OnTriggerEnter(Collider other)
    {
       if(other.gameObject.GetComponent<HumanVRController>() != null)
        {
            if (collisionCount == 0)
            {
                m_monsterAI.NotifyCollisionAhead(true);
            }
            collisionCount++;
        }
    }
    private void OnTriggerExit()
    {
        collisionCount--;
    }
}
