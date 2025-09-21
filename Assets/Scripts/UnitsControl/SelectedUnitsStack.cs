using System;
using System.Collections.Generic;
using UnityEngine;

namespace SampleProject
{
    /// <summary>
    /// 选中单位栈管理器，用于管理游戏场景中被选中的游戏对象集合
    /// 继承自MonoBehaviour，可作为Unity组件使用，实现ISelectedUnitsStack接口
    /// </summary>
    public sealed class SelectedUnitsStack : MonoBehaviour, ISelectedUnitsStack
    {
        /// <summary>
        /// 当选中单位集合发生变化时触发的事件
        /// </summary>
        public event Action<IEnumerable<GameObject>> OnUnitsChanged;
        
        /// <summary>
        /// 当有新单位被添加到选中集合时触发的事件
        /// </summary>
        public event Action<GameObject> OnUnitAdded;
        
        /// <summary>
        /// 当有单位从选中集合中被移除时触发的事件
        /// </summary>
        public event Action<GameObject> OnUnitRemoved;
        
        /// <summary>
        /// 当选中单位集合被清空时触发的事件
        /// </summary>
        public event Action OnUnitsCleared;

        /// <summary>
        /// 存储当前选中单位的HashSet集合，用于快速查找和去重
        /// </summary>
        private readonly HashSet<GameObject> selectedUnits = new();

        /// <summary>
        /// 获取当前所有选中的单位集合
        /// </summary>
        /// <returns>返回只读的GameObject集合，包含所有当前选中的单位</returns>
        public IReadOnlyCollection<GameObject> GetUnits()
        {
            return this.selectedUnits;
        }

        /// <summary>
        /// 设置新的选中单位组，会清空原有选中单位并添加新的单位组
        /// </summary>
        /// <param name="group">要设置为选中状态的GameObject数组</param>
        public void SetUnits(params GameObject[] group)
        {
            this.selectedUnits.Clear();
            this.selectedUnits.UnionWith(group);
            this.OnUnitsChanged?.Invoke(group);
        }

        /// <summary>
        /// 向选中单位集合中添加一个新的单位
        /// </summary>
        /// <param name="unit">要添加到选中集合的GameObject单位</param>
        public void AddUnit(GameObject unit)
        {
            // 只有当单位成功添加到集合中时（即之前不存在），才触发添加事件
            if (this.selectedUnits.Add(unit))
            {
                this.OnUnitAdded?.Invoke(unit);
            }
        }

        /// <summary>
        /// 从选中单位集合中移除指定的单位
        /// </summary>
        /// <param name="unit">要从选中集合中移除的GameObject单位</param>
        public void RemoveUnit(GameObject unit)
        {
            // 只有当单位成功从集合中移除时（即之前存在），才触发移除事件
            if (this.selectedUnits.Remove(unit))
            {
                this.OnUnitRemoved?.Invoke(unit);
            }
        }

        /// <summary>
        /// 清空所有选中的单位
        /// </summary>
        public void ClearUnits()
        {
            this.selectedUnits.Clear();
            this.OnUnitsCleared?.Invoke();
        }
    }
}
