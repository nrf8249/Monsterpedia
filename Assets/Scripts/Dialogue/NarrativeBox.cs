using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// NarrativeBox��ͳһ�������׶Ի�UI�Ľű���
/// ����ֻ��ע�����ı���
/// 1) narrativeStartText����NPC������ġ�����/��ʾ���ı�����ʾ�ڰ�ť�Ϸ���
/// 2) narrativeText��������ʼ�Ի��󣨻������������Ķ��ף���ʾ������
///
/// ˵����
/// - ������Ҫ��˵����������塱����˲������κΡ�����/ͷ�����á�
/// - ��ʱ���� Show / Accuse ������ť��ֻ������ѡ�İ�ť�鸸���壨���ڷ� Talk ��ť�������еİ�ť��������
/// - �ṩ������Ҫ��ڣ�
///     ShowIntro(...)     ���� ���롰������ʾ���׶Σ�����ʾ narrativeStartText���Ϳ�ѡ��ť�飩
///     StartDialogue(...) ���� ���롰��ʽ�Ի����׶Σ����ִ�ӡ narrativeText
///     StartMonologue(...)���� ���롰����/������ʾ���׶Σ����ִ�ӡ narrativeText��������ť�飩
/// - �� OnAdvance(...) �����룬ʵ�֡���һ��/�����
/// </summary>
public class NarrativeBox : MonoBehaviour
{
    [Header("�Ի�����ڵ㣨�������ĸ����壩")]
    [SerializeField] private GameObject dialogueGroup;     // �����Ի���UI�ĸ������硰DialogueGroup����
    [SerializeField] private GameObject portrait;

    [Header("�ı�����")]
    [SerializeField] private TextMeshProUGUI narrativeStartText; // ��ʼʱ����ť�Ϸ�������ʾ�ı�
    [SerializeField] private TextMeshProUGUI narrativeText;      // ��ʽ�Ի�/��������

    [Header("��ť��������ѡ��")]
    [SerializeField] private GameObject buttonGroup;       // ��������ʾ/�������鰴ť��������Ծ��� Show/Accuse��

    [Header("���ֻ�����")]
    [SerializeField] private float textSpeed = 0.02f;      // ���������ٶ�

    // ���� ���ݻ��� ���� //
    private DialogueData curData;          // ��ǰ���ŵ� DialogueData���Ի�����ף�
    private string[] legacyLines;          // ���ݾɵ� string[] ��ڣ��ɵ������ף�
    private bool usingSO = false;          // �Ƿ�ʹ�� DialogueData
    private int index = 0;                 // ���ڲ��ŵ��к�
    private Coroutine typingRoutine;       // ����Э��

    // ���� ��״̬�� ���� //
    private enum Mode { Hidden, Intro, Narrative }
    private Mode mode = Mode.Hidden;

    private void Reset()
    {
        // �ѽű��ϵ� DialogueGroup ��ʱ���Զ�������
        if (dialogueGroup == null) dialogueGroup = gameObject;
    }

    private void Awake()
    {
        HideAll();
    }

    // ========================== ���� API ===========================

    /// <summary>
    /// ��ʾ������/��ʾ���ı�����������NPC�նԻ�ʱ����ť�Ϸ���һ����ʾ����
    /// - ����ʾ narrativeStartText����ѡ��ʾ��ť�飩
    /// - ���������־��飬����Ӧ OnAdvance��
    /// </summary>
    /// <param name="introText">Ҫ��ʾ�Ŀ����ַ���</param>
    /// <param name="showButtons">�Ƿ���ʾ��ť����������ͨ��ֻ��Talk��ť��</param>
    public void ShowIntro(string introText, bool showButtons = true)
    {
        StopTypingIfAny();

        mode = Mode.Intro;
        EnsureGroupVisible(true);

        // �������ɼ�����������ղ�����
        SetStartText(introText);
        SetNarrativeText("");
        SetNarrativeVisible(false);

        SetButtonsVisible(showButtons);
    }

    /// <summary>
    /// ��ʾ������/��ʾ���ı����� DialogueData��Ĭ��ȡ��1�����ݣ���
    /// ���������ʾ����Ҳ���� SO��
    /// </summary>
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

    /// <summary>
    /// ��ʼ����ʽ�Ի��������ִ�ӡ narrativeText����
    /// - ���ؿ���������ʾ��������
    /// - ֧�ְ������/��һ�䣨OnAdvance��
    /// </summary>
    public void StartDialogue(DialogueData data)
    {
        if (data == null || data.lines == null || data.lines.Length == 0)
        {
            // ��������ֱ������
            StopAllNarrative();
            return;
        }

        StopTypingIfAny();
        usingSO = true;
        curData = data;
        index = 0;

        mode = Mode.Narrative;
        EnsureGroupVisible(true);

        // ���ؿ�������ʾ����
        SetStartVisible(false);
        SetNarrativeVisible(true);
        SetButtonsVisible(false); // ��ҪҲ���Ըĳ� true

        // ��ʼ����
        StartTyping(curData.lines[index].content);
    }

    /// <summary>
    /// ��ʼ������/������ʾ�������ִ�ӡ narrativeText���ް�ť����
    /// - ���������������������������
    /// </summary>
    public void StartMonologue(DialogueData data)
    {
        StartDialogue(data);
        SetButtonsVisible(false); // ����һ�㲻��ʾ��ť
    }

    /// <summary>
    /// ���ݾɰ棺�� string[] ��������Ϊ�����ס���
    /// </summary>
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

        StartTyping(legacyLines[index]);
    }

    /// <summary>
    /// ֹͣ������ȫ���Ի�UI��
    /// </summary>
    public void StopAllNarrative()
    {
        StopTypingIfAny();
        HideAll();
    }

    /// <summary>
    /// ����ص����ƽ��Ի������/��һ�䣩�����ڡ���ʽ�Ի�/���ס�ģʽ����Ч��
    /// </summary>
    public void OnAdvance(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (mode != Mode.Narrative) return;              // Intro �׶β���Ӧ
        if (!dialogueGroup || !dialogueGroup.activeSelf) return;

        if (usingSO)
        {
            var full = curData.lines[index].content;
            if (narrativeText.text == full)
            {
                // ��һ��
                if (index < curData.lines.Length - 1)
                {
                    index++;
                    StartTyping(curData.lines[index].content);
                }
                else
                {
                    StopAllNarrative(); // ����
                }
            }
            else
            {
                // ���������
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

    // ========================== �ڲ����� ===========================

    private void EnsureGroupVisible(bool show)
    {
        if (dialogueGroup) dialogueGroup.SetActive(show);
    }

    private void HideAll()
    {
        mode = Mode.Hidden;
        EnsureGroupVisible(false);
        SetStartText("");
        SetNarrativeText("");
        SetButtonsVisible(false);
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
