using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [Header("All the UI group")]
    [SerializeField] private GameObject clueBookPanel;

    [Header("Buttons")]
    [SerializeField] private GameObject clueBookIcon;
    [SerializeField] private GameObject showButton;
    [SerializeField] private GameObject accuseButton;

    [Header("Components")]
    [SerializeField] private GameObject header;
    [SerializeField] private GameObject clueGrid;
    [SerializeField] private GameObject clueDetailView;

    [Header("Image")]
    [SerializeField] private GameObject displayImage;
    [SerializeField] private Sprite commonClueBook;
    [SerializeField] private Sprite updatedClueBook;


    [Header("Text")]
    [SerializeField] private string clueName;
    [SerializeField] private GameObject clueDetailTitle;
    [SerializeField] private GameObject clueDescription;

    private List<GameObject> gridItems;


    // -------------------- Internal State --------------------
    private Mode mode;
    private State state;
    private GameObject currentClue;
    private enum Mode
    {
        InCheck,
        InDetail,
        Hidden,
    }

    private enum State
    {
        Common,
        Show,
        Accuse
    }

    private void Awake()
    {
        mode = Mode.Hidden;
        ApplyMode();
        state = State.Common;
        ApplyState();
    }

    // apply current mode
    private void ApplyMode()
    {
        switch (mode)
        {
            case Mode.InCheck:
                clueBookPanel.SetActive(true);
                clueBookIcon.SetActive(false);
                header.SetActive(true);
                clueGrid.SetActive(true);
                clueDetailView.SetActive(false);
                break;
            case Mode.InDetail:
                clueBookPanel.SetActive(true);
                clueBookIcon.SetActive(false);
                clueGrid.SetActive(false);
                clueDetailView.SetActive(true);
                break;
            case Mode.Hidden:
                clueBookIcon.SetActive(true);
                clueBookPanel.SetActive(false);
                header.SetActive(false);
                clueGrid.SetActive(false);
                clueDetailView.SetActive(false);
                break;
        }
    }

    private void ApplyState()
    {
        switch (state)
        {
            case State.Common:
                showButton.SetActive(false);
                accuseButton.SetActive(false);
                break;
            case State.Show:
                showButton.SetActive(true);
                accuseButton.SetActive(false);
                break;
            case State.Accuse:
                showButton.SetActive(false);
                accuseButton.SetActive(true);
                break;
        }
    }

    void Start()
    {
        gridItems = new List<GameObject>();

        foreach(Transform child in clueGrid.transform)
        {
            gridItems.Add(child.gameObject);
        }
        for(int i = 0; i < gridItems.Count; i++)
        {
            gridItems[i].SetActive(false);
        }
    }

    public void LoadDetailedView(GameObject clue)
    {
        clueName = clue.GetComponent<InventoryEvidence>().evidenceName;
        clueDetailTitle.GetComponent<TMPro.TextMeshProUGUI>().text = clueName;
        clueDescription.GetComponent<TMPro.TextMeshProUGUI>().text = clue.GetComponent<InventoryEvidence>().description;
        displayImage.GetComponent<Image>().sprite = clue.GetComponent<InventoryEvidence>().image;
        InDetail();
    }

    public void SendShowInfo()
    {
        NarrativeBoxManager.Instance.DisplayShowDialogue(clueName);
        InHidden();
    }

    public void SendAccuseInfo()
    {
        NarrativeBoxManager.Instance.DisplayAccuseDialogue(clueName);
        InHidden();
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

    public bool HasClue(string clueName)
    {
        for(int i = 0; i < gridItems.Count; i++)
        {
            if (gridItems[i].GetComponentInChildren<InventoryEvidence>().evidenceName == clueName && gridItems[i].activeSelf)
            {
                return true;
            }
        }
        return false;
    }

    public void OpenClueBook()
    {
        mode = Mode.InCheck;
        state = State.Common;
        ApplyMode();
        ApplyState();
        GetNewClue(false);
    }

    public void InCheck()
    {
        mode = Mode.InCheck;
        ApplyMode();
    }

    public void InDetail()
    {
        mode = Mode.InDetail;
        ApplyMode();
    }

    public void InCommon()
    {
        state = State.Common;
        ApplyState();
    }

    public void InShow()
    {
        state = State.Show;
        ApplyState();
    }

    public void InAccuse()
    {
        state = State.Accuse;
        ApplyState();
    }

    public void InHidden()
    {
        mode = Mode.Hidden;
        ApplyMode();
    }

    public bool IsHasAnyClue()
    {
        for (int i = 0; i < gridItems.Count; i++)
        {
            if (gridItems[i].activeSelf)
            {
                return true;
            }
        }
        return false;
    }

    public void GetNewClue(bool isUpdated)
    {
        if (isUpdated)
        {
            clueBookIcon.GetComponent<Image>().sprite = updatedClueBook;
        }
        else
        {
            clueBookIcon.GetComponent<Image>().sprite = commonClueBook;
        }
    }


    // -------------------- Internals --------------------
}