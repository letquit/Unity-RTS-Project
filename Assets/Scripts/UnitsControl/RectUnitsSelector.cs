using System.Collections.Generic;
using System.Linq;
using Ecs;
using UnityEngine;

namespace SampleProject
{
    /// <summary>
    /// 矩形区域单位选择器，用于在3D场景中通过矩形框选的方式选择单位
    /// </summary>
    public sealed class RectUnitsSelector : MonoBehaviour
    {
        [SerializeField]
        private SelectedUnitsStack stack;

        private float minX;
        private float maxX;
        private float minZ;
        private float maxZ;

        /// <summary>
        /// 根据起始和结束世界坐标点选择单位
        /// </summary>
        /// <param name="startWorldPoint">选择矩形的起始世界坐标点</param>
        /// <param name="endWorldPoint">选择矩形的结束世界坐标点</param>
        public void SelectUnits(Vector3 startWorldPoint, Vector3 endWorldPoint)
        {
            // 缓存选择区域的边界坐标
            this.CacheBounds(startWorldPoint, endWorldPoint);

            var allUnits = this.GetAllUnits();
            var selectedUnits = FilterUnits(allUnits);
            this.stack.SetUnits(selectedUnits.ToArray());
        }

        /// <summary>
        /// 缓存选择矩形区域的边界坐标
        /// </summary>
        /// <param name="startWorldPoint">起始世界坐标点</param>
        /// <param name="endWorldPoint">结束世界坐标点</param>
        private void CacheBounds(Vector3 startWorldPoint, Vector3 endWorldPoint)
        {
            minX = Mathf.Min(startWorldPoint.x, endWorldPoint.x);
            maxX = Mathf.Max(startWorldPoint.x, endWorldPoint.x);
            minZ = Mathf.Min(startWorldPoint.z, endWorldPoint.z);
            maxZ = Mathf.Max(startWorldPoint.z, endWorldPoint.z);
        }

        /// <summary>
        /// 获取场景中所有的单位实体
        /// </summary>
        /// <returns>所有单位实体的游戏对象集合</returns>
        private IEnumerable<GameObject> GetAllUnits()
        {
            return FindObjectsOfType<Entity>().Select(it => it.gameObject);
        }

        /// <summary>
        /// 过滤出在选择区域内的单位
        /// </summary>
        /// <param name="allUnits">所有单位的集合</param>
        /// <returns>在选择区域内的单位列表</returns>
        private List<GameObject> FilterUnits(IEnumerable<GameObject> allUnits)
        {
            List<GameObject> selectedUnits = new List<GameObject>();

            // 遍历所有单位，检查是否在选择区域内
            foreach (var unit in allUnits)
            {
                Vector3 position = unit.transform.position;
                // 首先检查单位的位置点是否在区域内
                if (IsPointInside(position))
                {
                    selectedUnits.Add(unit);
                    continue;
                }

                // 如果位置点不在区域内，则检查单位碰撞体的边界点
                var collider = unit.GetComponent<Collider>();
                if (IsPointInside(collider.bounds.min) || IsPointInside(collider.bounds.max))
                {
                    selectedUnits.Add(unit);
                }
            }

            return selectedUnits;
        }

        /// <summary>
        /// 判断指定点是否在选择的矩形区域内
        /// </summary>
        /// <param name="point">要检查的点坐标</param>
        /// <returns>如果点在矩形区域内返回true，否则返回false</returns>
        private bool IsPointInside(Vector3 point)
        {
            float x = point.x;
            float z = point.z;
            return x >= minX && x <= maxX && z >= minZ && z <= maxZ;
        }
    }
}
