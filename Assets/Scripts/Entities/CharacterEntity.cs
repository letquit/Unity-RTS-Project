using System;
using Ecs;
using SampleProject;
using UnityEngine;

namespace Entities
{
    /// <summary>
    /// 角色实体类，继承自Entity基类，用于创建和初始化角色相关的组件数据
    /// </summary>
    public sealed class CharacterEntity : Entity
    {
        [SerializeField]
        private float speed = 5.0f;
        
        /// <summary>
        /// 实体初始化方法，在实体创建时调用，用于设置角色所需的各种组件数据
        /// </summary>
        protected override void OnInit()
        {
            // 设置移动速度组件
            this.SetData(new MoveSpeedComponent
            {
                value = this.speed
            });
            
            // 设置变换组件
            this.SetData(new TransformComponent
            {
                value = this.transform
            });
            
            // 设置移动状态组件
            this.SetData(new MoveStateComponent());
            
            // 设置动画组件
            this.SetData(new AnimatorComponent
            {
                value = this.GetComponent<Animator>()
            });
        }
    }
}
