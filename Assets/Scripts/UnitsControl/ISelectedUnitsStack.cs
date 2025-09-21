using System;
using System.Collections.Generic;
using UnityEngine;

namespace SampleProject
{
    /// <summary>
    /// 选中单位栈接口，用于管理一组被选中的游戏对象单位
    /// </summary>
    public interface ISelectedUnitsStack
    {
        /// <summary>
        /// 当单位集合发生变化时触发的事件
        /// </summary>
        event Action<IEnumerable<GameObject>> OnUnitsChanged;

        /// <summary>
        /// 当有单位被添加时触发的事件
        /// </summary>
        event Action<GameObject> OnUnitAdded;

        /// <summary>
        /// 当有单位被移除时触发的事件
        /// </summary>
        event Action<GameObject> OnUnitRemoved;

        /// <summary>
        /// 当所有单位被清除时触发的事件
        /// </summary>
        event Action OnUnitsCleared;

        /// <summary>
        /// 获取当前所有选中的单位集合
        /// </summary>
        /// <returns>只读的单位集合</returns>
        IReadOnlyCollection<GameObject> GetUnits();

        /// <summary>
        /// 设置单位集合，替换当前所有选中的单位
        /// </summary>
        /// <param name="group">要设置的单位数组</param>
        void SetUnits(params GameObject[] group);

        /// <summary>
        /// 添加一个单位到选中集合中
        /// </summary>
        /// <param name="unit">要添加的单位游戏对象</param>
        void AddUnit(GameObject unit);

        /// <summary>
        /// 从选中集合中移除一个单位
        /// </summary>
        /// <param name="unit">要移除的单位游戏对象</param>
        void RemoveUnit(GameObject unit);

        /// <summary>
        /// 清除所有选中的单位
        /// </summary>
        void ClearUnits();
    }
}
