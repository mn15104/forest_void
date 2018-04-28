using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{

    private HingeJoint hinge;
    private JointMotor motor;
    private JointLimits limits;
    public float doorOpenTimer;

    // Use this for initialization
    void Start()
    {
        hinge = GetComponent<HingeJoint>();
        this.motor = hinge.motor;
        this.limits = hinge.limits;
        //motor.targetVelocity = 90;
        //Debug.Log(motor.force);
        //motor.force = 100;
        //Debug.Log(motor.force);
        doorOpenTimer = 10f;
    }

    // Update is called once per frame
    void Update()
    {
        if (hinge.angle >= -90 && hinge.angle < 0)
        {
            doorOpenTimer -= Time.deltaTime;
            this.limits.max = 0;
            this.limits.min = -90;
            if (doorOpenTimer < 0)
            {
                this.motor.targetVelocity = 20;
                this.motor.force = 2f;
            }
        }
        else if (hinge.angle > 0 && hinge.angle <= 90)
        {
            doorOpenTimer -= Time.deltaTime;
            this.limits.max = 90;
            this.limits.min = 0;
            if (doorOpenTimer < 0)
            {
                this.motor.targetVelocity = -20;
                this.motor.force = 2f;
            }
        }
        GetComponent<HingeJoint>().motor = this.motor;
        GetComponent<HingeJoint>().limits = this.limits;
        if (hinge.angle == 0)
        {
            doorOpenTimer = 10f;
            this.motor.targetVelocity = 0;
            this.motor.force = 0;
        }
        GetComponent<AudioSource>().volume = Mathf.Abs(GetComponent<HingeJoint>().velocity/45);
    }
}