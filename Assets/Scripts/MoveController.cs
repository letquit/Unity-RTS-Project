using System;
using Ecs;
using UnityEngine;

namespace SampleProject
{
    /// <summary>
    /// 移动控制器组件，用于控制实体的移动行为
    /// </summary>
    public sealed class MoveController : MonoBehaviour
    {
        private Entity[] entities;

        /// <summary>
        /// Unity生命周期函数，在对象启用时调用
        /// 负责初始化实体数组，查找场景中所有的Entity组件
        /// </summary>
        private void Awake()
        {
            this.entities = FindObjectsByType<Entity>(FindObjectsSortMode.None);
        }

        /// <summary>
        /// 将所有实体向前移动10个单位
        /// 通过上下文菜单可调用此方法
        /// </summary>
        [ContextMenu("Move Forward")]
        public void MoveForward()
        {
            // 遍历所有实体，为每个实体设置移动目标位置
            foreach (var entity in this.entities)
            {
                entity.SetData(new MoveToPositionCommand
                {
                    destination = entity.transform.position + Vector3.forward * 10
                });
            }
        }

        // private void Update()
        // {
        //     ref MoveStateComponent moveStateComponent = ref this.entity.GetData<MoveStateComponent>();
        //
        //     if (Input.GetKey(KeyCode.LeftArrow))
        //     {
        //         moveStateComponent.moveRequired = true;
        //         moveStateComponent.direction = Vector3.left;
        //     }
        //     else if (Input.GetKey(KeyCode.RightArrow))
        //     {
        //         moveStateComponent.moveRequired = true;
        //         moveStateComponent.direction = Vector3.right;
        //     }
        // }
    }
}
