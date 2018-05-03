using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityStandardAssets.CrossPlatformInput;

public class HumanCamera : MonoBehaviour
{

    private Transform m_Trans;
    private Vector3 m_Forward, m_Right;
    private HumanController m_Character;
    private float m_rotateSpeed = 2f;
    [Range(-1, 1)] private float maxRotateUp = 0.7f, maxRotateDown = -0.7f;
    // Use this for initialization
    void Start()
    {
        m_Trans = GetComponent<Transform>();
        m_Forward = m_Trans.forward;
        m_Character = GetComponentInParent<HumanController>();
    }

    // Update is called once per frame
    void Update()
    {
        /*
        //Vector3 move = new Vector3(1, 0, 1);
        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        float v = CrossPlatformInputManager.GetAxis("Vertical");
        float x = CrossPlatformInputManager.GetAxis("Mouse X");
        float y = CrossPlatformInputManager.GetAxis("Mouse Y");
        //Check input
        
        m_Forward = Vector3.Scale(m_Trans.forward, new Vector3(1, 0, 1)).normalized;
        m_Right = Vector3.Scale(m_Trans.right, new Vector3(1, 0, 1)).normalized;
        Vector3 movement =  v * m_Forward + h * m_Trans.right;
        Vector3 rotation = new Vector3(x, y, 0);
        //Move character & rotate on y axis
        m_Character.Move(movement, rotation);
        //Rotate camera about x axis
        if (y > 0 && transform.forward.y < maxRotateUp || y < 0 && transform.forward.y > maxRotateDown)
            transform.Rotate(Vector3.right, -y * m_rotateSpeed);
        */
    }
}
