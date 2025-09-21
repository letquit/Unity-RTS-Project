using System;
using UnityEngine;

namespace SampleProject
{
    /// <summary>
    /// 矩形选择输入处理类，用于处理鼠标拖拽创建矩形选择区域的输入逻辑
    /// </summary>
    public sealed class RectSelectionInput : MonoBehaviour
    {
        /// <summary>
        /// 当开始矩形选择时触发的事件
        /// </summary>
        public event Action OnStarted;
        
        /// <summary>
        /// 当完成矩形选择时触发的事件
        /// </summary>
        public event Action OnFinished;
        
        private Vector2 startPoint;
        private Vector2 endPoint;
        private bool isSelecting;

        /// <summary>
        /// 获取矩形选择的起始点坐标
        /// </summary>
        public Vector2 StartPoint
        {
            get { return this.startPoint; }
        }

        /// <summary>
        /// 获取矩形选择的结束点坐标
        /// </summary>
        public Vector2 EndPoint
        {
            get { return this.endPoint; }
        }

        /// <summary>
        /// 获取当前是否正在进行矩形选择的状态
        /// </summary>
        public bool IsSelecting
        {
            get { return this.isSelecting; }
        }

        /// <summary>
        /// Unity Update生命周期函数，处理鼠标输入逻辑
        /// 检测鼠标左键的按下、按住和释放状态来控制矩形选择流程
        /// </summary>
        private void Update()
        {
            // 处理鼠标左键按下事件 - 开始选择
            if (Input.GetMouseButtonDown(0))
            {
                this.isSelecting = true;
                this.startPoint = Input.mousePosition;
                this.endPoint = this.startPoint;
                this.OnStarted?.Invoke();
            }
            // 处理鼠标左键按住事件 - 更新选择区域
            else if (Input.GetMouseButton(0))
            {
                this.endPoint = Input.mousePosition;
            }
            // 处理鼠标左键释放事件 - 完成选择
            else if (Input.GetMouseButtonUp(0))
            {
                this.isSelecting = false;
                this.endPoint = Input.mousePosition;
                this.OnFinished?.Invoke();
            }
        }

    }
}
