using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Trees to be enabled to be trigggered from beginning of chase scene
public class BarrierTrees : MonoBehaviour {

    private List<GameObject> trees = new List<GameObject>();

    void OnEnable()
    {
        AppleScript_2.OnAlert += MakeBarrier;
    }
    void OnDisable()
    {
        AppleScript_2.OnAlert -= MakeBarrier;
    }

	// Use this for initialization
	void Start () {
        foreach(MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>())
        {
            GameObject tree = mesh.gameObject;
            trees.Add(tree);
            mesh.enabled = false;
            tree.GetComponent<MeshCollider>().enabled = false;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void MakeBarrier(GameObject human)
    {
        foreach(GameObject tree in trees)
        {
            tree.GetComponent<MeshRenderer>().enabled = true;
            tree.GetComponent<MeshCollider>().enabled = true;
        }
    }
}
