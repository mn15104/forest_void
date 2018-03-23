using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotifyEvent<T> : ScriptableObject {

    public delegate void GenEvent<T>(T info);
    public event GenEvent<T> notifyEvent;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
