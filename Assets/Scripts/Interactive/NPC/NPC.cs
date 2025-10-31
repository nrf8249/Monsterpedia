using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    [Header("基础信息")]
    public DialogueData talkData;
    public Sprite portrait;
    [Tooltip("NPC 名字（显示在对话框上）")]
    public string npcName;

    [Header("交互提示（挂在 NPC 身上）")]
    public GameObject interactHint;                 // 图标/气泡
    public Vector3 hintOffset = new Vector3(0, 1.2f, 0);
    public bool faceCamera = true;

    [Header("调试")]
    public bool debugLogs = false;                  // 需要时打开
    public Color gizmoColor = new Color(1f, 0.85f, 0f, 0.8f);

    private bool playerInRange = false;
    private Camera mainCam;
    private Transform hintTransform;

    private void Awake()
    {
        mainCam = Camera.main;

        if (interactHint != null)
        {
            hintTransform = interactHint.transform;
            interactHint.SetActive(false);
        }
    }

    private void Update()
    {
        // 在范围内，直接监听 E 键（不再依赖外部把 Action 路由到 NPC）
        if (playerInRange && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            TryStartDialogue();
        }
    }

    private void LateUpdate()
    {
        // 头顶跟随 + 面向相机
        if (interactHint != null && interactHint.activeSelf)
        {
            hintTransform.position = transform.position + hintOffset;
            if (faceCamera && mainCam) hintTransform.forward = mainCam.transform.forward;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
        if (interactHint) interactHint.SetActive(true);
        if (debugLogs) Debug.Log($"{name} | 玩家进入交互范围");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
        if (interactHint) interactHint.SetActive(false);
        if (DialogueManager.Instance != null) DialogueManager.Instance.StopDialogue();
        if (debugLogs) Debug.Log($"{name} | 玩家离开交互范围，隐藏提示 + 关闭对话框");
    }

    // 仍保留：若你后面走“Player路由到NPC”的方案B，这个回调仍然可用
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!(context.started || context.performed)) return;
        if (!playerInRange) return;
        TryStartDialogue();
    }

    private void TryStartDialogue()
    {
        if (talkData == null)
        {
            if (debugLogs) Debug.LogWarning($"{name} | talkData 为空，无法开始对话");
            return;
        }
        if (DialogueManager.Instance == null)
        {
            Debug.LogError($"{name} | DialogueManager.Instance 为空（场景里没挂或被禁用）");
            return;
        }

        var payload = new NarrativePayload(
            data: talkData,
            portrait: portrait,
            characterName: string.IsNullOrEmpty(npcName) ? null : npcName,
            asMonologue: false
        );

        DialogueManager.Instance.StartDialogue(payload);
        if (debugLogs) Debug.Log($"{name} | 已开始对话（行数={talkData.lines?.Length ?? 0}）");
    }

    // 在场景视图里可见提示位置
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;
        Vector3 p = transform.position + hintOffset;
        Gizmos.DrawWireSphere(p, 0.1f);
        Gizmos.DrawLine(transform.position, p);
    }
}

