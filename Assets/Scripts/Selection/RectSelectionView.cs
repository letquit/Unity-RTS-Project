using System;
using UnityEngine;
using UnityEngine.UI;

namespace Selection
{
    /// <summary>
    /// 矩形选择视图组件，用于在UI中显示一个可控制的矩形选择区域
    /// 该组件需要RectTransform和Image组件支持
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Image))]
    public sealed class RectSelectionView : MonoBehaviour
    {
        private RectTransform rectTransform;
        private Image image;

        /// <summary>
        /// 组件初始化方法，在Awake阶段获取必要的组件引用
        /// </summary>
        private void Awake()
        {
            this.rectTransform = this.GetComponent<RectTransform>();
            this.image = this.GetComponent<Image>();
        }

        /// <summary>
        /// 设置矩形选择区域的位置和大小
        /// </summary>
        /// <param name="rect">包含位置和大小信息的矩形对象</param>
        public void SetPositions(Rect rect)
        {
            this.rectTransform.sizeDelta = rect.size;
            this.rectTransform.anchoredPosition = new Vector2(rect.x, rect.y);
        }

        /// <summary>
        /// 控制矩形选择区域的显示/隐藏状态
        /// </summary>
        /// <param name="isVisible">true表示显示，false表示隐藏</param>
        public void SetVisible(bool isVisible)
        {
            this.image.enabled = isVisible;
        }

    }
}
