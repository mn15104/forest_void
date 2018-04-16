using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingSpaceController : MonoBehaviour {

    public float CurrentRotateTime;

    private Quaternion previousLocation;
    private Transform ToBeLookedAt;

    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void VRLookAT(Transform toBeLookedAt, float time)
    {
        CurrentRotateTime = time;
        ToBeLookedAt = toBeLookedAt;
        StartCoroutine("VrLooking");
    }

    IEnumerator VrLooking()
    {
        float elapsedTime = 0f;
        var neededRotation = Quaternion.LookRotation(ToBeLookedAt.position - transform.position);
        while((neededRotation != transform.rotation) && elapsedTime < 5f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, neededRotation, Time.deltaTime * CurrentRotateTime);
            elapsedTime += Time.deltaTime;
            Debug.Log(elapsedTime);
            yield return null;
        }
        Debug.Log("finished looking at");
    }

    public void VRLookBack()
    {
        transform.rotation = previousLocation;
    }
}
