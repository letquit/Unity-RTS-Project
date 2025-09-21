using UnityEngine;

namespace SampleProject
{
    /// <summary>
    /// 控制选中单位移动的控制器类
    /// 监听鼠标右键点击事件，当点击地面时指挥选中的单位移动到指定位置
    /// </summary>
    public sealed class SelectedUnitsMoveController : MonoBehaviour
    {
        [SerializeField]
        private ObjectsMover objectsMover;

        [SerializeField]
        private SelectedUnitsStack stack;
        
        /// <summary>
        /// 每帧更新检测鼠标右键点击事件
        /// 当检测到鼠标右键点击时，发射射线检测点击位置，如果点击到地面标签的对象，则指挥选中单位移动到该位置
        /// </summary>
        private void Update()
        {
            // 检测鼠标右键是否按下
            if (!Input.GetMouseButtonDown(1))
            {
                return;
            }

            // 从摄像机发射射线到鼠标位置
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            // 检测射线是否击中物体
            if (!Physics.Raycast(ray, out var hit))
            {
                return;
            }

            // 判断击中的物体是否为地面
            if (hit.transform.CompareTag("Ground"))
            {
                this.MoveToPosition(hit.point);
            }
        }

        /// <summary>
        /// 将选中的单位移动到指定位置
        /// </summary>
        /// <param name="destination">目标移动位置</param>
        private void MoveToPosition(Vector3 destination)
        {
            var units = this.stack.GetUnits();
            this.objectsMover.MoveObjects(units, destination);
        }
    }
}
