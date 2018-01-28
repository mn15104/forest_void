using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum GameEventStage
{
    Unknown
}

public class EventController : MonoBehaviour {

    //Events
    private GameEventStage m_GameEventStage;
    private AudioAndLightController m_AudioAndLightController;
    private NarrativeController m_NarrativeController;

    private void OnEnable()
    {
        MonsterAI.OnStateChange += MonsterStateChange;
    }

    void Start()
    {
        m_AudioAndLightController = FindObjectOfType<AudioAndLightController>();
        m_NarrativeController = FindObjectOfType<NarrativeController>();
    }

    private void MonsterStateChange(MonsterAI.State monsterStateChange)
    {
        if(monsterStateChange == MonsterAI.State.CHASE)
            m_AudioAndLightController.TriggerMonsterChase();
        else if (monsterStateChange == MonsterAI.State.HIDDEN)
            m_AudioAndLightController.TriggerMonsterHidden();
        else if(monsterStateChange == MonsterAI.State.APPEAR)
            m_AudioAndLightController.TriggerMonsterAppear();
    }


    // Use this for initialization
  
	
	// Update is called once per frame
	void Update () {

	}

    
}



//public GameEventStage GetGameEventStage()
//{
//    return m_GameEventStage;
//}

//public void TriggerEvent(GameEventStage gameEvent)
//{
//    if (!m_DisableChronoligicalEvents)
//    {
//        if (m_GameEventStage + 1 == gameEvent)
//        {
//            m_GameEventStage = gameEvent;

//        }
//    }
//}