using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanVRMovement : MonoBehaviour {

    public SixenseHand rhand = null;
    public SixenseHand lhand = null;
    private Transform m_Transform;
    private Vector3 m_Forward;
    private Vector3 m_Right;


    // Use this for initialization
    void Start () {
        m_Transform = transform;
	}
	
	// Update is called once per frame
	void Update () {
        //foreach (SixenseHand hand in hands)
        float horizontal = lhand.m_controller.JoystickX;
        float vertical = lhand.m_controller.JoystickY;
        float rotate = rhand.m_controller.JoystickX;
        Debug.Log(vertical);
        m_Forward = Vector3.Scale(m_Transform.forward, new Vector3(1, 0, 1)).normalized;
        m_Right = Vector3.Scale(m_Transform.right, new Vector3(1, 0, 1)).normalized;
        Vector3 movement = vertical * m_Forward + horizontal * m_Right;
        Vector3 rotation = new Vector3(rotate, 0, 0);
        GetComponent<HumanVRController>().Move(movement, rotation);
	}
}
