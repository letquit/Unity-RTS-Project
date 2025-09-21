using System;
using System.Collections;
using Ecs;
using UnityEngine;
using UnityEngine.AI;

namespace SampleProject
{
    /// <summary>
    /// 控制单位在NavMesh上的移动行为，包括路径规划、避障和路径修正功能。
    /// </summary>
    [RequireComponent(typeof(Entity))]
    public sealed class MoveAgent : MonoBehaviour
    {
        private const float STOPPING_DISTANCE_SQR = 0.2f;
        private const float COMPLETE_DELAY = 0.1f;
        private const float CORRECT_PATH_PERIOD = 0.75f;
        private const float SHIFT_OFFSET = 2.0f;
        private const float SHIFT_FACTOR = 0.75f;

        private Entity unit;
        private Vector3 destination;
        private NavMeshPath navMeshPath;

        private Coroutine moveCoroutine;
        private Coroutine completeCoroutine;
        private Coroutine checkObstacleCoroutine;
        private Coroutine avoidObstacleCoroutine;

        private Vector3[] pointPath;
        private int pointer;
        private bool isCompleted;
        private float correctPathTime;

        /// <summary>
        /// 获取移动是否已完成。
        /// </summary>
        public bool IsCompleted
        {
            get { return this.isCompleted; }
        }

        /// <summary>
        /// 获取是否正在进行避障。
        /// </summary>
        public bool IsObstacleAvoid
        {
            get { return this.avoidObstacleCoroutine != null; }
        }

        /// <summary>
        /// 获取是否可以进行路径修正。
        /// </summary>
        public bool CanCorrectPath
        {
            get { return Time.time - this.correctPathTime >= CORRECT_PATH_PERIOD; }
        }

        /// <summary>
        /// 获取是否已到达路径的最后一个点。
        /// </summary>
        public bool IsLastPoint
        {
            get { return this.pointer >= this.pointPath.Length - 1; }
        }

        private void Awake()
        {
            this.unit = this.GetComponent<Entity>();
            this.navMeshPath = new NavMeshPath();
        }

        #region Move

        /// <summary>
        /// 移动到指定位置。
        /// </summary>
        /// <param name="destination">目标位置。</param>
        public void MoveToPosition(Vector3 destination)
        {
            this.StopMove(isCompleted: false);
            this.StartMove(destination);
        }

        /// <summary>
        /// 开始移动到指定位置。
        /// </summary>
        /// <param name="destination">目标位置。</param>
        private void StartMove(Vector3 destination)
        {
            this.destination = destination;

            // 确保目标位置在NavMesh上
            if (NavMesh.SamplePosition(destination, out var hit, 2.0f, NavMesh.AllAreas))
            {
                this.destination = hit.position;
            }

            var pathGenerated = NavMesh.CalculatePath(
                this.transform.position,
                this.destination,
                NavMesh.AllAreas,
                this.navMeshPath
            );

            if (!pathGenerated || this.navMeshPath.status != NavMeshPathStatus.PathComplete)
            {
                Debug.LogWarning($"无法生成完整路径到目标位置: {destination}");
                return;
            }

            this.pointer = 0;
            this.pointPath = this.navMeshPath.corners;

            this.moveCoroutine = this.StartCoroutine(this.MoveRoutine());
            this.checkObstacleCoroutine = this.StartCoroutine(this.CheckObstacleRoutine());
        }

        /// <summary>
        /// 移动协程，按路径点依次移动。
        /// </summary>
        private IEnumerator MoveRoutine()
        {
            var framePeriod = new WaitForFixedUpdate();

            while (this.pointer < this.pointPath.Length)
            {
                yield return framePeriod;

                if (this.IsObstacleAvoid)
                {
                    continue;
                }

                this.MoveByPath();
            }

            this.StopMove(isCompleted: true);
        }

        /// <summary>
        /// 根据路径点移动单位。
        /// </summary>
        private void MoveByPath()
        {
            var currentPosiiton = this.transform.position;
            var targetPosition = this.pointPath[this.pointer];
            var distanceVector = targetPosition - currentPosiiton;

            var isTargetReached = distanceVector.sqrMagnitude <= STOPPING_DISTANCE_SQR;
            if (isTargetReached)
            {
                this.pointer++;
                return;
            }

            var direction = distanceVector.normalized;
            this.MoveUnit(direction);
        }

        /// <summary>
        /// 移动单位。
        /// </summary>
        /// <param name="direction">移动方向。</param>
        private void MoveUnit(Vector3 direction)
        {
            this.unit.SetData(new MoveStateComponent
            {
                moveRequired = true,
                direction = direction
            });
        }

        #endregion

        #region CorrectPath

        /// <summary>
        /// 修正当前路径。
        /// </summary>
        public void CorrectPath()
        {
            if (!this.CanCorrectPath || this.IsLastPoint)
            {
                return;
            }

            this.correctPathTime = Time.time;

            var currentPosition = this.transform.position;
            var targetPosition = this.pointPath[this.pointer];
            var nextPosition = this.pointPath[this.pointer + 1];

            var line = nextPosition - currentPosition;

            var isRight = Algorithms.PointRelativeToVector(currentPosition, nextPosition, targetPosition) > 0;
            var crossVector = isRight ? Vector3.up : Vector3.down;
            var shiftOffset = Vector3.Cross(line.normalized, crossVector) * SHIFT_OFFSET;

            var newPosition = Vector3.Lerp(
                currentPosition + shiftOffset,
                nextPosition - shiftOffset,
                SHIFT_FACTOR
            );

            if (NavMesh.SamplePosition(newPosition, out var hit, 2.0f, NavMesh.AllAreas))
            {
                newPosition = hit.position;
            }

            var pathGenerated = NavMesh.CalculatePath(
                newPosition,
                this.destination,
                NavMesh.AllAreas,
                this.navMeshPath
            );

            if (!pathGenerated || this.navMeshPath.status != NavMeshPathStatus.PathComplete)
            {
                this.pointPath[this.pointer] = newPosition;
                return;
            }

            this.pointer = 0;
            this.pointPath = this.navMeshPath.corners;
        }

        /// <summary>
        /// 尝试获取下一个路径点。
        /// </summary>
        /// <param name="targetPosition">输出下一个路径点的位置。</param>
        /// <returns>如果成功获取下一个路径点则返回true，否则返回false。</returns>
        public bool TryGetNextPosition(out Vector3 targetPosition)
        {
            var lastPoint = this.pointer >= this.pointPath.Length - 1;
            if (lastPoint)
            {
                targetPosition = default;
                return false;
            }

            targetPosition = this.pointPath[this.pointer];
            return true;
        }

        #endregion

        #region ObstacleAvoidance

        /// <summary>
        /// 开始避障。
        /// </summary>
        public void StartAvoidObstacle()
        {
            if (this.avoidObstacleCoroutine == null)
            {
                var avoidDirection = Vector3.Cross(this.transform.forward, Vector3.up);
                this.avoidObstacleCoroutine = this.StartCoroutine(this.AvoidObstacleRoutine(avoidDirection));
            }
        }

        /// <summary>
        /// 开始避障。
        /// </summary>
        /// <param name="avoidDirection">避障方向。</param>
        private void StartAvoidObstacle(Vector3 avoidDirection)
        {
            if (this.avoidObstacleCoroutine == null)
            {
                this.avoidObstacleCoroutine = this.StartCoroutine(this.AvoidObstacleRoutine(avoidDirection));
            }
        }

        /// <summary>
        /// 停止避障。
        /// </summary>
        private void StopAvoidObstacle()
        {
            if (this.avoidObstacleCoroutine != null)
            {
                this.StopCoroutine(this.avoidObstacleCoroutine);
                this.avoidObstacleCoroutine = null;
            }
        }

        /// <summary>
        /// 避障协程，持续向指定方向移动。
        /// </summary>
        /// <param name="moveDirection">移动方向。</param>
        private IEnumerator AvoidObstacleRoutine(Vector3 moveDirection)
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();
                this.unit.SetData(new MoveStateComponent
                {
                    moveRequired = true,
                    direction = moveDirection
                });
            }
        }

        /// <summary>
        /// 检查障碍物协程，定期检测前方是否有障碍物并启动避障。
        /// </summary>
        private IEnumerator CheckObstacleRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.35f);

                var currentPosition = this.transform.position;
                var targetPosition = this.pointPath[this.pointer];
                var direction = (targetPosition - currentPosition).normalized;

                var ray = new Ray(currentPosition, direction);
                // 增加射线检测距离，提高障碍物检测灵敏度
                if (!Physics.Raycast(ray, out var hit, 1.0f, LayerMask.GetMask("Obstacle")))
                {
                    this.StopAvoidObstacle();
                }
                else
                {
                    var avoidDirection = Vector3.Cross(hit.normal, Vector3.up);
                    this.StartAvoidObstacle(avoidDirection);
                }
            }
        }

        #endregion

        #region Stop

        /// <summary>
        /// 完成移动。
        /// </summary>
        public void CompleteMove()
        {
            if (this.isCompleted || this.completeCoroutine != null)
            {
                return;
            }

            this.completeCoroutine = this.StartCoroutine(this.CompleteDelayed());
        }

        /// <summary>
        /// 延迟完成移动。
        /// </summary>
        private IEnumerator CompleteDelayed()
        {
            yield return new WaitForSeconds(COMPLETE_DELAY);
            this.StopMove(isCompleted: true);
        }

        /// <summary>
        /// 停止移动。
        /// </summary>
        /// <param name="isCompleted">是否标记为已完成。</param>
        private void StopMove(bool isCompleted)
        {
            if (this.moveCoroutine != null)
            {
                this.StopCoroutine(this.moveCoroutine);
                this.moveCoroutine = null;
            }

            if (this.checkObstacleCoroutine != null)
            {
                this.StopCoroutine(this.checkObstacleCoroutine);
                this.checkObstacleCoroutine = null;
            }

            if (this.completeCoroutine != null)
            {
                this.StopCoroutine(this.completeCoroutine);
                this.completeCoroutine = null;
            }

            if (this.avoidObstacleCoroutine != null)
            {
                this.StopCoroutine(this.avoidObstacleCoroutine);
                this.avoidObstacleCoroutine = null;
            }

            this.isCompleted = isCompleted;
        }

        #endregion

        #region Editor

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            try
            {
                this.DrawMovingPath();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 绘制移动路径。
        /// </summary>
        private void DrawMovingPath()
        {
            Gizmos.color = Color.magenta;

            var current = this.transform.position;
            for (int i = this.pointer; i < this.pointPath.Length; i++)
            {
                Gizmos.DrawLine(current, this.pointPath[i]);
                current = this.pointPath[i];
            }
        }
#endif

        #endregion
    }
}
