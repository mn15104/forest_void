using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChapelTriggerType {
    ENTRANCE
}

public class ChapelScript : MonoBehaviour, AudioEvent {

    AudioController audController;

 
    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Notify(ChapelTriggerType trig)
    {
        switch(trig) {
            case ChapelTriggerType.ENTRANCE:
                
                break;
            default:
                break;
        }
    }
}
