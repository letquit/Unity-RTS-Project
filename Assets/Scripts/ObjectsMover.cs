using System;
using System.Linq;
using Ecs;
using UnityEngine;

namespace SampleProject
{
    /// <summary>
    /// 移动控制器组件，用于控制实体的移动行为
    /// </summary>
    public sealed class ObjectsMover : MonoBehaviour
    {
        [SerializeField]
        private MovingGroupManager movingGroupManager;
        
        private MoveAgent[] agents;

        /// <summary>
        /// Unity生命周期函数，在对象启用时调用
        /// 负责初始化实体数组，查找场景中所有的Entity组件
        /// </summary>
        private void Awake()
        {
            this.agents = FindObjectsByType<MoveAgent>(FindObjectsSortMode.None);
        }

        /// <summary>
        /// Unity生命周期函数，每帧调用一次
        /// 检测鼠标右键点击事件，如果点击到地面，则移动所有代理到点击位置
        /// </summary>
        private void Update()
        {
            // 检查鼠标右键是否按下
            if (!Input.GetMouseButtonDown(1))
            {
                return;
            }

            // 从摄像机发射射线到鼠标位置
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            // 检测射线是否击中物体
            if (!Physics.Raycast(ray, out var hit))
            {
                return;
            }

            // 检查击中的物体是否为地面标签
            if (hit.transform.CompareTag("Ground"))
            {
                this.MoveToPosition(hit.point);
            }
        }

        /// <summary>
        /// 将所有移动代理移动到指定位置
        /// </summary>
        /// <param name="targetPosition">目标位置坐标</param>
        private void MoveToPosition(Vector3 targetPosition)
        {
            // 将目标位置的Y坐标设为0（地面高度）
            targetPosition.y = 0;

            // 计算所有代理的中心位置（当前被注释掉的代码）
            // Vector3 centerPosition = this.CalculateCenter(this.agents.Select(it => it.transform.position).ToArray());
            
            // 将代理组添加到移动组管理器中
            this.movingGroupManager.AddGroup(this.agents, targetPosition);
            
            // 遍历所有代理并执行移动操作
            foreach (var agent in this.agents)
            {
                agent.MoveToPosition(targetPosition);
                // 计算相对于中心的偏移位置（当前被注释掉的代码）
                // Vector3 offset = agent.transform.position - centerPosition;
                // agent.MoveToPosition(targetPosition + offset);
            }
        }

        /// <summary>
        /// 计算给定点集的几何中心位置
        /// </summary>
        /// <param name="points">需要计算中心的点坐标数组</param>
        /// <returns>所有点的平均坐标位置</returns>
        private Vector3 CalculateCenter(Vector3[] points)
        {
            Vector3 result = Vector3.zero;
            
            // 累加所有点的坐标
            foreach (var point in points)
            {
                result += point;
            }
            
            // 返回平均坐标（总和除以点的数量）
            return result / points.Length;
        }
    }
}

