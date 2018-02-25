using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    public List<GameObject> keys;


    void addKeyToInventory(GameObject key)
    {
        keys.Add(key);
    }

    void removeKeyFromInventory(GameObject key)
    {
        keys.Remove(key);
    }

}
