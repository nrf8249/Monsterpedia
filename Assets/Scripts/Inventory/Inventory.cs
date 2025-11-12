using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public List<InventoryEvidence> inventory;
    public static Inventory instance;
    public GameObject openButtonCanvas;
    public GameObject inventoryCanvas;
    public GameObject grid;
    public GameObject detailedView;
    public GameObject displayImage;
    public GameObject displayTitle;
    public GameObject displayDescription;
    public GameObject showButton;
    private bool showMode;

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
        grid.SetActive(true);
        detailedView.SetActive(false);
    }

    public void CloseInventory()
    {
        openButtonCanvas.SetActive(true);
        inventoryCanvas.SetActive(false);
        grid.SetActive(false);
        detailedView.SetActive(false);
    }

    public void DetailedView(GameObject clue)
    {
        displayTitle.GetComponent<TMPro.TextMeshProUGUI>().text = clue.GetComponent<InventoryEvidence>().evidenceName;
        displayDescription.GetComponent<TMPro.TextMeshProUGUI>().text = clue.GetComponent<InventoryEvidence>().description;
        displayImage.GetComponent<Image>().sprite = clue.GetComponent<InventoryEvidence>().image;
        grid.SetActive(false);
        detailedView.SetActive(true);
        if (showMode)
        {
            showButton.SetActive(true);
        }
        else
        {
            showButton.SetActive(false);
        }
    }

    public void GridView()
    {
        grid.SetActive(true);
        detailedView.SetActive(false);
    }

    public void ShowModeToggle(bool show)
    {
        showMode = show;
    }
}