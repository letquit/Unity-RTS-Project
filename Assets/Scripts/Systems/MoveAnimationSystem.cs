using Ecs;
using SampleProject;
using UnityEngine;

namespace Systems
{
    /// <summary>
    /// 移动动画系统，用于控制实体的移动动画状态
    /// 该系统在LateUpdate阶段执行，根据实体的移动状态来设置对应的动画参数
    /// </summary>
    public sealed class MoveAnimationSystem : ILateUpdateSystem
    {
        private static readonly int State = Animator.StringToHash("State");

        private ComponentPool<AnimatorComponent> animatorPool;
        private ComponentPool<MoveStateComponent> movePool;

        /// <summary>
        /// LateUpdate系统回调方法，在每帧的后期更新阶段执行
        /// 根据实体的移动状态组件来更新动画组件的状态参数
        /// </summary>
        /// <param name="entity">要处理的实体ID</param>
        void ILateUpdateSystem.OnLateUpdate(int entity)
        {
            // 检查实体是否同时拥有动画组件和移动状态组件
            if (!this.animatorPool.HasComponent(entity) || !this.movePool.HasComponent(entity))
            {
                return;
            }

            ref AnimatorComponent animatorComponent = ref this.animatorPool.GetComponent(entity);
            ref MoveStateComponent moveStateComponent = ref this.movePool.GetComponent(entity);

            // 根据移动需求状态设置动画状态参数
            if (moveStateComponent.moveRequired)
            {
                animatorComponent.value.SetInteger(State, 1);
            }
            else
            {
                animatorComponent.value.SetInteger(State, 0);
            }
        }
    }
}
