using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainAudioController : MonoBehaviour {

    EventManager eventManager = FindObjectOfType<EventManager>();


    private void Awake()
    {
        eventManager.NotifyNoRun.notifyEvent += RunAudio;
        eventManager.NotifyLocation.notifyEvent += StructureAudio;
        eventManager.NotifyStage.notifyEvent += StageAudio;
        eventManager.NotifyHeartRate.notifyEvent += HeartRateAudio;
    }


    void RunAudio(bool running)
    {
        if (running)
        {
            //Running available Audio
        }
        else
        {
            //No running left Audio
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

}
