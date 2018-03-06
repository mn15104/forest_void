using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapelChildTrigger : MonoBehaviour {

    ChapelScript m_Chapel;

	// Use this for initialization
	void Start () {
        m_Chapel = GetComponentInParent<ChapelScript>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<HumanVRController>()
            || other.GetComponent<HumanController>())
        {
            m_Chapel.Notify(ChapelTriggerType.ENTRANCE);
        }
    }
}
