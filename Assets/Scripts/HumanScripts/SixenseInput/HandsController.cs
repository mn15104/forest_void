using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandsController : MonoBehaviour {
    float m_sensitivity = 0.003f;
    public GameObject leftHand;
    public GameObject rightHand;

    // Use this for initialization
    void Start () {
        //leftHand.transform.localRotation = GetParentComponent<rotation>();
	}
	
	// Update is called once per frame
	void Update () {
        leftHand.transform.localPosition = SixenseInput.Controllers[0].Position * m_sensitivity;
        rightHand.transform.localPosition = SixenseInput.Controllers[1].Position * m_sensitivity;
        leftHand.transform.localRotation = SixenseInput.Controllers[0].Rotation;
        rightHand.transform.localRotation = SixenseInput.Controllers[1].Rotation;
        
    }
}
