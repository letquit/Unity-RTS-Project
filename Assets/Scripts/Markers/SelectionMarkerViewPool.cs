using System.Collections.Generic;
using UnityEngine;

namespace SampleProject
{
    /// <summary>
    /// 选择标记视图对象池管理器
    /// 用于管理和复用SelectionMarkerView对象，避免频繁的创建和销毁操作
    /// </summary>
    public sealed class SelectionMarkerViewPool : MonoBehaviour
    {
        [SerializeField]
        private SelectionMarkerView prefab;

        [SerializeField]
        private Transform activeContainer;

        [SerializeField]
        private Transform inactiveContainer;

        [SerializeField]
        private int initialSize = 32;

        private readonly Queue<SelectionMarkerView> markers = new();

        /// <summary>
        /// 初始化对象池，在Awake阶段创建指定数量的预制体实例
        /// </summary>
        private void Awake()
        {
            // 预先创建指定数量的对象实例并放入对象池中
            for (int i = 0; i < this.initialSize; i++)
            {
                var marker = Instantiate(this.prefab, this.inactiveContainer);
                this.markers.Enqueue(marker);
            }
        }

        /// <summary>
        /// 从对象池中获取一个SelectionMarkerView实例
        /// 如果对象池中有可用对象则直接返回，否则创建新实例
        /// </summary>
        /// <returns>SelectionMarkerView实例</returns>
        public SelectionMarkerView Get()
        {
            if (this.markers.TryDequeue(out var marker))
            {
                marker.transform.SetParent(this.activeContainer);
                return marker;
            }

            return Instantiate(this.prefab, this.activeContainer);
        }

        /// <summary>
        /// 将SelectionMarkerView实例回收到对象池中
        /// </summary>
        /// <param name="marker">要回收的SelectionMarkerView实例</param>
        public void Release(SelectionMarkerView marker)
        {
            marker.transform.SetParent(this.inactiveContainer);
            this.markers.Enqueue(marker);
        }
    }
}
