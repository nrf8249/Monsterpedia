using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class NarrativeBox : MonoBehaviour
{
    [Header("�Ի�����ڵ㣨�������ĸ����壩")]
    [SerializeField] private GameObject dialogueGroup;

    [Header("���棨��ѡ��")]
    [SerializeField] private Image portraitImage;

    [Header("�����ı�����ѡ��")]
    [SerializeField] private TextMeshProUGUI nameText;     // �� ������NPC ������ʾ����Ϊ�գ�

    [Header("�ı�����")]
    [SerializeField] private TextMeshProUGUI narrativeStartText;
    [SerializeField] private TextMeshProUGUI narrativeText;

    [Header("��ť��������ѡ��")]
    [SerializeField] private GameObject buttonGroup;

    [Header("���ֻ�����")]
    [SerializeField] private float textSpeed = 0.02f;

    private DialogueData curData;
    private string[] legacyLines;
    private bool usingSO = false;
    private int index = 0;
    private Coroutine typingRoutine;

    private enum Mode { Hidden, Intro, Narrative }
    private Mode mode = Mode.Hidden;

    private void Reset()
    {
        if (dialogueGroup == null) dialogueGroup = gameObject;
    }

    private void Awake()
    {
        HideAll();
    }

    // -------------------- Public API --------------------

    public void ShowIntro(string introText, bool showButtons = true)
    {
        StopTypingIfAny();

        mode = Mode.Intro;
        EnsureGroupVisible(true);

        SetPortraitVisible(portraitImage != null && portraitImage.sprite != null);
        // ���֣�������һ�����õĿɼ�״̬����ǿ�ƸĶ�
        SetStartText(introText);
        SetNarrativeText("");
        SetNarrativeVisible(false);

        SetButtonsVisible(showButtons);
    }

    public void ShowIntro(DialogueData introData, bool showButtons = true, int useLineIndex = 0)
    {
        if (introData == null || introData.lines == null || introData.lines.Length == 0)
        {
            ShowIntro("", showButtons);
            return;
        }
        var line = Mathf.Clamp(useLineIndex, 0, introData.lines.Length - 1);
        ShowIntro(introData.lines[line].content, showButtons);
    }

    public void StartDialogue(DialogueData data)
    {
        if (data == null || data.lines == null || data.lines.Length == 0)
        {
            StopAllNarrative();
            return;
        }

        StopTypingIfAny();
        usingSO = true;
        curData = data;
        index = 0;

        mode = Mode.Narrative;
        EnsureGroupVisible(true);

        SetStartVisible(false);
        SetNarrativeVisible(true);
        SetButtonsVisible(false);

        // ��֮ǰûͨ�� payload �������֣������������ speaker ����
        if (nameText && string.IsNullOrEmpty(nameText.text))
        {
            var firstSpeaker = curData.lines[0].speaker;
            SetNameText(string.IsNullOrEmpty(firstSpeaker) ? null : firstSpeaker);
        }

        StartTyping(curData.lines[index].content);
    }

    public void StartDialogue(NarrativePayload payload)
    {
        if (payload == null || payload.data == null || payload.data.lines == null || payload.data.lines.Length == 0)
        {
            StopAllNarrative();
            return;
        }

        // ����
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

        // ���֣����� payload.characterName������������ speaker���ٲ�Ȼ����
        string displayName = !string.IsNullOrEmpty(payload.characterName)
            ? payload.characterName
            : (payload.data.lines.Length > 0 ? payload.data.lines[0].speaker : null);
        SetNameText(string.IsNullOrEmpty(displayName) ? null : displayName);

        // �����Ի�����
        StartDialogue(payload.data);

        if (payload.asMonologue) SetButtonsVisible(false);
    }

    public void StartMonologue(DialogueData data)
    {
        StartDialogue(data);
        SetButtonsVisible(false);
    }

    public void StartMonologue(string[] lines)
    {
        if (lines == null || lines.Length == 0)
        {
            StopAllNarrative();
            return;
        }

        StopTypingIfAny();
        usingSO = false;
        legacyLines = lines;
        index = 0;

        mode = Mode.Narrative;
        EnsureGroupVisible(true);

        SetStartVisible(false);
        SetNarrativeVisible(true);
        SetButtonsVisible(false);

        // ���ײ���ʾ����
        SetNameText(null);

        StartTyping(legacyLines[index]);
    }

    public void StopAllNarrative()
    {
        StopTypingIfAny();
        HideAll();
    }

    public void OnAdvance(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (mode != Mode.Narrative) return;
        if (!dialogueGroup || !dialogueGroup.activeSelf) return;

        if (usingSO)
        {
            var full = curData.lines[index].content;
            if (narrativeText.text == full)
            {
                if (index < curData.lines.Length - 1)
                {
                    index++;
                    StartTyping(curData.lines[index].content);
                }
                else
                {
                    StopAllNarrative();
                }
            }
            else
            {
                SkipTypingTo(full);
            }
        }
        else
        {
            var full = legacyLines[index];
            if (narrativeText.text == full)
            {
                if (index < legacyLines.Length - 1)
                {
                    index++;
                    StartTyping(legacyLines[index]);
                }
                else
                {
                    StopAllNarrative();
                }
            }
            else
            {
                SkipTypingTo(full);
            }
        }
    }

    // -------------------- Internals --------------------

    private void EnsureGroupVisible(bool show)
    {
        if (dialogueGroup) dialogueGroup.SetActive(show);
    }

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

