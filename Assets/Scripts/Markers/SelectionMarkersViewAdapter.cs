using System.Collections.Generic;
using UnityEngine;

namespace SampleProject
{
    /// <summary>
    /// 选择标记视图适配器类，用于管理单位选择时的标记显示
    /// 监听单位栈的变化，动态创建和销毁选择标记
    /// </summary>
    public sealed class SelectionMarkersViewAdapter : MonoBehaviour
    {
        [SerializeField]
        private SelectionMarkerViewPool pool;
        
        [Header("Stack")]
        [SerializeField]
        private SelectedUnitsStack stack;

        private readonly Dictionary<GameObject, SelectionMarkerView> markers = new();

        /// <summary>
        /// 当组件启用时注册单位变化事件监听器
        /// </summary>
        private void OnEnable()
        {
            this.stack.OnUnitsChanged += this.OnUnitsChanged;
        }

        /// <summary>
        /// 每帧更新所有标记的位置，使其跟随对应单位的位置变化
        /// </summary>
        private void Update()
        {
            foreach (var (unit, marker) in this.markers)
            {
                marker.SetPosition(unit.transform.position);
            }
        }

        /// <summary>
        /// 当组件禁用时注销单位变化事件监听器
        /// </summary>
        private void OnDisable()
        {
            this.stack.OnUnitsChanged -= this.OnUnitsChanged;
        }

        /// <summary>
        /// 响应单位栈变化事件，重新创建所有单位的标记
        /// </summary>
        /// <param name="units">当前选中的单位集合</param>
        private void OnUnitsChanged(IEnumerable<GameObject> units)
        {
            this.DestroyMarkers();
            this.SpawnMarkers(units);
        }

        /// <summary>
        /// 为指定的单位集合创建选择标记
        /// </summary>
        /// <param name="units">需要创建标记的单位集合</param>
        private void SpawnMarkers(IEnumerable<GameObject> units)
        {
            foreach (var unit in units)
            {
                var marker = this.pool.Get();
                marker.SetPosition(unit.transform.position);
                this.markers.Add(unit, marker);
            }
        }

        /// <summary>
        /// 销毁所有当前存在的标记并清空标记字典
        /// </summary>
        private void DestroyMarkers()
        {
            foreach (var marker in this.markers.Values)
            {
                this.pool.Release(marker);
            }
            
            this.markers.Clear();
        }
    }
}
