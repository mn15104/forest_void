using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapelDoorEvent : TextEvent
{
    
    public override void OnEnable()
    {
        base.OnEnable();
        trigger = eventManager.ChapelTriggerEvent;
        Debug.Log(trigger);
    }
    
}
