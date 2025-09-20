using Ecs;
using SampleProject;

namespace Systems
{
    /// <summary>
    /// 移动到指定位置的系统类，实现固定更新逻辑
    /// 该系统负责处理实体的移动命令，计算移动方向并更新移动状态
    /// </summary>
    public sealed class MoveToPositionSystem : IFixedUpdateSystem
    {
        private const float MIN_SQR_DISTANCE = 0.01f;

        private ComponentPool<MoveToPositionCommand> commandPool;
        private ComponentPool<MoveStateComponent> movePool;
        private ComponentPool<TransformComponent> transformPool;

        /// <summary>
        /// 固定更新方法，在每个固定时间步长执行
        /// 处理指定实体的移动逻辑，计算到目标位置的方向并向量
        /// </summary>
        /// <param name="entity">需要处理移动逻辑的实体ID</param>
        void IFixedUpdateSystem.OnFixedUpdate(int entity)
        {
            // 检查实体是否有移动命令组件，如果没有则直接返回
            if (!this.commandPool.HasComponent(entity))
            {
                return;
            }

            ref MoveToPositionCommand command = ref this.commandPool.GetComponent(entity);
            ref MoveStateComponent moveComponent = ref this.movePool.GetComponent(entity);
            ref TransformComponent transformComponent = ref this.transformPool.GetComponent(entity);

            var endPosition = command.destination;
            var myPosition = transformComponent.value.position;

            // 计算当前位置到目标位置的距离向量，并判断是否需要继续移动
            var distanceVector = endPosition - myPosition;
            if (distanceVector.sqrMagnitude > MIN_SQR_DISTANCE)
            {
                // 距离目标位置较远，需要继续移动，设置移动方向
                moveComponent.moveRequired = true;
                moveComponent.direction = distanceVector.normalized;
            }
            else
            {
                // 已经接近目标位置，移除移动命令组件
                this.commandPool.RemoveComponent(entity);
            }
        }
    }
}
