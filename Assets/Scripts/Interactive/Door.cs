using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour, IInteractableTarget
{
    [Header("传送目标")]
    public Transform teleportTarget;      // 传送去哪里
    [Header("玩家")]
    public Transform playerTransform;     // 玩家对象

    [Header("interact hint")]
    public GameObject interactHint;
    public Vector3 hintOffset = new Vector3(0, 1.2f, 0);
    public bool faceCamera = true;
    public bool IsPlayerInRange => playerInRange;
    private bool playerInRange = false;
    private bool isTeleporting = false;
    private Camera cam;
    private Transform hintTransform;
    private Collider2D hintCol;


    private void Awake()
    {
        cam = Camera.main;

        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
        }

        if (interactHint != null)
        {
            hintCol = interactHint.GetComponent<Collider2D>();
            hintTransform = interactHint.transform;
            interactHint.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerTransform = other.transform;
        }

        // 不再直接开关自己的 Hint，交给 Manager 统一决定
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
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!playerInRange) return;
        if (!ctx.performed) return;
        if (!playerInRange) return;
        if (isTeleporting) return;                // ⭐ 防止同一帧多次触发
        if (playerTransform == null || teleportTarget == null) return;


        // 关键：只允许当前最近的那个响应
        if (InteractManager.Instance != null &&
            !InteractManager.Instance.IsCurrent(this))
            return;
        isTeleporting = true;
        playerTransform.position = teleportTarget.position;
        isTeleporting = false;
    }

    public void SetHintVisible(bool visible)
    {
        if (interactHint == null) return;
        interactHint.SetActive(visible);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.85f, 0f, 0.8f);
        var p = transform.position + hintOffset;
        Gizmos.DrawWireSphere(p, 0.1f);
        Gizmos.DrawLine(transform.position, p);
    }
}
