using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioController : MonoBehaviour {

    public float detectionRange;

    public Camera player1Camera;

    public InventoryScript playerInventory;

    private Plane[] player1CameraPlanes;

    private bool player1PickUp;
    private int numWood;

    // Use this for initialization
    void Start ()
    {
        player1CameraPlanes = GeometryUtility.CalculateFrustumPlanes(player1Camera);
        Debug.Log(player1CameraPlanes);
    }
	
	// Update is called once per frame
	void Update ()
    {
        
        //if (Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player 1").transform.position) <= detectionRange)
        //{
        //    if (Input.GetButtonDown("PickUp"))
        //    {
        //        this.gameObject.SetActive(false);

        //    }
        //}
        player1CameraPlanes = GeometryUtility.CalculateFrustumPlanes(player1Camera);
        if (InView() && DistanceToPlayer() < detectionRange){
            Debug.Log("In View & Distance");
            if (Input.GetButtonDown("PickUp"))
            {
                Debug.Log("picked up");
                //playerInventory.InventoryMapping[ForestItem.Wood] += 1;
                this.gameObject.SetActive(false);
            }
        }
        else Debug.Log("Not in View or distance or both");
	}

    bool InView()
    {
        if (GeometryUtility.TestPlanesAABB(player1CameraPlanes, GetComponent<Collider>().bounds))
            return true;
        else return false;
    }

    float DistanceToPlayer()
    {
        float dis = Vector3.Distance(transform.position, player1Camera.transform.position);
        return dis; 
    }

}
