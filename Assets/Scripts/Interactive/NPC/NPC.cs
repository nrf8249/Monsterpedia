using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 最小可用版 NPC：
/// - 玩家进入触发器范围才能互动；
/// - 按下“交互”输入后，调用 DialogueManager 开始对话；
/// - 离开范围时关闭对话框。
///
/// 使用方法：
/// 1) 给 NPC 物体加一个 2D Trigger Collider（如 CircleCollider2D，勾选 IsTrigger）；
/// 2) 给玩家物体打上 Tag = "Player"；
/// 3) 在 Inspector 里把 talkData（DialogueData 资源）拖进来；
/// 4) 输入系统把“交互键”（如 E 键）绑定到本脚本的 OnInteract。
/// </summary>
public class NPC : MonoBehaviour
{
    [Header("对话数据")]
    [Tooltip("与该 NPC 对话时要播放的内容（DialogueData ScriptableObject）")]
    public DialogueData talkData;

    [Header("可选：靠近提示UI")]
    [Tooltip("玩家靠近时要显示的互动提示（可留空，不影响功能）")]
    public GameObject interactHint;

    // 内部状态：玩家是否在范围内
    private bool playerInRange = false;

    // —— 触发器进入/离开范围 —— //
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;
        Debug.Log($"{name}: 玩家进入交互范围。");

        // 显示“按键提示”（可选）
        if (interactHint) interactHint.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        Debug.Log($"{name}: 玩家离开交互范围。");

        // 隐藏提示
        if (interactHint) interactHint.SetActive(false);

        // 玩家走开时关闭正在显示的对话框
        DialogueManager.Instance.StopDialogue();
    }

    // —— 交互输入回调（在 Input Actions 中绑定到本方法）—— //
    public void OnInteract(InputAction.CallbackContext context)
    {
        // 只在按键“刚开始”那一刻触发，而且必须在范围内
        if (!context.started || !playerInRange) return;

        if (talkData == null)
        {
            Debug.LogWarning($"{name}: 未设置 talkData，对话无法开始。");
            return;
        }

        // 调用统一的对话入口：开始“正式对话”（逐字打印 narrativeText）
        DialogueManager.Instance.StartDialogue(talkData);

        // 如果你想先弹一条“开场提示”，再进入正式对话，可以换成这两步：
        // DialogueManager.Instance.ShowIntro("……（这里是按钮上方的开场提示）", showButtons: true);
        // 之后在按钮或另一个输入里再调用：DialogueManager.Instance.StartDialogue(talkData);
    }
}

