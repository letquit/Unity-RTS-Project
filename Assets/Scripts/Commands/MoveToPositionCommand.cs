using System;
using UnityEngine;

namespace SampleProject
{
    /// <summary>
    /// 移动到指定位置的命令结构体
    /// 用于存储和传递移动目标位置信息
    /// </summary>
    [Serializable]
    public struct MoveToPositionCommand
    {
        /// <summary>
        /// 移动的目标位置坐标
        /// </summary>
        public Vector3 destination;
    }
}
