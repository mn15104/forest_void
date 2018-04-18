using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextEvent : MonoBehaviour {

    protected TextController textController;
    protected EventManager eventManager;
    protected TriggerEvent2 trigger;
    Camera cam;
    GameObject player;

    // Use this for initialization
    public virtual void Awake()
    {
        textController = GetComponent<TextController>();
        eventManager = FindObjectOfType<EventManager>();
        trigger = eventManager.TextTriggerEvent;
        cam = eventManager.player.GetComponentInChildren<Camera>();
        player = eventManager.player;
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
        trigger.TriggerEnter(other.gameObject, transform.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        trigger.TriggerExit(other.gameObject, transform.gameObject);

    }
    private void Update()
    {
        //Vector3 v = player.transform.position - transform.position;
        //v.x = v.z = 0.0f;
        //transform.LookAt(player.transform.position - v );
        //transform.Rotate(0, 180, 0);
    }
}
