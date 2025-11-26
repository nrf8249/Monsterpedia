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
    private List<GameObject> gridItems;
    public static Inventory instance;
    public GameObject openButtonCanvas;
    public GameObject inventoryCanvas;
    public GameObject grid;
    public GameObject detailedView;
    public GameObject displayImage;
    public string clueName;
    public GameObject displayTitle;
    public GameObject displayDescription;
    public GameObject showButton;
    private bool showMode;
    public GameObject accuseButton;
    private bool accuseMode;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        inventory = new List<InventoryEvidence>();
        openButtonCanvas.SetActive(true);
        inventoryCanvas.SetActive(false);
        gridItems = new List<GameObject>();
        foreach(Transform child in grid.transform)
        {
            gridItems.Add(child.gameObject);
        }
        for(int i = 0; i < gridItems.Count; i++)
        {
            gridItems[i].SetActive(false);
        }
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
        clueName = clue.GetComponent<InventoryEvidence>().evidenceName;
        displayTitle.GetComponent<TMPro.TextMeshProUGUI>().text = clueName;
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
        if(accuseMode)
        {
            accuseButton.SetActive(true);
        }
        else
        {
            accuseButton.SetActive(false);
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

    public void SendShowInfo()
    {
        NarrativeBoxManager.Instance.DisplayShowDialogue(clueName);
        showButton.SetActive(false);
        showMode = false;
        CloseInventory();
    }

    public void AccuseModeToggle(bool accuse)
    {
        accuseMode = accuse;
    }

    public void SendAccuseInfo()
    {
        NarrativeBoxManager.Instance.DisplayAccuseDialogue(clueName);
        accuseButton.SetActive(false);
        accuseMode = false;
        CloseInventory();
    }

    public void GetClue(string clueName)
    {
        for(int i = 0; i < gridItems.Count; i++)
        {
            Debug.Log(gridItems[i].GetComponentInChildren<InventoryEvidence>().evidenceName);
            Debug.Log(clueName);
            if (gridItems[i].GetComponentInChildren<InventoryEvidence>().evidenceName == clueName)
            {
                gridItems[i].SetActive(true);
            }
        }
    }

    public void GetAllClues()
    {
        for(int i = 0; i < gridItems.Count; i++)
        {
            gridItems[i].SetActive(true);
        }
    }
}