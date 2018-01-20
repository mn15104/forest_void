using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ForestItem
  {
    Radio,
    Wood,
 }

public class InventoryScript : MonoBehaviour, ISerializationCallbackReceiver
{
    //Lists for serialization so an editor can be produced for the inventory dictionary. Dumb as hell.
    public List<ForestItem> InventoryMappingKeys = new List<ForestItem>();
    public List<int> InventoryMappingValues = new List<int>();
    //Dictionary mapping items to number of said item.
    public Dictionary<ForestItem, int> InventoryMapping;

    public void Start()
    {
       //Initialise an inventory with 0 of all possible items.
       //(Possibly not efficient if we add a lot of items but it is just an enum tbh.)
        InventoryMapping = new Dictionary<ForestItem, int>();
        var items = System.Enum.GetValues(typeof(ForestItem));

        foreach (ForestItem item in items)
        {
            InventoryMapping.Add(item, 0);   
        }

        InventoryMappingKeys = InventoryMapping.Keys.ToList();
        InventoryMappingValues = InventoryMapping.Values.ToList();

    }

    public void OnBeforeSerialize()
    {
        InventoryMappingKeys.Clear();
        InventoryMappingValues.Clear();

        foreach (var kvp in InventoryMapping)
        {
            InventoryMappingKeys.Add(kvp.Key);
            InventoryMappingValues.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        InventoryMapping = new Dictionary<ForestItem, int>();

        for (int i = 0; i != Math.Min(InventoryMappingKeys.Count, InventoryMappingValues.Count); i++)
            InventoryMapping.Add(InventoryMappingKeys[i], InventoryMappingValues[i]);
    }

    void OnGUI()
    {
        foreach (var kvp in InventoryMapping)
            GUILayout.Label("Key: " + kvp.Key + " value: " + kvp.Value);
    }


    void AddItem(ForestItem item)
    {
        InventoryMapping[item] += 1;
    }

    void RemoveItem(ForestItem item)
    {
        if (InventoryMapping[item] > 0)
        InventoryMapping[item] -= 1;
    }


    // Update is called once per frame
  
}
