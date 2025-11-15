using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ClueGround : MonoBehaviour, IInteractableTarget
{
    [Header("clue data")]
    public MonologueData data;
    public string clueName;
    public int clueID;

    [Header("interact hint")]
    public GameObject interactHint;
    public Vector3 hintOffset = new Vector3(0, 1.0f, 0.0f);
    public bool faceCamera = true;

    [Header("visible sorting")]
    public string sortingLayerName = "UI";
    public int sortingOrder = 100;

    [Header("debug")]
    public bool debugLogs = true;

    public bool IsPlayerInRange => playerInRange;
    private bool playerInRange = false;
    private Camera cam;
    private Transform hintTf;
    private Collider2D hintCol;

    // Start is called before the first frame update
    private void Awake()
    {
        cam = Camera.main;

        if (interactHint != null)
        {
            hintCol = interactHint.GetComponent<Collider2D>();
            hintTf = interactHint.transform;
            interactHint.SetActive(false);
        }
        else if (debugLogs)
        {
            Debug.LogWarning($"{name} | interactHint 未赋值（进入范围也不会显示图标）。", this);
        }
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (interactHint && interactHint.activeSelf)
        {
            hintTf.position = transform.position + hintOffset;
            if (faceCamera && cam) hintTf.forward = cam.transform.forward;
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

    // player enters the clue range
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;

        // ✅ 交给 Manager 管理谁显示
        if (InteractManager.Instance != null)
            InteractManager.Instance.Register(this);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;

        if (InteractManager.Instance != null)
            InteractManager.Instance.Unregister(this);

        if (interactHint) interactHint.SetActive(false);
        if (debugLogs) Debug.Log($"{name} | 玩家离开线索范围，隐藏提示。", this);

        if (NarrativeBoxManager.Instance != null)
        {
            NarrativeBoxManager.Instance.StopDialogue();
            if (debugLogs) Debug.Log($"{name} | 玩家离开线索范围，关闭 Monologue UI。", this);
        }
    }


    // interact input action
    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!playerInRange) return;

        if (InteractManager.Instance != null &&
            !InteractManager.Instance.IsCurrent(this))
            return;

        StartMonologue();
    }

    public void ClickHint()
    {
        if (!playerInRange) return;

        if (InteractManager.Instance != null &&
            !InteractManager.Instance.IsCurrent(this))
            return;

        var gm = GameManager.Instance;
        if (!NarrativeBoxManager.Instance.CanStartNarrative) return;

        if (debugLogs) Debug.Log($"{name} 被点击 → 开始独白");
        StartMonologue();
    }



    // try to show the interact hint
    private void TryShowHint(string reason)
    {
        if (interactHint == null)
        {
            if (debugLogs) Debug.LogWarning($"{name} | {reason}: interactHint 为空。", this);
            return;
        }

        interactHint.SetActive(true);

        // 置顶层级
        var sr = interactHint.GetComponentInChildren<SpriteRenderer>(true);
        if (sr)
        {
            sr.sortingLayerName = sortingLayerName;
            sr.sortingOrder = sortingOrder;
        }

        var canvas = interactHint.GetComponentInChildren<Canvas>(true);
        if (canvas && canvas.renderMode == RenderMode.WorldSpace)
        {
            canvas.sortingLayerName = sortingLayerName;
            canvas.sortingOrder = sortingOrder;
            var cg = interactHint.GetComponentInChildren<CanvasGroup>(true);
            if (cg) cg.alpha = 1f;
        }

        interactHint.transform.position = transform.position + hintOffset;
        if (faceCamera && cam) interactHint.transform.forward = cam.transform.forward;
    }

    // Start the monologue dialogue
    private void StartMonologue()
    {
        if (NarrativeBoxManager.Instance == null)
        {
            Debug.LogError($"{name} | DialogueManager.Instance 为空。", this);
            return;
        }

        if (data == null)
        {
            Debug.LogWarning($"{name} | ClueData 未绑定。", this);
            return;
        }

        var payload = new MonologuePayload(
            data: data,
            portrait: null,
            clueName: clueName
        );
        if (NarrativeBoxManager.Instance.IsNarrating)
            return;
        NarrativeBoxManager.Instance.StartMonologue(payload);
        Inventory.instance.GetClue(clueName);
    }

    // Gizmos for hint position
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.85f, 0f, 0.8f);
        var p = transform.position + hintOffset;
        Gizmos.DrawWireSphere(p, 0.1f);
        Gizmos.DrawLine(transform.position, p);
    }

    public void SetHintVisible(bool visible)
    {
        if (interactHint == null) return;

        if (visible)
        {
            // 复用你原来的显示逻辑，保证 sortingLayer 等正常
            TryShowHint("InteractManager");
        }
        else
        {
            interactHint.SetActive(false);
        }
    }
}
