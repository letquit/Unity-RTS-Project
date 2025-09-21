using UnityEngine;

namespace SampleProject
{
    /// <summary>
    /// UI服务类，提供屏幕坐标到UI坐标的转换功能
    /// </summary>
    public sealed class UIService : MonoBehaviour
    {
        [SerializeField]
        private Canvas canvas;

        [SerializeField]
        private new Camera camera;

        private RectTransform canvasTransform;

        private void Awake()
        {
            this.canvasTransform = this.canvas.GetComponent<RectTransform>();
        }

        /// <summary>
        /// 将屏幕坐标转换为UI局部坐标
        /// </summary>
        /// <param name="screenPoint">屏幕坐标点</param>
        /// <returns>对应的UI局部坐标点</returns>
        public Vector2 GetUIPointByScreenPoint(Vector2 screenPoint)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                this.canvasTransform,
                screenPoint,
                this.camera,
                out var result
            );

            return result;
        }

        /// <summary>
        /// 根据两个屏幕坐标点计算UI矩形区域
        /// </summary>
        /// <param name="startScreenPoint">起始屏幕坐标点</param>
        /// <param name="endScreenPoint">结束屏幕坐标点</param>
        /// <returns>包含两个点的UI矩形区域</returns>
        public Rect GetUIRectByScreenPoints(Vector2 startScreenPoint, Vector2 endScreenPoint)
        {
            // 转换屏幕坐标到UI坐标
            var startUIPoint = this.GetUIPointByScreenPoint(startScreenPoint);
            var endUIPoint = this.GetUIPointByScreenPoint(endScreenPoint);
            
            // 计算矩形中心点和尺寸
            Vector2 center = (startUIPoint + endUIPoint) / 2;
            Vector2 vector = endUIPoint - startUIPoint;
            Vector2 size = new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
            
            return new Rect(center, size);
        }

    }
}
