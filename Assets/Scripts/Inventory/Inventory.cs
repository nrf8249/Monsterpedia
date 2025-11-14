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
    private string clueName;
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
        //NarrativeBoxManager.Instance.DisplayShowDialogue(clueName); //uncomment once we get real clue keys in
        showButton.SetActive(false);
        showMode = false;
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
}