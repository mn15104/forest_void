using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLight : MonoBehaviour {

    private Light f_Light;
    private float timer;

	// Use this for initialization
	void Start () {
        f_Light = gameObject.GetComponent<Light>();
        f_Light.intensity = 3;
    }


	// Update is called once per frame
	void Update () {
        int chance = Random.Range(0, 101);
        Debug.Log("hi");
        if (chance < 80)
        {
            f_Light.intensity = 3;
        }
        else
        {
            f_Light.intensity = 1;
            timer = 5.0f;
            while (timer > 0)
            {
                timer = timer - Time.time;
            }
           
        }
        
		
	}

    //IEnumerator timer()
    //{
    //    yield return new WaitForSeconds(5);
    //}
}
