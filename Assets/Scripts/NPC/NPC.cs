using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NPC : MonoBehaviour, IInteractableTarget
{
    private Vector2 movement;
    private Animator animator;

    [Header("basic data")]
    public DialogueData talkData;
    public DialogueData showData;
    public DialogueData accuseData;
    public Sprite portrait;
    [Tooltip("NPC name")]
    public string npcName;

    [Header("interact hint")]
    public GameObject interactHint;                 
    public Vector3 hintOffset = new Vector3(0, 1.2f, 0);
    public bool faceCamera = true;

    [Header("debug")]
    public bool debugLogs = false;                  
    public Color gizmoColor = new Color(1f, 0.85f, 0f, 0.8f);

    public bool IsPlayerInRange => playerInRange;
    private bool playerInRange = false;
    private Camera cam;
    private Transform hintTransform;
    private Collider2D hintCol;

    private int talkTimes = 1;

    // Start is called before the first frame update
    private void Awake()
    {
        cam = Camera.main;
        animator = GetComponent<Animator>();

        if (interactHint != null)
        {
            hintCol = interactHint.GetComponent<Collider2D>();
            hintTransform = interactHint.transform;
            interactHint.SetActive(false);
        }
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        // update hint position and rotation
        if (interactHint != null && interactHint.activeSelf)
        {
            hintTransform.position = transform.position + hintOffset;
            if (faceCamera && cam) hintTransform.forward = cam.transform.forward;
        }
        // 鼠标点击检测
        if (Mouse.current == null || cam == null || hintCol == null) return;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        // 如果点在 UI 上，直接忽略（防冲突）
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        Vector2 worldPos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        if (hintCol.OverlapPoint(worldPos))
        {
            ClickHint();  // 命中 → 调用
        }
    }

    // player enters the NPC range
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;

        // 不再直接开关自己的 Hint，交给 Manager 统一决定
        if (InteractManager.Instance != null)
            InteractManager.Instance.Register(this);

        if (debugLogs) Debug.Log($"{name} | 玩家进入交互范围");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;

        if (InteractManager.Instance != null)
            InteractManager.Instance.Unregister(this);

        // 保险起见自己也关一下
        if (interactHint) interactHint.SetActive(false);

        if (NarrativeBoxManager.Instance != null)
            NarrativeBoxManager.Instance.StopDialogue();

        if (debugLogs) Debug.Log($"{name} | 玩家离开交互范围，隐藏提示 + 关闭对话框");
    }


    // interact input action
    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!playerInRange) return;

        // 关键：只允许当前最近的那个响应
        if (InteractManager.Instance != null &&
            !InteractManager.Instance.IsCurrent(this))
            return;

        StartDialogue();
    }

    // click interact hint
    public void ClickHint()
    {
        if (!playerInRange) return;

        if (InteractManager.Instance != null &&
            !InteractManager.Instance.IsCurrent(this))
            return;

        var gm = GameManager.Instance;
        if (!NarrativeBoxManager.Instance.CanStartNarrative) return;

        if (debugLogs) Debug.Log($"{name} 被点击 → 开始对话");
        StartDialogue();
    }

    // start dialogue with this NPC
    private void StartDialogue()
    {
        // debugging checks
        if (talkData == null)
        {
            if (debugLogs) Debug.LogWarning($"{name} | talkData 为空，无法开始对话");
            return;
        }
        if (NarrativeBoxManager.Instance == null)
        {
            Debug.LogError($"{name} | DialogueManager.Instance 为空（场景里没挂或被禁用）");
            return;
        }

        // start dialogue
        var payload = new DialoguePayload(
            data: talkData,
            portrait: portrait,
            characterName: string.IsNullOrEmpty(npcName) ? null : npcName, 
            talkTimes: talkTimes
        );
        if (NarrativeBoxManager.Instance.IsNarrating)
            return;
        NarrativeBoxManager.Instance.StartDialogue(payload);
        talkTimes++;
    }

    // Gizmos: notice the interact hint position in Editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;
        Vector3 p = transform.position + hintOffset;
        Gizmos.DrawWireSphere(p, 0.1f);
        Gizmos.DrawLine(transform.position, p);
    }

    public void SetHintVisible(bool visible)
    {
        if (interactHint == null) return;
        interactHint.SetActive(visible);
    }
}

