using Ecs;
using SampleProject;
using UnityEngine;

namespace Systems
{
    /// <summary>
    /// 移动系统，负责处理实体的移动逻辑
    /// 在每个固定更新周期中，根据实体的移动状态、速度和变换组件来更新实体的位置和朝向
    /// </summary>
    public sealed class MovementSystem : IFixedUpdateSystem
    {
        private ComponentPool<MoveStateComponent> statePool;
        private ComponentPool<MoveSpeedComponent> speedPool;
        private ComponentPool<TransformComponent> transformPool;

        /// <summary>
        /// 在固定更新周期中执行移动逻辑
        /// </summary>
        /// <param name="entity">实体的索引</param>
        void IFixedUpdateSystem.OnFixedUpdate(int entity) //Index на мою сущность!
        {
            // 检查实体是否具有移动状态组件
            if (!this.statePool.HasComponent(entity))
            {
                return;
            }
            
            ref MoveStateComponent stateComponent = ref this.statePool.GetComponent(entity);
            // 如果不需要移动，则直接返回
            if (!stateComponent.moveRequired)
            {
                return;
            }
            
            // 执行移动逻辑：
            ref TransformComponent transformComponent = ref this.transformPool.GetComponent(entity);
            ref MoveSpeedComponent moveSpeedComponent = ref this.speedPool.GetComponent(entity);

            var direction = stateComponent.direction;
            var offset = direction * moveSpeedComponent.value * Time.fixedDeltaTime;
            transformComponent.value.position += offset;
            transformComponent.value.rotation = Quaternion.LookRotation(direction, Vector3.up);

            stateComponent.moveRequired = false;
        }
    }
}
