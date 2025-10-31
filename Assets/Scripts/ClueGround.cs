using System;
using System.Dynamic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClueGround : MonoBehaviour
{
    public string[] dialogueLines;
    private bool playerInRange = false;
    public Inventory inventory;
    public string evidenceName;
    public string description;
    public Sprite image;
    public GameObject invClueTEMP;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("player entered item range, show UI");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("player left item range, hide UI");
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started && playerInRange)
        {
            Debug.Log("interacted with item");
            //DialogueManager.Instance.StartDialogue(dialogueLines);
            invClueTEMP.SetActive(true);
        }
    }
}
