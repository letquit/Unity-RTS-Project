using System;
using UnityEngine;

namespace SampleProject
{
    /// <summary>
    /// 可序列化的变换组件结构体，用于包装Unity的Transform组件
    /// </summary>
    [Serializable]
    public struct TransformComponent
    {
        /// <summary>
        /// Transform组件的引用值
        /// </summary>
        public Transform value;
    }
}
