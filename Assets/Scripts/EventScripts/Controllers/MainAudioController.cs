using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainAudioController : MonoBehaviour {

    EventManager eventManager = FindObjectOfType<EventManager>();


    private void Awake()
    {
        eventManager.NotifyRunStamina.NotifyEventOccurred += RunAudio;
        eventManager.NotifyLocation.NotifyEventOccurred += StructureAudio;
        eventManager.NotifyStage.NotifyEventOccurred += StageAudio;
        eventManager.NotifyHeartRate.NotifyEventOccurred += HeartRateAudio;
        //eventManager.BridgeCrossedEvent.TriggerEnterEvent += BridgeTriggerAudio; 

    }


    void RunAudio(bool runStamina)
    {
        if (runStamina)
        {
            //Means is able to run again - not exhausted audio
            Debug.Log("Event: Stamina=True");
            
        }
        else
        {
            //No running left Audio - panting
            Debug.Log("Event: Stamina=False");
        }
    }

    void StructureAudio(EventManager.Location location)
    {
        switch (location)
        {
            case EventManager.Location.Caravan:
                //Caravan Audio
                Debug.Log("Event: Location=Caravan");
                break;
            case EventManager.Location.Chapel:
                Debug.Log("Event: Location=Chapel");
                //Chapel Audio
                break;
            case EventManager.Location.Crypt:
                Debug.Log("Event: Location=Crypt");
                //Crypt Audio
                break;
            case EventManager.Location.Forest:
                Debug.Log("Event: Location=Forest");
                //Forest Audio
                break;
            case EventManager.Location.Generator:
                Debug.Log("Event: Location=Generator");
                //Generator Audio
                break;
            case EventManager.Location.ToolShed:
                Debug.Log("Event: Location=Toolshed");
                //Toolshed Audio
                break;

        }
            
    }

    void StageAudio(EventManager.Stage stage)
    {
        switch (stage)
        {
            case EventManager.Stage.Stage0:
                Debug.Log("Event: Stage=Stage0");
                //Initial Audio
                break;
            case EventManager.Stage.Stage1:
                Debug.Log("Event: Stage=Stage1");
                //Stage 1 Audio
                break;
            case EventManager.Stage.Stage2:
                Debug.Log("Event: Stage=Stage2");
                //Stage 2 Audio
                break;
            case EventManager.Stage.Stage3:
                Debug.Log("Event: Stage=Stage3");
                //Stage 3 Audio
                break;
        }
    }


    void HeartRateAudio(float heartRate)
    {
        //Heart Rate is send in - notifyed based on variable set in eventManager 
        //Currrently set to start notifying after 30sec, every 10sec
    }



    //If you want to also control audio for triggerevents(e.g BridgeTrigger(voice recording), CaravanTrigger(enter door), TextAppear etc
    //Do this e.g BridgeTrigger: uncomment is awake subscription to this event
     
    void BridgeTriggerAudio(GameObject colliderObject)
    {
        //Audio for bridge would go in here. Currently the audio is attached to the trigger itself. Found in the scene -> EventTriggers -> BridgeTrigger
        //Putting audio is here with run when trigger has been entered
 
    }



}
