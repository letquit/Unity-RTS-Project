using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SampleProject
{
    /// <summary>
    /// 移动组管理器，负责管理多个移动组代理的创建、更新和销毁
    /// </summary>
    public sealed class MovingGroupManager : MonoBehaviour
    {
        private readonly List<MovingGroupAgent> groupAgents = new();
        private readonly List<MovingGroupAgent> cache = new();

        /// <summary>
        /// 添加一个新的移动组到管理器中
        /// </summary>
        /// <param name="agents">要添加到组中的移动代理集合</param>
        /// <param name="destination">该组的目標位置</param>
        public void AddGroup(IEnumerable<MoveAgent> agents, Vector3 destination)
        {
            // 检查现有组中是否包含相同的代理，如果包含则从现有组中移除
            foreach (var activeGroup in this.groupAgents.ToList())
            {
                activeGroup.RemoveAgents(agents);
                if (activeGroup.IsCompleted())
                {
                    this.groupAgents.Remove(activeGroup);
                }
            }

            this.groupAgents.Add(new MovingGroupAgent(agents, destination));
        }

        /// <summary>
        /// 固定更新方法，在物理更新时调用，用于更新所有移动组的状态
        /// </summary>
        private void FixedUpdate()
        {
            // 使用缓存列表避免在迭代过程中修改集合
            this.cache.Clear();
            this.cache.AddRange(this.groupAgents);

            for (int i = 0, count = this.cache.Count; i < count; i++)
            {
                var group = this.cache[i];
                group.Update();

                // 移除已完成的组
                if (group.IsCompleted())
                {
                    this.groupAgents.Remove(group);
                }
            }
        }
    }
}
