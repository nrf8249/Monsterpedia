using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<InventoryEvidence> inventory;
    public static Inventory instance;
    public GameObject openButtonCanvas;
    public GameObject inventoryCanvas;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        inventory = new List<InventoryEvidence>();
        openButtonCanvas.SetActive(true);
        inventoryCanvas.SetActive(false);
    }

    public void OpenInventory()
    {
        openButtonCanvas.SetActive(false);
        inventoryCanvas.SetActive(true);
    }

    public void CloseInventory()
    {
        openButtonCanvas.SetActive(true);
        inventoryCanvas.SetActive(false);
    }
}