﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextEvent : MonoBehaviour {

    protected TextController textController;
    protected EventManager eventManager;
    protected TriggerEvent trigger;

    // Use this for initialization
    public virtual void Awake()
    {
        textController = GetComponent<TextController>();
        eventManager = FindObjectOfType<EventManager>();
        trigger = eventManager.TextTriggerEvent;
    }

    public virtual void OnEnable()
    {
    }

    protected void Start()
    {
        trigger.TriggerEnterEvent += textController.ShowText;

        trigger.TriggerExitEvent += textController.HideText;
    }

    public void onDisable()
    {
        trigger.TriggerEnterEvent -= textController.ShowText;
        trigger.TriggerExitEvent -= textController.HideText;
    }

    private void OnTriggerEnter(Collider other)
    {
        trigger.TriggerEnter(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        trigger.TriggerExit(other.gameObject);

    }
}