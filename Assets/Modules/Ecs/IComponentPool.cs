using System;

namespace Ecs
{
    /// <summary>
    /// 组件池接口，定义了组件池的基本操作
    /// </summary>
    public interface IComponentPool
    {
        /// <summary>
        /// 分配一个新的组件空间
        /// </summary>
        void AllocateComponent();

        /// <summary>
        /// 移除指定实体的组件
        /// </summary>
        /// <param name="entity">实体的索引</param>
        void RemoveComponent(int entity);
    }
    
    /// <summary>
    /// 泛型组件池类，用于管理特定类型的组件
    /// </summary>
    /// <typeparam name="T">组件的类型，必须是值类型</typeparam>
    public class ComponentPool<T> : IComponentPool where T : struct
    {
        private Component[] components = new Component[256];
        private int size;

        /// <summary>
        /// 实现接口的组件分配方法，扩展组件数组并添加新的组件占位符
        /// </summary>
        void IComponentPool.AllocateComponent()
        {
            // 当组件数组空间不足时，扩容为原来的两倍
            if (this.size + 1 >= this.components.Length)
            {
                Array.Resize(ref this.components, this.components.Length * 2);
            }

            this.components[this.size] = new Component
            {
                exists = false,
                value = default
            };
            
            this.size++;
        }

        /// <summary>
        /// 获取指定实体的组件引用
        /// </summary>
        /// <param name="entity">实体的索引</param>
        /// <returns>指定实体组件的引用</returns>
        public ref T GetComponent(int entity) //Index
        {
            ref var component = ref this.components[entity];
            return ref component.value;
        }

        /// <summary>
        /// 设置指定实体的组件数据
        /// </summary>
        /// <param name="entity">实体的索引</param>
        /// <param name="data">要设置的组件数据的引用</param>
        public void SetComponent(int entity, ref T data)
        {
            ref var component = ref this.components[entity];
            component.exists = true;
            component.value = data;
        }

        /// <summary>
        /// 检查指定实体是否具有组件
        /// </summary>
        /// <param name="entity">实体的索引</param>
        /// <returns>如果实体具有组件则返回true，否则返回false</returns>
        public bool HasComponent(int entity)
        {
            return this.components[entity].exists;
        }

        /// <summary>
        /// 移除指定实体的组件
        /// </summary>
        /// <param name="entity">实体的索引</param>
        public void RemoveComponent(int entity)
        {
            ref var component = ref this.components[entity];
            component.exists = false;
        }

        /// <summary>
        /// 组件结构体，包含组件的存在状态和实际数据
        /// </summary>
        private struct Component
        {
            public bool exists;
            public T value;
        }
    }
}
