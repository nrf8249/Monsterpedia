using Unity.VisualScripting;
using UnityEngine;

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

    public void StartDialogue()
    {
        dialogueBox.gameObject.SetActive(true);
        Debug.Log("Starting dialogue...");
    }
}
