using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour, IInteractableTarget
{

    [Header("interact hint")]
    public GameObject interactHint;
    public Vector3 hintOffset = new Vector3(0, 1.2f, 0);
    public bool faceCamera = true;
    public bool IsPlayerInRange => playerInRange;
    private bool playerInRange = false;
    private Camera cam;
    private Transform hintTransform;
    private Collider2D hintCol;

    public string levelToLoad = "";

    private void Awake()
    {
        cam = Camera.main;

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
        if (!other.CompareTag("Player")) return;
        playerInRange = true;

        // 不再直接开关自己的 Hint，交给 Manager 统一决定
        if (InteractManager.Instance != null)
            InteractManager.Instance.Register(this);

        
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!playerInRange) return;

        // 关键：只允许当前最近的那个响应
        if (InteractManager.Instance != null &&
            !InteractManager.Instance.IsCurrent(this))
            return;

        SceneManager.LoadScene(levelToLoad);

    }

    public void SetHintVisible(bool visible)
    {
        if (interactHint == null) return;
        interactHint.SetActive(visible);
    }
}
