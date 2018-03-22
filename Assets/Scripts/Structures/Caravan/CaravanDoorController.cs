﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DoorState { Opening, Closing, Static };


public class CaravanDoorController : MonoBehaviour
{


    public float smooth;
    public float x;
    public float y;
    public float angle;
    public float z;
    public int count;
    public int currentCount;
    public DoorState door;

    // Use this for initialization
    void Start()
    {
        currentCount = 0;
        door = DoorState.Static;
    }

    // Update is called once per frame
    void Update()
    {
        count = (int) (angle / smooth);
        var point = new Vector3(x, y, z);
        if (door == DoorState.Opening && currentCount < count)
        {
            transform.RotateAround(point, Vector3.up, -smooth);
            currentCount++;
        }
        if (door == DoorState.Closing && currentCount < count)
        {
            transform.RotateAround(point, Vector3.up, smooth);
            currentCount++;
        }
        if (currentCount >= count)
        {
            currentCount = 0;
            door = DoorState.Static;
        }
    }
}
