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
    private float GameTimerSeconds = 0f; 

    public TriggerEvent TextTriggerEvent;
    public TriggerEvent BridgeCrossedEvent;
    public TriggerEvent ChapelBackDoorHandEvent;
    public TriggerEvent CaravanTriggerEvent;
    public TriggerEvent GeneratorZoneTriggerEvent;
    public TriggerEvent StructureZoneTriggerEvent;

    public NotifyEvent<float> NotifyHeartRate;
    public NotifyEvent<Stage> NotifyStage;
    public NotifyEvent<Location> NotifyLocation;
    public NotifyEvent<bool> NotifyRunStamina;
    public NotifyEvent<bool> NotifyTorchPressed;

    private VoidSystem voidSys;
    private Stage currentStage;
    private Location currentLocation;

    public GameObject player;
    public GameObject monster;

    private float startNotifyingHeartRate = 20;
    private float NotifyHeartRateInterval = 3;
    
   
    public void Awake()
    {
       
        NotifyHeartRate = new NotifyEvent<float>();
        NotifyStage = new NotifyEvent<Stage>();
        NotifyLocation = new NotifyEvent<Location>();
        NotifyRunStamina = new NotifyEvent<bool>();
        NotifyTorchPressed = new NotifyEvent<bool>();

        BridgeCrossedEvent = new TriggerEvent();
        ChapelBackDoorHandEvent = new TriggerEvent();
        CaravanTriggerEvent = new TriggerEvent();
        GeneratorZoneTriggerEvent = new TriggerEvent();
        TextTriggerEvent = new TriggerEvent();
        StructureZoneTriggerEvent = new TriggerEvent();

        currentStage = Stage.Intro;
        currentLocation = Location.Forest;
        EventManagerSubscriptions();
    }

    public void Start()
    {
        //E.G Notify heart rate to all listeners, after 20sec -> every 3 sec
        InvokeRepeating("PassHeartRate", startNotifyingHeartRate, NotifyHeartRateInterval);
        NotifyStage.Notify(currentStage);
    }

    void EventManagerSubscriptions()
    {
        voidSys.NotifyStage.NotifyEventOccurred += SetStage;
        StructureZoneTriggerEvent.TriggerEnterEvent += SetStructureLocation;
        StructureZoneTriggerEvent.TriggerExitEvent += SetForestLocation;
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
