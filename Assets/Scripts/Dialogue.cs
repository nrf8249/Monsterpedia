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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textComponent.text = string.Empty;
        //StartDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartDialogue()
    {
        gameObject.SetActive(true);
        index = 0;

        textComponent.text = string.Empty;
        StartCoroutine(TypeLine(lines[index]));
    }
    public void StopDialogue()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    IEnumerator TypeLine(string line)
    {
        yield return null; 

        foreach (char c in line.ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine(lines[index]));
        }
        else
        {
            gameObject.SetActive(false);
            textComponent.text = string.Empty;
            // End of dialogue logic here
        }
    }

    public void UpdateDialogue(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
            }
        }
    }
}
