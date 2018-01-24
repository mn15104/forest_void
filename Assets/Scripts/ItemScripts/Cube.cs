using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour {

    ForestItem ItemClass;

    private void OnEnable()
    {
        HumanVRLeftHand.OnInteract += AddItemClass;
    }

    // Use this for initialization
    void Start () {
        ItemClass = ForestItem.DruidTome;	
	}
	
	// Update is called once per frame
	void Update () {
       
	}
     
    //Adds itself to the inventory
    private void AddItemClass(GameObject item, GameObject hand)
    {
        hand.GetComponentInChildren<InventoryScript>().AddItem(ItemClass);
        gameObject.SetActive(false);
    }
}
