using UnityEngine;

namespace SampleProject
{
    /// <summary>
    /// 算法工具类，提供各种计算方法
    /// </summary>
    public class Algorithms
    {
        /// <summary>
        /// 计算点相对于向量的位置关系
        /// 通过计算向量叉积来判断点在向量的左侧、右侧或线上
        /// </summary>
        /// <param name="start">向量的起始点</param>
        /// <param name="end">向量的结束点</param>
        /// <param name="point">需要判断位置关系的点</param>
        /// <returns>
        /// 返回叉积计算结果：
        /// 大于0表示点在向量左侧，
        /// 小于0表示点在向量右侧，
        /// 等于0表示点在向量所在直线上
        /// </returns>
        public static float PointRelativeToVector(Vector3 start, Vector3 end, Vector3 point)
        {
            // 计算向量叉积的z分量，用于判断点相对于向量的位置
            return (point.x - start.x) * (end.z - start.z) -
                   (point.z - start.z) * (end.x - start.x);
        }
    }
}
