using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This is the event manager of which a new instance is attached to every area. Once the player enters the area a set of events will be triggered. 
public class EventManager : MonoBehaviour {

    public enum Stage
    {
        Intro,Stage1,Stage2,Stage3,GameOverStage 
    }
   
    public enum Location
    {
        Chapel,Forest,Crypt,ToolShed, Caravan, Generator
    }

    private float[] StageTimes = { 120f, 600f, 720f };
    public float GameTimerSeconds = 0f; 

    public TriggerEvent TextTriggerEvent = new TriggerEvent();
    public TriggerEvent BridgeCrossedEvent = new TriggerEvent();
    public TriggerEvent ChapelBackDoorHandEvent = new TriggerEvent();
    public TriggerEvent CaravanTriggerEvent = new TriggerEvent();
    public TriggerEvent GeneratorZoneTriggerEvent = new TriggerEvent();
    public TriggerEvent StructureZoneTriggerEvent = new TriggerEvent();

    public NotifyEvent<float> NotifyHeartRate = new NotifyEvent<float>();
    public NotifyEvent<Stage> NotifyStage = new NotifyEvent<Stage>();
    public NotifyEvent<Location> NotifyLocation = new NotifyEvent<Location>();
    public NotifyEvent<bool> NotifyRunStamina = new NotifyEvent<bool>();
    public NotifyEvent<bool> NotifyTorchPressed = new NotifyEvent<bool>();

    private VoidSystem voidSys;
    public Stage currentStage;
    private Location currentLocation;

    public GameObject player;
    public GameObject monster;
    public GameObject playerSpawnPoint;
    public bool StoryMode = false;
    private float startNotifyingHeartRate = 20;
    private float NotifyHeartRateInterval = 3;

    private void OnEnable()
    {
        voidSys = FindObjectOfType<VoidSystem>();
        currentStage = Stage.Intro;
        currentLocation = Location.Forest;
        voidSys.NotifyStage.NotifyEventOccurred += SetStage;
        StructureZoneTriggerEvent.TriggerEnterEvent += SetStructureLocation;
        StructureZoneTriggerEvent.TriggerExitEvent += SetForestLocation;
    }


    public void Start()
    {
        
        InvokeRepeating("PassHeartRate", startNotifyingHeartRate, NotifyHeartRateInterval);
        NotifyStage.Notify(currentStage);
        if (StoryMode)
        {
            player.transform.position = playerSpawnPoint.transform.position;
        }

    }

    void EventManagerSubscriptions()
    {

    }

    public float getPlayerHeartrate()
    {
        return player.GetComponentInChildren<Heartbeat>().m_Heartbeat;
    }

    public int getCurrentKeyCount()
    {
        return player.GetComponent<Inventory>().keys.Count;
        
    }

    // VoidSystem notifies EventManager of stage, EventManager notifies every other necessary script
    void SetStage(Stage t_stage)
    {
        currentStage = t_stage;
        NotifyStage.Notify(currentStage);
    }

    void PassHeartRate()
    {
        float currentHeartRate = getPlayerHeartrate();
        NotifyHeartRate.Notify(currentHeartRate);
    }


    void Update()
    {
        GameTimerSeconds += Time.deltaTime;
    }

    void SetStructureLocation(GameObject gameObject)
    {
        currentLocation = gameObject.GetComponent<StructureZone>().location;
        NotifyLocation.Notify(currentLocation);
    } 

    void SetForestLocation(GameObject gameObject)
    {
        currentLocation = Location.Forest;
        NotifyLocation.Notify(currentLocation);
    }

    public float GetGameTime()
    {
        return GameTimerSeconds;
    }
}
