using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BluetoothAPI : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(getHeartRate());  
	}

    IEnumerator getHeartRate()
    {
        using (UnityWebRequest webaddress = UnityWebRequest.Get("10.254.225.25:5005"))
        {
            yield return webaddress.SendWebRequest();

            if(webaddress.isNetworkError || webaddress.isHttpError)
            {
                Debug.Log("Get Request Error");
            }
            else
            {
                Debug.Log("Data Receieved");
                byte[] results = webaddress.downloadHandler.data;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
