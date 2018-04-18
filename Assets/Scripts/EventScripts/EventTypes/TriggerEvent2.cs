using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEvent2
{

    // Use this for initialization
    public delegate void Events(GameObject Player, GameObject SceneObject);
    public event Events TriggerEnterEvent;
    public event Events TriggerExitEvent;

    public void TriggerEnter(GameObject Player, GameObject SceneObject)
    {
        TriggerEnterEvent(Player, SceneObject);
    }
    public void TriggerExit(GameObject Player, GameObject SceneObject)
    {
        TriggerExitEvent(Player, SceneObject);
    }

}

