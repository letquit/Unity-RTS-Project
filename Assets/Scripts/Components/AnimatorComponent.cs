using System;
using UnityEngine;

namespace SampleProject
{
    /// <summary>
    /// 可序列化的动画控制器组件结构体，用于在Unity中存储和引用Animator组件的引用
    /// </summary>
    [Serializable]
    public struct AnimatorComponent
    {
        /// <summary>
        /// Animator组件的引用值
        /// </summary>
        public Animator value;
    }
}
