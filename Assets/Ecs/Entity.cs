using UnityEngine;

namespace Ecs
{
    /// <summary>
    /// 实体类，继承自MonoBehaviour，用于在ECS系统中表示一个实体对象
    /// </summary>
    public class Entity : MonoBehaviour
    {
        /// <summary>
        /// 获取实体的唯一标识符
        /// </summary>
        public int Id
        {
            get { return this.id; }
        }

        private int id;

        private EcsWorld ecsWorld;

        /// <summary>
        /// 初始化实体，设置实体ID并关联到指定的ECS世界
        /// </summary>
        /// <param name="world">实体所属的ECS世界实例</param>
        public void Init(EcsWorld world)
        {
            this.id = world.CreateEntity();
            this.ecsWorld = world;
            this.OnInit();
        }

        /// <summary>
        /// 初始化回调方法，可在子类中重写以执行特定的初始化逻辑
        /// </summary>
        protected virtual void OnInit()
        {
        }

        /// <summary>
        /// 销毁实体，从ECS世界中移除该实体并清理相关引用
        /// </summary>
        public void Dispose()
        {
            this.ecsWorld.DestroyEntity(this.id);
            this.ecsWorld = null;
            this.id = -1;
        }

        /// <summary>
        /// 为实体设置组件数据
        /// </summary>
        /// <typeparam name="T">组件类型，必须是值类型</typeparam>
        /// <param name="component">要设置的组件数据</param>
        public void SetData<T>(T component) where T : struct
        {
            this.ecsWorld.SetComponent(this.id, ref component);
        }

        /// <summary>
        /// 获取实体的组件数据引用
        /// </summary>
        /// <typeparam name="T">组件类型，必须是值类型</typeparam>
        /// <returns>指定类型的组件数据引用</returns>
        public ref T GetData<T>() where T : struct
        {
            return ref this.ecsWorld.GetComponent<T>(this.id);
        } 
    }
}
