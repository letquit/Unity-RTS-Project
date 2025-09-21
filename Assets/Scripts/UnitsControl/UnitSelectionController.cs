using Selection;
using UnityEngine;

namespace SampleProject
{
    /// <summary>
    /// 单位选择控制器，负责处理鼠标框选单位的逻辑
    /// 监听鼠标输入事件，将屏幕坐标转换为世界坐标，并调用选择器选择范围内的单位
    /// </summary>
    public sealed class UnitSelectionController : MonoBehaviour
    {
        private static Plane GROUND_PLANE = new(Vector3.up, Vector3.zero);

        [SerializeField]
        private RectSelectionInput input;

        [SerializeField]
        private RectUnitsSelector unitsSelector;
        
        /// <summary>
        /// 当组件启用时注册事件监听器
        /// 将OnSelectionFinished方法注册到输入组件的OnFinished事件
        /// </summary>
        private void OnEnable()
        {
            this.input.OnFinished += this.OnSelectionFinished;
        }

        /// <summary>
        /// 当组件禁用时注销事件监听器
        /// 避免组件禁用后仍然响应事件导致错误
        /// </summary>
        private void OnDisable()
        {
            this.input.OnFinished -= this.OnSelectionFinished;
        }

        /// <summary>
        /// 处理选择完成事件
        /// 获取鼠标起始和结束点的屏幕坐标，转换为世界坐标，
        /// 扩展选择范围后调用选择器进行单位选择
        /// </summary>
        private void OnSelectionFinished()
        {
            Vector2 startScreenPoint = this.input.StartPoint;
            Vector2 endScreenPoint = this.input.EndPoint;
            
            // 获取地面位置时添加一些容差
            Vector3 startWorldPoint = this.GetGroundPosition(startScreenPoint);
            Vector3 endWorldPoint = this.GetGroundPosition(endScreenPoint);
            
            // 扩展选取范围，增加边界
            float expandFactor = 0.5f; // 可调整的扩展因子
            Vector3 expandedStart = startWorldPoint - new Vector3(expandFactor, 0, expandFactor);
            Vector3 expandedEnd = endWorldPoint + new Vector3(expandFactor, 0, expandFactor);
            
            this.unitsSelector.SelectUnits(expandedStart, expandedEnd);
        }
        
        /// <summary>
        /// 将屏幕坐标转换为地面上的世界坐标
        /// 使用主摄像机将屏幕点转换为射线，与地面平面求交点
        /// </summary>
        /// <param name="screenPoint">屏幕坐标点</param>
        /// <returns>地面上对应的世界坐标点</returns>
        private Vector3 GetGroundPosition(Vector2 screenPoint)
        {
            Ray ray = Camera.main.ScreenPointToRay(screenPoint);
            GROUND_PLANE.Raycast(ray, out var distance);
            Vector3 groundPoint = ray.GetPoint(distance);
            return groundPoint;
        }
    }
}

