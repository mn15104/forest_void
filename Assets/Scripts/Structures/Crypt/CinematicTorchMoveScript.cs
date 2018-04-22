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


    // Use this for initialization
    void Start () {
        TorchMoveSpeed = 0.25f;
        PlayedMusic = false;
        foo = 0f;
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
        if (foo >= maxTime)
        {
            torch.SetActive(false);
            monster.SetActive(false);
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        triggeredTime = Time.time;
        
        triggered = true;
        if (!PlayedMusic)
        {
            jumpScare.Play();
            PlayedMusic = true;
        }
    }


}
