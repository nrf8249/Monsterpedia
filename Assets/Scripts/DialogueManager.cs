using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance; // Singleton (easy global access)

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartDialogue(string[] lines)
    {
        Debug.Log("Starting dialogue...");
        foreach (string line in lines)
        {
            Debug.Log(line);
        }
        // TODO: Replace with actual UI dialogue display
    }
}
