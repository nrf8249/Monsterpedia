using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<InventoryEvidence> inventory;
    public static Inventory instance;
    public Canvas openButtonCanvas;
    private CanvasGroup buttonGroup;
    public Canvas inventoryCanvas;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        inventory = new List<InventoryEvidence>();
        canvasGroup = inventoryCanvas.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        buttonGroup = openButtonCanvas.GetComponent<CanvasGroup>();
        openButtonCanvas.sortingOrder = 2;
        inventoryCanvas.sortingOrder = 1;
    }

    public void OpenInventory()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        buttonGroup.alpha = 0;
        buttonGroup.interactable = false;
        openButtonCanvas.sortingOrder = 1;
        inventoryCanvas.sortingOrder = 2;
    }

    public void CloseInventory()
    {
        buttonGroup.alpha = 1;
        buttonGroup.interactable = true;
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        openButtonCanvas.sortingOrder = 2;
        inventoryCanvas.sortingOrder = 1;
    }
}