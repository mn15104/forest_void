using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    public List<GameObject> keys;


    public void addKeyToInventory(GameObject key)
    {
        keys.Add(key);
    }

    public void removeKeyFromInventory(GameObject key)
    {
        keys.Remove(key);
    }

    public GameObject peekInventory()
    {
        return keys[0];
    }

    public int inventorySize()
    {
        return keys.Count;
    }

}
