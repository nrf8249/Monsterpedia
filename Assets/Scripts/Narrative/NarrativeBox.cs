using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using static DialogueData;

public class NarrativeBox : MonoBehaviour
{
    [Header("All the UI group")]
    [SerializeField] private GameObject dialogueGroup;

    [Header("Portrait")]
    [SerializeField] private Image portraitImage;

    [Header("Name text")]
    [SerializeField] private TextMeshProUGUI nameText;

    [Header("Intro text & Narrative text")]
    [SerializeField] private TextMeshProUGUI narrativeStartText;
    [SerializeField] private TextMeshProUGUI narrativeText;

    [Header("Buttons")]
    [SerializeField] private GameObject buttonGroup;

    [Header("Speed of type")]
    [SerializeField] private float textSpeed = 0.02f;

    [Header("Input")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private string playerMap = "Player";// default player map
    [SerializeField] private string uiMap = "UI";        // default ui map

    [SerializeField] private float reopenCooldown = 0.15f;
    private float _lastClosedAt = -999f;
    private bool _requireRelease = false;

    // can start narrative check
    public bool CanStartNarrative
    {
        get
        {
            // if is narrative active
            if (hasActiveSeq) return false;
            // cooldown check
            if (Time.unscaledTime - _lastClosedAt < reopenCooldown) return false;
            var kb = Keyboard.current;
            // require release check
            if (_requireRelease && kb != null && kb.eKey.isPressed) return false;
            return true;
        }
    }

    // -------------------- Internal State --------------------
    private DialogueData curDiaData;
    private MonologueData curMonoData;
    private int index = 0;
    private Coroutine typingRoutine;
    private Mode mode;
    private enum Mode
    {
        Start,
        InTalk,
        InShow,
        InAccuse,
        Hidden,
        InMonologue
    }

    // text sequence struct
    private struct TextSequence
    {
        public int Count;                          // total number of entries
        public System.Func<int, string> GetContent;// function to get content by index
        public System.Action OnEnd;                // action to perform on end
    }
    private TextSequence activeSeq;
    public bool hasActiveSeq = false;

    // -------------------- Unity Callbacks --------------------
    private void Reset()
    {
        if (dialogueGroup == null) dialogueGroup = gameObject;
    }
    private void Awake()
    {
        mode = Mode.Hidden;
        ApplyMode();
    }

    // apply current mode
    private void ApplyMode()
    {
        switch (mode)
        {
            case Mode.Start:
                Debug.Log("Mode: Start");
                EnsureGroupVisible(true);
                SetStartVisible(true);      
                SetNarrativeVisible(false);
                SetButtonsVisible(true);
                break;
            case Mode.InTalk:
                SetButtonsVisible(false);
                SetStartVisible(false);
                SetNarrativeVisible(true);
                DisplayTalkDialogue(curDiaData);
                break;
            case Mode.InShow:
                SetButtonsVisible(false);
                SetStartVisible(false);
                Inventory.instance.OpenInventory();
                break;
            case Mode.InAccuse:
                SetButtonsVisible(false);
                SetStartVisible(false);
                break;
            case Mode.Hidden:
                hasActiveSeq = false;
                index = 0;
                curDiaData = null;
                curMonoData = null;
                StopTypingIfAny();
                HideAll();
                SwitchToPlayer();
                break;
            case Mode.InMonologue:
                ShowAll();
                SetButtonsVisible(false);
                SetStartVisible(false);
                DisplayMonologue(curMonoData);
                break;
        }
    }

    // -------------------- Public API --------------------
    // —— Monologue ——
    public void StartMonologue(MonologuePayload payload)
    {
        SwitchToUI();
        // null check
        if (payload == null || payload.data == null || payload.data.lines == null || payload.data.lines.Length == 0)
        {
            StopAllNarrative();
            return;
        }

        // Protrait
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

        // Name
        string displayName = payload.clueName;
        SetNameText(string.IsNullOrEmpty(displayName) ? null : displayName);

        // Data
        curMonoData = payload.data;
        mode = Mode.InMonologue;
        ApplyMode();
    }
    public void DisplayMonologue(MonologueData data)
    {
        if (data == null || data.lines == null || data.lines.Length == 0)
        { StopAllNarrative(); return; }

        StartSequence(new TextSequence
        {
            Count = data.lines.Length,
            GetContent = i => data.lines[i].content ?? "",
            OnEnd = () => { StopAllNarrative(); } // ← 独白的结束行为
        });
    }

    // —— Dialogue ——
    public void StartDialogue(DialoguePayload payload)
    {
        SwitchToUI();
        // null check
        if (payload == null || payload.data == null || payload.data.narrativeComponents == null || payload.data.narrativeComponents.Length == 0)
        {
            StopAllNarrative();
            return;
        }

        // portrait
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

        // name
        string displayName = payload.characterName;
        SetNameText(string.IsNullOrEmpty(displayName) ? null : displayName);

        // data
        curDiaData = payload.data;
        NarrativeComponent narrative = null;
        for (int i = 0; i < curDiaData.narrativeComponents.Length; i++)
        {
            if (curDiaData.narrativeComponents[i].narrativeType == DialogueData.NarrativeType.Start)
            {
                narrative = curDiaData.narrativeComponents[i];
                break;
            }
        }
        if (narrative != null && narrative.lines.Length > 0)
        {
            SetStartText(narrative.lines[0].content ?? "");
        }
        else
        {
            SetStartText("");
        }
        mode = Mode.Start;
        ApplyMode();
    }
    public void InTalk()
    {
        mode = Mode.InTalk;
        ApplyMode();
    }
    public void DisplayTalkDialogue(DialogueData data)
    {
        if (data == null || data.narrativeComponents == null || data.narrativeComponents.Length == 0)
        {
            StopAllNarrative();
            return;
        }
        NarrativeComponent comp = null;
        for (int i = 0; i < data.narrativeComponents.Length; i++)
        {
            if (data.narrativeComponents[i].narrativeType == DialogueData.NarrativeType.Talk)
            {
                comp = data.narrativeComponents[i];
                break;
            }
        }
        StartSequence(new TextSequence
        {
            Count = comp.lines.Length,
            // 假设 DialogueData 的行结构也有 content 字段；若有 speaker/portrait，可在 ApplyPerLine 设置
            GetContent = i => comp.lines[i].content ?? "",
            OnEnd = () => { BackToStart(); }
        });
    }
    public void InShow()
    {
        mode = Mode.InShow;
        ApplyMode();
    }
    public void DisplayShowDialogue(string clueKey)
    {
        if (curDiaData == null || curDiaData.narrativeComponents == null || curDiaData.narrativeComponents.Length == 0)
        {
            StopAllNarrative();
            return;
        }
        NarrativeComponent comp = null;
        for (int i = 0; i < curDiaData.narrativeComponents.Length; i++)
        {
            if (curDiaData.narrativeComponents[i].narrativeType == DialogueData.NarrativeType.Clue &&
                curDiaData.narrativeComponents[i].key == clueKey)
            {
                comp = curDiaData.narrativeComponents[i];
                break;
            }
        }
        StartSequence(new TextSequence
        {
            Count = comp.lines.Length,
            GetContent = i => comp.lines[i].content ?? "",
            OnEnd = () => { BackToStart(); }
        });
    }
    public void InAccuse()
    {
        mode = Mode.InAccuse;
        ApplyMode();
    }
    public void BackToStart()
    {
        mode = Mode.Start;
        ApplyMode();
    }

    // stop all narrative
    public void StopAllNarrative()
    {
        mode = Mode.Hidden;
        ApplyMode();
    }

    // exit narrative (to be called by Input System)
    public void OnExit(InputAction.CallbackContext ctx)
    {
        mode = Mode.Hidden;
        ApplyMode();
    }

    // advance narrative (to be called by Input System)
    public void OnAdvance(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (hasActiveSeq)
        {
            AdvanceNext();
            return;
        }
    }

    // -------------------- Internals --------------------
    // show/hide helpers
    private void EnsureGroupVisible(bool show)
    {
        if (dialogueGroup && dialogueGroup != gameObject)
            dialogueGroup.SetActive(show);
    }
    private void ShowAll()
    {
        EnsureGroupVisible(true);
        SetPortraitVisible(true);
        SetButtonsVisible(true);
        SetNarrativeVisible(true);
    }
    private void HideAll()
    {
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

    // typing effect
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

    // text sequence management
    private void StartSequence(TextSequence seq)
    {
        if (seq.Count <= 0 || seq.GetContent == null)
        {
            StopAllNarrative();
            return;
        }
        activeSeq = seq;
        hasActiveSeq = true;
        index = 0;

        EnsureGroupVisible(true);
        SetStartVisible(false);
        SetNarrativeVisible(true);
        SetButtonsVisible(false);

        StartTyping(activeSeq.GetContent(index));
    }
    private void AdvanceNext()
    {
        if (!hasActiveSeq) return;

        if (typingRoutine != null)
        {
            SkipTypingTo(activeSeq.GetContent(index));
            return;
        }

        index++;
        if (index >= activeSeq.Count)
        {
            hasActiveSeq = false;
            // 关键：不同入口给的 OnEnd 不同 → 这里自动做对的事
            if (activeSeq.OnEnd != null) activeSeq.OnEnd.Invoke();
            else StopAllNarrative(); // 兜底
            return;
        }

        StartTyping(activeSeq.GetContent(index));
    }

    // input map switching
    private void SwitchToUI()
    {
        if (playerInput && playerInput.currentActionMap?.name != uiMap)
            playerInput.SwitchCurrentActionMap(uiMap);
    }
    private void SwitchToPlayer()
    {
        if (playerInput && playerInput.currentActionMap?.name != playerMap)
            playerInput.SwitchCurrentActionMap(playerMap);
    }


}

