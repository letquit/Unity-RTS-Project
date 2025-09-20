using System;

namespace SampleProject
{
    /// <summary>
    /// 表示移动速度的组件结构体，用于存储和传递移动速度数据
    /// </summary>
    [Serializable]
    public struct MoveSpeedComponent
    {
        /// <summary>
        /// 移动速度的数值
        /// </summary>
        public float value;
    }
}
