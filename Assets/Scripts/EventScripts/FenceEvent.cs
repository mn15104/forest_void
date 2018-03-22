using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FenceEvent : TextEvent {

	// Use this for initialization
    
	 public override void OnEnable () {
        base.OnEnable();
        trigger = eventManager.FenceTextTriggerEvent;
	}
   
    
}
