using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class NPC : MonoBehaviour
{
    public string[] dialogueLines;
    private bool playerInRange = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("player entered NPC range, show UI");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("player left NPC range, hide UI");
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started && playerInRange)
        {
            Debug.Log("interacted with NPC");
            DialogueManager.Instance.StartDialogue(dialogueLines);
        }
    }
}
