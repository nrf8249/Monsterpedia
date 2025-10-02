using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public Dialogue dialogueBox;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        dialogueBox = GameObject.Find("DialogueBox").GetComponent<Dialogue>();
        dialogueBox.gameObject.SetActive(false);
    }

    public void StartDialogue(string[] lines)
    {
        dialogueBox.lines = lines;
        dialogueBox.StartDialogue();
        Debug.Log("Starting dialogue...");
    }
}
