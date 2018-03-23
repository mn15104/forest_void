using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainAudioController : MonoBehaviour {

    EventManager eventManager = FindObjectOfType<EventManager>();


    private void Awake()
    {
        eventManager.NotifyRunStamina.notifyEvent += RunAudio;
        eventManager.NotifyLocation.notifyEvent += StructureAudio;
        eventManager.NotifyStage.notifyEvent += StageAudio;
        eventManager.NotifyHeartRate.notifyEvent += HeartRateAudio;
        //eventManager.BridgeCrossedEvent.TriggerEnterEvent += BridgeTriggerAudio; 

    }


    void RunAudio(bool runStamina)
    {
        if (runStamina)
        {
            //Means is able to run again - not exhausted audio
            
        }
        else
        {
            //No running left Audio - panting
        }
    }

    void StructureAudio(EventManager.Location location)
    {
        switch (location)
        {
            case EventManager.Location.Caravan:
                //Caravan Audio
                break;
            case EventManager.Location.Chapel:
                //Chapel Audio
                break;
            case EventManager.Location.Crypt:
                //Crypt Audio
                break;
            case EventManager.Location.Forest:
                //Forest Audio
                break;
            case EventManager.Location.Generator:
                //Generator Audio
                break;
            case EventManager.Location.ToolShed:
                //Toolshed Audio
                break;

        }
            
    }

    void StageAudio(EventManager.Stage stage)
    {
        switch (stage)
        {
            case EventManager.Stage.Stage0:
                //Initial Audio
                break;
            case EventManager.Stage.Stage1:
                //Stage 1 Audio
                break;
            case EventManager.Stage.Stage2:
                //Stage 2 Audio
                break;
            case EventManager.Stage.Stage3:
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
