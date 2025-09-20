using System.Collections.Generic;
using UnityEngine;

namespace SampleProject
{
    /// <summary>
    /// 管理一组移动代理（MoveAgent）的类，用于协调多个代理的移动行为。
    /// 包括路径修正、障碍物规避和完成状态判断等功能。
    /// </summary>
    public sealed class MovingGroupAgent
    {
        private const float STOPPING_DISTANCE = 1.0f;
        private const float OBSTACLE_AVOID_DISTANCE = 2.5f;
        private const float EQUALS_POINT_DISTANCE = 0.1f;
        private const float COMPLETE_RADIUS = 4.0f;
        private const float COMPLETE_RADIUS_COEF = 4 * Mathf.PI;

        private readonly List<MoveAgent> movingAgents;
        private readonly List<MoveAgent> completeAgents;
        private readonly List<MoveAgent> cache;
        private readonly Vector3 destination;
        private readonly float completeRadius;

        /// <summary>
        /// 初始化一个 MovingGroupAgent 实例。
        /// </summary>
        /// <param name="agents">初始的移动代理集合。</param>
        /// <param name="destination">目标位置。</param>
        public MovingGroupAgent(IEnumerable<MoveAgent> agents, Vector3 destination)
        {
            this.movingAgents = new List<MoveAgent>(agents);
            this.completeAgents = new List<MoveAgent>();
            this.cache = new List<MoveAgent>();

            this.destination = destination;
            this.completeRadius = Mathf.Max(COMPLETE_RADIUS, this.movingAgents.Count / COMPLETE_RADIUS_COEF);
        }

        /// <summary>
        /// 判断所有代理是否已完成移动。
        /// </summary>
        /// <returns>如果所有代理已完成移动则返回 true，否则返回 false。</returns>
        public bool IsCompleted()
        {
            return this.movingAgents.Count <= 0;
        }

        /// <summary>
        /// 更新代理组的状态，包括路径修正、障碍规避和完成判断。
        /// </summary>
        public void Update()
        {
            this.cache.Clear();
            this.cache.AddRange(this.movingAgents);
            
            CorrectAgentPaths(this.cache);
            AvoidObstacles(this.cache);
            
            this.CompleteAgents();
        }

        /// <summary>
        /// 从代理组中移除指定的代理。
        /// </summary>
        /// <param name="agents">要移除的代理集合。</param>
        public void RemoveAgents(IEnumerable<MoveAgent> agents)
        {
            foreach (var agent in agents)
            {
                this.completeAgents.Remove(agent);
                this.movingAgents.Remove(agent);
            }
        }
        
        /// <summary>
        /// 检查并标记已完成移动的代理。
        /// </summary>
        private void CompleteAgents()
        {
            // 将当前移动中的代理复制到缓存中进行处理
            this.cache.Clear();
            this.cache.AddRange(this.movingAgents);

            // 先处理已经标记为完成的代理
            for (int i = 0, count = this.cache.Count; i < count; i++)
            {
                var agent = this.cache[i];
                if (agent.IsCompleted)
                {
                    this.movingAgents.Remove(agent);
                    this.completeAgents.Add(agent);
                }
            }
            
            // 检查未完成的代理是否满足完成条件
            for (int i = 0, movingCount = this.movingAgents.Count; i < movingCount; i++)
            {
                var agent = this.movingAgents[i];
                var agentPosition = agent.transform.position;
                
                // 如果代理距离目标点超过完成半径，则跳过
                if (Vector3.Distance(agentPosition, this.destination) > this.completeRadius)
                {
                    continue;
                }

                // 如果代理距离目标点足够近，则标记为完成
                if (Vector3.Distance(agentPosition, this.destination) <= STOPPING_DISTANCE)
                {
                    agent.CompleteMove();
                    continue;
                }

                // 检查是否有已完成的代理在附近，如果有则标记当前代理为完成
                for (int j = 0, completeCount = this.completeAgents.Count; j < completeCount; j++)
                {
                    var otherAgent = this.completeAgents[j];
                   
                    var otherPosition = otherAgent.transform.position;
                    if (Vector3.Distance(agentPosition, otherPosition) < STOPPING_DISTANCE)
                    {
                        agent.CompleteMove();
                        break;
                    }
                }
            }
        }

        #region CorrectPaths

        /// <summary>
        /// 对一组代理进行路径修正。
        /// </summary>
        /// <param name="agents">需要进行路径修正的代理列表。</param>
        private static void CorrectAgentPaths(List<MoveAgent> agents)
        {
            var count = agents.Count;

            for (var i = 0; i < count; i++)
            {
                var agent = agents[i];
                CorrectAgentPath(agent, agents);
            }
        }

        /// <summary>
        /// 根据周围代理的状态对单个代理进行路径修正。
        /// </summary>
        /// <param name="agent">需要修正路径的代理。</param>
        /// <param name="otherAgents">其他代理的列表。</param>
        private static void CorrectAgentPath(MoveAgent agent, List<MoveAgent> otherAgents)
        {
            if (!agent.CanCorrectPath)
            {
                return;
            }
            
            // 如果附近有已完成路径修正的代理，则进行路径修正
            if (HasCorrectedAgentNear(agent, otherAgents))
            {
                Debug.Log("CORRECT NEAR AGENT>>>");
                agent.CorrectPath();
                return;
            }

            // 如果存在即将碰撞的情况，则进行路径修正
            if (CheckCollisionForCorrect(agent, otherAgents))
            {
                Debug.Log("CORRECT PATH AGENT>>>");
                agent.CorrectPath();
            }
        }

        /// <summary>
        /// 检查是否存在即将发生碰撞的情况。
        /// </summary>
        /// <param name="agent">当前代理。</param>
        /// <param name="otherAgents">其他代理的列表。</param>
        /// <returns>如果存在即将碰撞的情况则返回 true，否则返回 false。</returns>
        private static bool CheckCollisionForCorrect(MoveAgent agent, List<MoveAgent> otherAgents)
        {
            var position = agent.transform.position;
            if (!agent.TryGetNextPosition(out var targetPosition))
            {
                return false;
            }

            // 如果当前位置与目标位置距离较远，则不进行碰撞检测
            if (Vector3.Distance(position, targetPosition) > STOPPING_DISTANCE)
            {
                return false;
            }

            // 遍历其他代理，检查是否存在目标位置重叠的情况
            for (int i = 0, count = otherAgents.Count; i < count; i++)
            {
                var otherAgent = otherAgents[i];
                if (otherAgent == agent)
                {
                    continue;
                }

                if (!otherAgent.TryGetNextPosition(out var otherTargetPosition))
                {
                    continue;
                }

                // 如果两个代理的目标位置距离较远，则跳过
                if (Vector3.Distance(targetPosition, otherTargetPosition) > EQUALS_POINT_DISTANCE)
                {
                    continue;
                }

                // 如果另一个代理当前位置与当前代理目标位置较近，则认为即将发生碰撞
                if (Vector3.Distance(otherAgent.transform.position, targetPosition) < STOPPING_DISTANCE)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 检查附近是否有已完成路径修正的代理。
        /// </summary>
        /// <param name="agent">当前代理。</param>
        /// <param name="otherAgents">其他代理的列表。</param>
        /// <returns>如果附近有已完成路径修正的代理则返回 true，否则返回 false。</returns>
        private static bool HasCorrectedAgentNear(MoveAgent agent, List<MoveAgent> otherAgents)
        {
            var position = agent.transform.position;

            // 遍历其他代理，检查是否有已完成路径修正且距离较近的代理
            for (int i = 0, count = otherAgents.Count; i < count; i++)
            {
                var otherAgent = otherAgents[i];
                if (otherAgent == agent)
                {
                    continue;
                }

                if (otherAgent.CanCorrectPath)
                {
                    continue;
                }

                var otherPosition = otherAgent.transform.position;
                if (Vector3.Distance(position, otherPosition) <= STOPPING_DISTANCE)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
        
        #region AvoidObstacles

        /// <summary>
        /// 对一组代理进行障碍物规避处理。
        /// </summary>
        /// <param name="agents">需要进行障碍物规避的代理列表。</param>
        private static void AvoidObstacles(List<MoveAgent> agents)
        {
            for (int i = 0, count = agents.Count; i < count; i++)
            {
                var agent = agents[i];
                AvoidObstacles(agent, agents);
            }
        }

        /// <summary>
        /// 对单个代理进行障碍物规避处理。
        /// </summary>
        /// <param name="agent">需要进行障碍物规避的代理。</param>
        /// <param name="agents">其他代理的列表。</param>
        private static void AvoidObstacles(MoveAgent agent, List<MoveAgent> agents)
        {
            if (agent.IsObstacleAvoid)
            {
                return;
            }
            
            var agentPosition = agent.transform.position;
            
            // 遍历其他代理，检查是否有正在进行障碍物规避且距离较近的代理
            for (int i = 0, count = agents.Count; i < count; i++)
            {
                var otherAgent = agents[i];
                if (otherAgent == agent)
                {
                    continue;
                }

                if (!otherAgent.IsObstacleAvoid)
                {
                    continue;
                }
            
                var otherPosition = agent.transform.position;
                if (Vector3.Distance(agentPosition, otherPosition) <= OBSTACLE_AVOID_DISTANCE)
                {
                    otherAgent.StartAvoidObstacle();
                    break;
                }
            }
        }

        #endregion
    }
}
