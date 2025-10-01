using UnityEngine;
using UnityEngine.InputSystem;

public class NPC : MonoBehaviour
{
    public string[] dialogueLines; // Assign in Inspector
    private bool playerInRange = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Player entered NPC range. Show prompt UI here!");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("Player left NPC range. Hide prompt UI.");
        }
    }

    // This method will be called from Player Input → Interact event
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed && playerInRange)
        {
            //DialogueManager.Instance.StartDialogue(dialogueLines);
        }
    }
}
