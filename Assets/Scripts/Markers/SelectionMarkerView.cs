using UnityEngine;

namespace SampleProject
{
    /// <summary>
    /// 选择标记视图类，用于在场景中显示选择标记的位置
    /// </summary>
    public sealed class SelectionMarkerView : MonoBehaviour
    {
        /// <summary>
        /// 设置选择标记的位置
        /// </summary>
        /// <param name="position">要设置的位置坐标</param>
        public void SetPosition(Vector3 position)
        {
            this.transform.position = position;
        }
    }
}
