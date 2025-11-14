public interface IInteractableTarget
{
    // 交互物体的世界位置（用来算和玩家的距离）
    UnityEngine.Transform transform { get; }

    // 当前玩家是否在范围内（由自己脚本维护）
    bool IsPlayerInRange { get; }

    // 控制提示显隐
    void SetHintVisible(bool visible);
}
