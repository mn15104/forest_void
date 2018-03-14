using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaravanEventController : MonoBehaviour {

    public CaravanDoorController caravanDoor;
    public GameObject human;
    public float DoorWait;
    public FlickeringLight light;
    public AudioSource eventClip;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //Do the Door event as a coroutine i.e. open the door, yield for the amount of time desired, then open the door;
    IEnumerator DoorEvent()
    {
        caravanDoor.door = DoorState.Closing;
        yield return new WaitForSeconds(DoorWait);
        caravanDoor.door = DoorState.Opening;

    }

    IEnumerator LightChange()
    {
        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == human)
        {
            StartCoroutine("DoorEvent");
            eventClip.Play();
            StartCoroutine("LightChange");
            //StartCoroutine("monsterAppear");
            //StartCoroutine("playAudio");
        }
    }
}
