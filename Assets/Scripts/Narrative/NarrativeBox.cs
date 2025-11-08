using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class NarrativeBox : MonoBehaviour
{
    [Header("对话框根节点（整个面板的父物体）")]
    [SerializeField] private GameObject dialogueGroup;

    [Header("立绘（可选）")]
    [SerializeField] private Image portraitImage;

    [Header("名字文本（可选）")]
    [SerializeField] private TextMeshProUGUI nameText;

    [Header("文本引用")]
    [SerializeField] private TextMeshProUGUI narrativeStartText;
    [SerializeField] private TextMeshProUGUI narrativeText;

    [Header("按钮容器（可选）")]
    [SerializeField] private GameObject buttonGroup;

    [Header("打字机参数")]
    [SerializeField] private float textSpeed = 0.02f;

    private DialogueData curDiaData;
    private MonologueData curMonoData;
    private int index = 0;
    private Coroutine typingRoutine;

    private enum Mode
    {
        Start,
        InTalk,
        InShow,
        InAccuse,
        Hidden,
        InMonologue
    }
    private Mode mode = Mode.Hidden;

    private void Reset()
    {
        if (dialogueGroup == null) dialogueGroup = gameObject;
    }

    private void Awake()
    {
        HideAll();
    }

    private void Update()
    {
        switch (mode)
        {
            case Mode.Start:
                EnsureGroupVisible(true);
                SetNarrativeVisible(false);
                SetButtonsVisible(true);
                break;
            case Mode.InTalk:
                SetButtonsVisible(false);
                SetStartVisible(false);
                break;
            case Mode.InShow:
                SetButtonsVisible(false);
                SetStartVisible(false);
                break;
            case Mode.InAccuse:
                SetButtonsVisible(false);
                SetStartVisible(false);
                break;
            case Mode.Hidden:
                StopAllNarrative();
                HideAll();
                break;
            case Mode.InMonologue:
                EnsureGroupVisible(true);
                SetButtonsVisible(false);
                SetStartVisible(false);
                break;
        }
    }
    // -------------------- Public API --------------------
    // show intro from DialogueData ScriptableObject
    public void ShowIntro(DialogueData introData, bool showButtons = true, int useLineIndex = 0)
    {
        
    }


    // show dialogue from DialogueData ScriptableObject
    public void DisplayDialogue(DialogueData data)
    {
        
    }

    public void StartDialogue(DialoguePayload payload)
    {
        mode = Mode.Start;
        if (payload == null || payload.data == null || payload.data.lines == null || payload.data.lines.Length == 0)
        {
            StopAllNarrative();
            return;
        }

        // 立绘
        if (portraitImage)
        {
            if (payload.portrait != null)
            {
                portraitImage.sprite = payload.portrait;
                portraitImage.gameObject.SetActive(true);
            }
            else
            {
                portraitImage.gameObject.SetActive(false);
            }
        }

        // 名字：优先 payload.characterName；否则用首行 speaker；再不然隐藏
        string displayName = payload.characterName;
        SetNameText(string.IsNullOrEmpty(displayName) ? null : displayName);
    }

    // show monologue from MonologueData ScriptableObject
    public void StartMonologue(MonologuePayload payload)
    {
        if (payload == null || payload.data == null || payload.data.lines == null || payload.data.lines.Length == 0)
        {
            StopAllNarrative();
            return;
        }

        if (portraitImage)
        {
            if (payload.portrait != null)
            {
                portraitImage.sprite = payload.portrait;
                portraitImage.gameObject.SetActive(true);
            }
            else
            {
                portraitImage.gameObject.SetActive(false);
            }
        }

        // 名字：优先 payload.characterName；否则用首行 speaker；再不然隐藏
        string displayName = payload.clueName;
        SetNameText(string.IsNullOrEmpty(displayName) ? null : displayName);

        mode = Mode.InTalk;
        DisplayMonologue(payload.data);
    }

    public void DisplayMonologue(MonologueData data)
    {
        

    }

    // stop all narrative
    public void StopAllNarrative()
    {
        StopTypingIfAny();
        HideAll();
    }

    // advance narrative (to be called by Input System)
    public void OnAdvance(InputAction.CallbackContext ctx)
    {
        
    }

    // -------------------- Internals --------------------

    // show/hide helpers
    private void EnsureGroupVisible(bool show)
    {
        if (dialogueGroup) dialogueGroup.SetActive(show);
    }

    // hide all UI elements
    private void HideAll()
    {
        mode = Mode.Hidden;
        EnsureGroupVisible(false);
        SetPortraitVisible(false);
        SetStartText("");
        SetNarrativeText("");
        SetNameText(null);
        SetButtonsVisible(false);
    }

    // UI element setters
    private void SetPortraitVisible(bool show)
    {
        if (portraitImage) portraitImage.gameObject.SetActive(show);
    }

    private void SetNameText(string t)
    {
        if (!nameText) return;
        nameText.text = t ?? "";
        nameText.gameObject.SetActive(!string.IsNullOrEmpty(t));
    }

    private void SetStartText(string t)
    {
        if (narrativeStartText) narrativeStartText.text = t ?? "";
        SetStartVisible(!string.IsNullOrEmpty(t));
    }

    private void SetNarrativeText(string t)
    {
        if (narrativeText) narrativeText.text = t ?? "";
    }

    private void SetStartVisible(bool show)
    {
        if (narrativeStartText) narrativeStartText.gameObject.SetActive(show);
    }

    private void SetNarrativeVisible(bool show)
    {
        if (narrativeText) narrativeText.gameObject.SetActive(show);
    }

    private void SetButtonsVisible(bool show)
    {
        if (buttonGroup) buttonGroup.SetActive(show);
    }

    private void StartTyping(string content)
    {
        StopTypingIfAny();
        typingRoutine = StartCoroutine(TypeRoutine(content));
    }

    private void StopTypingIfAny()
    {
        if (typingRoutine != null)
        {
            StopCoroutine(typingRoutine);
            typingRoutine = null;
        }
    }

    private IEnumerator TypeRoutine(string content)
    {
        SetNarrativeText("");
        foreach (char c in content)
        {
            narrativeText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        typingRoutine = null;
    }

    private void SkipTypingTo(string full)
    {
        StopTypingIfAny();
        SetNarrativeText(full);
    }
}

