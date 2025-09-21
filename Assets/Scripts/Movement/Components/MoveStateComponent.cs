using System;
using UnityEngine;

namespace SampleProject
{
    /// <summary>
    /// 移动状态组件，用于存储和序列化对象的移动状态信息
    /// </summary>
    [Serializable]
    public struct MoveStateComponent
    {
        /// <summary>
        /// 是否需要移动的标志位，默认为false
        /// </summary>
        public bool moveRequired; //False
        
        /// <summary>
        /// 移动方向向量，表示移动的方向和距离
        /// </summary>
        public Vector3 direction;
    }
}
