using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{

    public Light f_Light;
    private float currentTimer;
    public float maxTimer;
    public float maxTimeDiff;
    public float lowerBound;
    public float setLowIntensity;
    public float setHighIntensity;
    public float higherBound;
    private float highIntensity;
    private float lowIntensity;

    // Use this for initialization
    void Start()
    {
        //setlowIntensity = 1f;
        //sethighIntensity = 3f;
        //lowIntensity = 1f;
        //highIntensity = 3f;
        //lowerBound = 0.5f;
        //higherBound = 4f;
        //maxTimeDiff = 2f;
        //maxTimer = 5f;
        f_Light = gameObject.GetComponent<Light>();
        f_Light.intensity = highIntensity;
        
    }


    // Update is called once per frame
    void Update()
    {
        if (currentTimer > maxTimer)
        {
            int chance = Random.Range(0, 101);
            if (chance < 90)
            {
                f_Light.intensity = highIntensity;
                currentTimer = 0;
                maxTimer = Random.Range(0, maxTimeDiff);
                highIntensity = Random.Range(setHighIntensity, higherBound);
                lowIntensity = Random.Range(lowerBound, setLowIntensity);
                
            }
        }
        else
        {
            f_Light.intensity = lowIntensity;
            currentTimer += Time.deltaTime;
        }

        //IEnumerator timer()
        //{
        //    Debug.Log("running couroutine");
        //    yield return WaitForSeconds(5);
        //}
    }
}
