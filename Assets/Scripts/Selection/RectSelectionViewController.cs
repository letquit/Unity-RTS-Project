using SampleProject;
using UnityEngine;

namespace Selection
{
    /// <summary>
    /// 矩形选择视图控制器，负责控制矩形选择区域的显示和隐藏
    /// </summary>
    public sealed class RectSelectionViewController : MonoBehaviour
    {
        [SerializeField] private RectSelectionInput input;
        [SerializeField] private RectSelectionView view;
        [SerializeField] private UIService uiService;
        
        /// <summary>
        /// 每帧更新矩形选择视图的显示状态和位置
        /// </summary>
        private void Update()
        {
            // 根据输入状态控制矩形选择区域的显示
            if (input.IsSelecting)
            {
                // 计算并设置矩形选择区域的位置和大小
                Rect rect = this.uiService.GetUIRectByScreenPoints(this.input.StartPoint, this.input.EndPoint);
                
                view.SetPositions(rect);
                view.SetVisible(true);
            }
            else
            {
                view.SetVisible(false);
            }
        }

    }
}
