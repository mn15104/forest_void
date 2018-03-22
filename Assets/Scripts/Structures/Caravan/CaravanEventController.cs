using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaravanEventController : MonoBehaviour {

    public CaravanDoorController caravanDoor;
    public CaravanDoorController toiletDoor;
    public GameObject human;

    public float DoorWait;
    public float MonsterWait;
    public float MonsterCycle;
    public bool MonsterAppeared;
    private int[] playingInds;

    public FlickeringLight light;
    public AudioSource eventClip;
    public float maxIntensity;
    public GameObject monster;
    bool TriggerTriggered = false;
    public float MonsterDistance = 5.0f;
    public Color lightval;

    public float monsterMaxX;
    public float monsterMaxY;
    public float monsterMaxZ;
    public float monsterMinX;
    public float monsterMinY;
    public float monsterMinZ;

    // Use this for initialization
    void Start () {
        monster.SetActive(false);
        MonsterAppeared = false;
    }
	
    void monsterAppear(float distance)
    {
        var cameraForward = human.GetComponentInChildren<Camera>().transform.forward;
        var playerTransform = human.GetComponentInChildren<Camera>().transform;
        var monsterpos = (cameraForward * distance) + playerTransform.position;
        monsterpos.y = 73.95f;
        monster.transform.position = monsterpos;
        monster.transform.forward = -cameraForward;
    }

	// Update is called once per frame
	void Update () {
        Debug.Log(human.GetComponentInChildren<Camera>().transform.forward);

	}


    //Do the Door event as a coroutine i.e. open the door, yield for the amount of time desired, then open the door;
    IEnumerator DoorEvent()
    {
        yield return new WaitForSeconds(1);
        caravanDoor.door = DoorState.Closing;
        yield return new WaitForSeconds(DoorWait);
        caravanDoor.door = DoorState.Opening;
        toiletDoor.door = DoorState.Opening;


    }
    IEnumerator AudioEvent()
    {

        
        eventClip.Play();
        yield return new WaitForSeconds(1.8f);
        //Mute All Audio Sources
        playingInds = new int[50];
        AudioSource[] sources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
        int noOfPlaying = 0;
        for (int index = 0; index < sources.Length; ++index)
        {   
            //Store audios playing
            if (sources[index].isPlaying)
            {
                if (sources[index] == eventClip)
                {
                    print("NOPE");
                }
                else
                {
                    playingInds[noOfPlaying] = index;
                    noOfPlaying++;
                    sources[index].mute = true;
                }

            }
        }
        
        yield return new WaitForSeconds(DoorWait);
        yield return new WaitForSeconds(5);
        eventClip.Stop();
        
        //Replay The Audio Sources that were playing
        for (int index = 0; index < playingInds.Length; index++)
        {
            if (sources[index] == eventClip)
            {
                print("NOPE");
            }
            sources[playingInds[index]].mute = false ;
        }

    }

    IEnumerator LightChange()
    {

        MonsterAppeared = true;
        monster.SetActive(false);
        light.enabled = true;

    
        print("Starting Monster Flicker");
        //yield return new WaitForSeconds(1.8f);
        yield return new WaitForSeconds(22.1f);

        //Light Off 1
        print("Light Off");
        light.enabled = false;
        light.f_Light.intensity = 0;
        RenderSettings.fogDensity = 0.6F;
        yield return new WaitForSeconds(1.3f);

        //Light On Monster On 1
        print("Light On Monster On");
        RenderSettings.fogDensity = 0.25F;
        light.f_Light.intensity = 0.3f;
        monsterAppear(10);
        monster.SetActive(true);
        RenderSettings.fogDensity = 0.25F;
        yield return new WaitForSeconds(1.3f);

        //Light Off Monster Off 1
        print("Monster Off");
        RenderSettings.fogDensity = 0.6F;
        light.f_Light.intensity = 0;
        monster.SetActive(false);
        yield return new WaitForSeconds(0.4f);

        //Light On 1
        RenderSettings.fogDensity = 0.25F;
        light.enabled = true;
        MonsterAppeared = true;
        yield return new WaitForSeconds(7.4f);


        //Light Off 2
        print("Light Off");
        light.enabled = false;
        light.f_Light.intensity = 0;
        RenderSettings.fogDensity = 0.6F;
        yield return new WaitForSeconds(1);

        //Light On Monster On 2
        print("Light On Monster On");
        RenderSettings.fogDensity = 0.25F;
        light.f_Light.intensity = 0.5f;
        monsterAppear(5);
        monster.SetActive(true);
        yield return new WaitForSeconds(0.6f);

        //Light Off Monster Off 2
        print("Monster Off");
        RenderSettings.fogDensity = 0.6F;
        light.f_Light.intensity = 0;
        monster.SetActive(false);
        yield return new WaitForSeconds(0.4f); 

        //Light On 2
        RenderSettings.fogDensity = 0.25F;
        light.enabled = true;
        yield return new WaitForSeconds(9.0f);


        //Light Off 3
        print("Light Off");
        light.enabled = false;
        light.f_Light.intensity = 0;
        RenderSettings.fogDensity = 0.6F;
        yield return new WaitForSeconds(1.2f);

        //Light On Monster On 3
        print("Light On Monster On");
        RenderSettings.fogDensity = 0.25F;
        light.f_Light.intensity = 0.5f;
        monsterAppear(3);
        monster.SetActive(true);
        yield return new WaitForSeconds(1.5f);

        //Light Off Monster Off 3
        print("Monster Off");
        RenderSettings.fogDensity = 0.6F;
        light.f_Light.intensity = 0;
        monster.SetActive(false);
        yield return new WaitForSeconds(2.1f);

        //Light On 3
        RenderSettings.fogDensity = 0.25F;
        light.enabled = true;
        light.f_Light.color = lightval;
        yield return new WaitForSeconds(2.4f);//Gets to 50secs

    }

        private void OnTriggerEnter(Collider other)
    {

        if(other.gameObject == human && TriggerTriggered == false)
        {
            lightval = light.f_Light.color;
            light.f_Light.color = Color.red;
            TriggerTriggered = true;
            StartCoroutine("DoorEvent");

            StartCoroutine("AudioEvent");
            //eventClip.Play();
            StartCoroutine("LightChange");
            //StartCoroutine("monsterAppear");
            //StartCoroutine("playAudio");#


        }
    }
}
