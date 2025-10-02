using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;

    private int index;
    private bool isTyping;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textComponent.text = string.Empty;
    }

    public void StartDialogue()
    {
        gameObject.SetActive(true);
        index = 0;

        textComponent.text = string.Empty;
        StartCoroutine(TypeLine(lines[index]));
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        textComponent.text = string.Empty;

        yield return null; // Wait one frame to avoid skipping the first character

        foreach (char c in line.ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        isTyping = false;
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            StartCoroutine(TypeLine(lines[index]));
        }
        else
        {
            gameObject.SetActive(false);
            textComponent.text = string.Empty;
        }
    }

    public void UpdateDialogue(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        
        if (isTyping)
        {
            StopAllCoroutines();
            textComponent.text = lines[index];
            isTyping = false;
        }
        else
        {
            NextLine();
        }
        
    }
}
