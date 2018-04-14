using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuntimePatchRendererScript : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Terrain.activeTerrain.collectDetailPatches = false;
    }

    // Update is called once per frame
    void Update()
    {

    }
}