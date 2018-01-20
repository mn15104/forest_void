using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameEventStage
{
    WalkingThroughDaylightForest,
    EnterFarm,
    ChasedToBridge,
    EndOfChase,
    EnteringDarknessForest,
    WalkingThroughDarknessForest
}

public class EventController : MonoBehaviour {

    //Events
    public bool m_DisableChronoligicalEvents;
    public GameObject m_DaylightForest;
    public GameObject m_DarknessForest;

    private GameEventStage m_GameEventStage; 
    private AudioAndLightController m_AudioAndLightController;
    private NarrativeController m_NarrativeController;
    // Use this for initialization
    void Start () {
        m_GameEventStage = GameEventStage.WalkingThroughDaylightForest;
        m_AudioAndLightController = FindObjectOfType<AudioAndLightController>();
        m_NarrativeController = FindObjectOfType<NarrativeController>();
    }
	
	// Update is called once per frame
	void Update () {
        Debug.Log(m_GameEventStage);
	}

    public GameEventStage GetGameEventStage()
    {
        return m_GameEventStage;
    }

    public void TriggerEvent(GameEventStage gameEvent)
    {
        if (!m_DisableChronoligicalEvents)
        {
            if (m_GameEventStage + 1 == gameEvent)
            {
                m_GameEventStage = gameEvent;
               
            }
        }
    }

    public void SetSceneToDarkness(bool setDark)
    {
        m_AudioAndLightController.SetLightSettingsToDark(setDark);
        if (setDark)
        {
            foreach(Terrain ter in m_DaylightForest.GetComponents<Terrain>())
            {
                ter.enabled = false;
            }
        }
    }
}
