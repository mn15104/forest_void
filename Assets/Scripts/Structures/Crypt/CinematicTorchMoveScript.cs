using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicTorchMoveScript : MonoBehaviour {

    public GameObject torch;
    public GameObject monster;
    public float TorchMoveSpeed;
    private bool triggered = false;
    private float triggeredTime;
    public GameObject TargetPoint;
    public AudioSource jumpScare;
    public bool PlayedMusic;
    public float maxTime;
    public float foo;
    public List<Light> CandleLights;
    public List<GameObject> Candles;
    private bool EventFinished;
    protected EventManager eventManager;


    // Use this for initialization
    void Start () {
        TorchMoveSpeed = 0.45f;
        PlayedMusic = false;
        foo = 0f;
        EventFinished = false;
        torch.SetActive(false);
        eventManager = FindObjectOfType<EventManager>();
        foreach (GameObject Candle in Candles)
        {
            Candle.GetComponent<MeshRenderer>().enabled = true;
            //Candle.intensity = 0f;
        }
    }
	
	// Update is called once per frame
	void Update () {
		if (triggered && foo < maxTime)
        {
            foo += Time.deltaTime;
            Debug.Log("Lerping");
            Debug.Log(torch.transform.position);
            torch.transform.position = Vector3.Lerp(torch.transform.position, TargetPoint.transform.position, Time.deltaTime * TorchMoveSpeed);
        }
        if (foo >= maxTime && !EventFinished)
        {
            
            foreach (Light CandleLight in CandleLights)
            {
                CandleLight.GetComponent<LightFlicker>().enabled = false;
                CandleLight.intensity = 0f;      
            }
            foreach (GameObject Candle in Candles)
            {
                Candle.GetComponent<MeshRenderer>().enabled = false;
                //Candle.intensity = 0f;
            }

            torch.SetActive(false);
            monster.SetActive(false);
            EventFinished = true;
        }


    }

    private void OnTriggerEnter(Collider other)
    {



        triggeredTime = Time.time;
        if(other == eventManager.player.GetComponent<Collider>())
        {
            triggered = true;
            if (!PlayedMusic)
            {
                torch.SetActive(true);
                jumpScare.Play();
                PlayedMusic = true;
            }
        }
    }


}
