using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<InventoryEvidence> inventory;
    public static Inventory instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        inventory = new List<InventoryEvidence>();
    }
}
