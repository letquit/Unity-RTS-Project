using Ecs;
using Systems;
using UnityEngine;

namespace SampleProject
{
    /// <summary>
    /// ECS管理器，负责初始化ECS世界、绑定组件和系统，并驱动ECS的更新循环
    /// </summary>
    public sealed class EcsManager : MonoBehaviour
    {
        private readonly EcsWorld ecsWorld = new();

        /// <summary>
        /// 在MonoBehaviour唤醒时初始化ECS世界，绑定所需组件和系统，并初始化场景中的实体
        /// </summary>
        private void Awake()
        {
            // 绑定移动相关的组件
            this.ecsWorld.BindComponent<MoveStateComponent>();
            this.ecsWorld.BindComponent<MoveSpeedComponent>();
            this.ecsWorld.BindComponent<TransformComponent>();
            this.ecsWorld.BindComponent<AnimatorComponent>();
            
            // 绑定移动相关的系统
            this.ecsWorld.BindSystem<MovementSystem>();
            this.ecsWorld.BindSystem<MoveAnimationSystem>();
            
            // 安装并初始化ECS世界
            this.ecsWorld.Install();

            // 初始化场景中所有的实体
            foreach (var entity in FindObjectsByType<Entity>(FindObjectsSortMode.None))
            {
                entity.Init(this.ecsWorld);
            }
        }

        /// <summary>
        /// 每帧更新ECS世界的状态
        /// </summary>
        private void Update()
        {
            this.ecsWorld.Update();
        }

        /// <summary>
        /// 固定时间步长更新ECS世界，适用于物理计算
        /// </summary>
        private void FixedUpdate()
        {
            this.ecsWorld.FixedUpdate();
        }

        /// <summary>
        /// 延迟更新ECS世界，在所有Update执行完成后调用
        /// </summary>
        private void LateUpdate()
        {
            this.ecsWorld.LateUpdate();
        }
    }
}

