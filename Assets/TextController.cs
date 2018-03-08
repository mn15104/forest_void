using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TextController : MonoBehaviour {

    public float amplitude;
    public float speed;

	// Use this for initialization
	void Start () {
        amplitude = 0.0007f;
        speed = 3f;
	}
	
	// Update is called once per frame
	void Update () {
        var y0 = transform.position.y;
        transform.position = new Vector3(transform.position.x, y0 + amplitude * Mathf.Sin(speed * Time.time), transform.position.z);
        //transform.position.Set(transform.position.x, y0 + amplitude * Mathf.Sin(speed * Time.time), transform.position.z); 
		
	}
}
