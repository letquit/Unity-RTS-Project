using System;
using System.Collections.Generic;
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
        /// 移动指定的游戏对象集合到目标位置
        /// </summary>
        /// <param name="objects">需要移动的游戏对象集合</param>
        /// <param name="destination">目标位置坐标</param>
        public void MoveObjects(IEnumerable<GameObject> objects, Vector3 destination)
        {
            // 将目标位置的Y轴坐标设为0，确保在水平面上移动
            destination.y = 0;
            
            // 筛选出具有MoveAgent组件的游戏对象
            List<MoveAgent> agents = new List<MoveAgent>();
            foreach (var obj in objects)
            {
                if (obj.TryGetComponent(out MoveAgent agent))
                {
                    agents.Add(agent);
                }
            }

            // 将代理添加到移动组管理器中进行统一管理
            this.movingGroupManager.AddGroup(agents, destination);
            
            // 通知所有代理开始移动到指定位置
            foreach (var agent in agents)
            {
                agent.MoveToPosition(destination);
            }
        }
    }
}

