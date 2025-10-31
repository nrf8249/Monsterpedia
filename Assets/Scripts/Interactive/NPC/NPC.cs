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
    public bool debugLogs = true;                   // 开关
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
        else
        {
            if (debugLogs) Debug.LogWarning($"{name} | interactHint 未赋值。进入范围时不会显示提示。", this);
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
        TryShowHint("OnTriggerEnter2D");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;

        if (interactHint) interactHint.SetActive(false);
        DialogueManager.Instance.StopDialogue();

        if (debugLogs) Debug.Log($"{name} | 玩家离开范围，隐藏提示 + 关闭对话框。", this);
    }

    // 为了排查“卡边缘进不去”的情况，持续报告
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!debugLogs) return;
        if (!other.CompareTag("Player")) return;

        // 每秒左右打一条（避免刷屏）
        if (Time.frameCount % 60 == 0)
        {
            Debug.Log($"{name} | OnTriggerStay2D: 玩家仍在范围内。", this);
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.started || !playerInRange) return;

        if (talkData == null)
        {
            if (debugLogs) Debug.LogWarning($"{name} | 未设置 talkData，对话无法开始。", this);
            return;
        }

        var payload = new NarrativePayload(
            data: talkData,
            portrait: portrait,
            characterName: string.IsNullOrEmpty(npcName) ? null : npcName,
            asMonologue: false
        );

        if (debugLogs) Debug.Log($"{name} | 按键交互开始对话。Data 行数 = {talkData.lines?.Length ?? 0}", this);
        DialogueManager.Instance.StartDialogue(payload);
    }

    private void TryShowHint(string reason)
    {
        if (interactHint == null)
        {
            if (debugLogs) Debug.LogWarning($"{name} | {reason}: interactHint 为空，无法显示提示。", this);
            return;
        }

        // 先设为 true
        interactHint.SetActive(true);

        if (debugLogs)
        {
            // 打印当前状态 & 组件信息
            string active = $"activeSelf={interactHint.activeSelf}, activeInHierarchy={interactHint.activeInHierarchy}";
            string pos = $"worldPos={interactHint.transform.position}  offsetTarget={transform.position + hintOffset}";

            // SpriteRenderer 路线（世界内 sprite）
            var sr = interactHint.GetComponentInChildren<SpriteRenderer>(true);
            string srInfo = sr
                ? $"SR[sprite={(sr.sprite ? sr.sprite.name : "null")}, color.a={sr.color.a:F2}, layer={SortingLayer.IDToName(sr.sortingLayerID)}, order={sr.sortingOrder}]"
                : "SR[null]";

            // UI 路线（World Space/Overlay）
            var canvas = interactHint.GetComponentInChildren<Canvas>(true);
            var img = interactHint.GetComponentInChildren<Image>(true);
            var cg = interactHint.GetComponentInChildren<CanvasGroup>(true);

            string canvasInfo = canvas
                ? $"Canvas[mode={canvas.renderMode}, sortingLayer={SortingLayer.IDToName(canvas.sortingLayerID)}, order={canvas.sortingOrder}]"
                : "Canvas[null]";
            string imgInfo = img
                ? $"Image[sprite={(img.sprite ? img.sprite.name : "null")}, color.a={img.color.a:F2}]"
                : "Image[null]";
            string cgInfo = cg ? $"CanvasGroup[alpha={cg.alpha:F2}, blocks={cg.blocksRaycasts}, interactable={cg.interactable}]" : "CanvasGroup[null]";

            Debug.Log($"{name} | 显示提示（{reason}） -> {active}; {pos}\n    {srInfo}; {canvasInfo}; {imgInfo}; {cgInfo}", this);
        }

        // 立刻把它放到正确的位置（避免下一帧才移动）
        interactHint.transform.position = transform.position + hintOffset;
        if (faceCamera && mainCam) interactHint.transform.forward = mainCam.transform.forward;
    }

    // 在场景视图里可见提示位置
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;
        Vector3 p = Application.isPlaying ? (transform.position + hintOffset) : (transform.position + hintOffset);
        Gizmos.DrawWireSphere(p, 0.1f);
        Gizmos.DrawLine(transform.position, p);
    }
}
