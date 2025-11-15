using System.Collections.Generic;
using UnityEngine;

public class InteractManager : MonoBehaviour
{
    public static InteractManager Instance { get; private set; }

    private readonly List<IInteractableTarget> inRangeTargets = new();
    private IInteractableTarget current;   // 当前选中的那个
    private Transform playerTf;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // 默认用 Tag 找玩家，有别的写法的话你可以改
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTf = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("[InteractManager] 场景里找不到 Tag=Player 的物体，会导致最近目标判断失效。");
        }
    }

    // 进范围时注册
    public void Register(IInteractableTarget target)
    {
        if (target == null) return;
        if (!inRangeTargets.Contains(target))
        {
            inRangeTargets.Add(target);
        }
    }

    // 出范围时注销
    public void Unregister(IInteractableTarget target)
    {
        if (target == null) return;
        if (inRangeTargets.Remove(target))
        {
            if (current == target)
            {
                current.SetHintVisible(false);
                current = null;
            }
        }
    }

    private void LateUpdate()
    {
        if (playerTf == null) return;

        float bestDistSq = float.MaxValue;
        IInteractableTarget best = null;

        // 在所有“玩家在范围内”的目标里找最近的
        for (int i = 0; i < inRangeTargets.Count; i++)
        {
            var t = inRangeTargets[i];
            if (t == null) continue;
            if (!t.IsPlayerInRange) continue;

            float d2 = (t.transform.position - playerTf.position).sqrMagnitude;
            if (d2 < bestDistSq)
            {
                bestDistSq = d2;
                best = t;
            }
        }

        if (best != current)
        {
            // 切换高亮对象
            if (current != null)
                current.SetHintVisible(false);

            current = best;

            if (current != null)
                current.SetHintVisible(true);
        }
    }

    // 提供一个查询：某个目标是不是当前那个
    public bool IsCurrent(IInteractableTarget target)
    {
        return target != null && target == current;
    }
}
