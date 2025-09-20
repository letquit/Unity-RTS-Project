using System;
using System.Collections.Generic;
using System.Reflection;

namespace Ecs
{
    /// <summary>
    /// 表示一个ECS世界，用于管理实体、组件和系统。
    /// </summary>
    public sealed class EcsWorld
    {
        private readonly List<ISystem> systems = new();
        private readonly List<IUpdateSystem> updateSystems = new();
        private readonly List<IFixedUpdateSystem> fixedUpdateSystems = new();
        private readonly List<ILateUpdateSystem> lateUpdateSystems = new();

        private readonly Dictionary<Type, IComponentPool> componentPools = new();
        private readonly List<bool> entities = new();

        /// <summary>
        /// 创建一个新的实体，并返回其ID。
        /// </summary>
        /// <returns>新创建实体的ID。</returns>
        public int CreateEntity()
        {
            var id = 0;
            var count = this.entities.Count;

            // 查找第一个未使用的实体ID
            for (; id < count; id++)
            {
                if (!this.entities[id])
                {
                    this.entities[id] = true;
                    return id;
                }
            }

            // 如果没有找到空闲ID，则添加新的实体
            id = count;
            this.entities.Add(true);

            // 为所有组件池分配新组件空间
            foreach (var pool in this.componentPools.Values)
            {
                pool.AllocateComponent();
            }

            return id;
        }

        /// <summary>
        /// 销毁指定ID的实体。
        /// </summary>
        /// <param name="entity">要销毁的实体ID。</param>
        public void DestroyEntity(int entity)
        {
            this.entities[entity] = false;
            // 从所有组件池中移除该实体对应的组件
            foreach (var pool in this.componentPools.Values)
            {
                pool.RemoveComponent(entity);
            }
        }

        /// <summary>
        /// 获取指定实体的指定类型组件的引用。
        /// </summary>
        /// <typeparam name="T">组件类型。</typeparam>
        /// <param name="entity">实体ID。</param>
        /// <returns>组件的引用。</returns>
        public ref T GetComponent<T>(int entity) where T : struct
        {
            var pool = (ComponentPool<T>) this.componentPools[typeof(T)];
            return ref pool.GetComponent(entity);
        }

        /// <summary>
        /// 设置指定实体的指定类型组件。
        /// </summary>
        /// <typeparam name="T">组件类型。</typeparam>
        /// <param name="entity">实体ID。</param>
        /// <param name="component">要设置的组件值的引用。</param>
        public void SetComponent<T>(int entity, ref T component) where T : struct
        {
            var pool = (ComponentPool<T>) this.componentPools[typeof(T)];
            pool.SetComponent(entity, ref component);
        }

        /// <summary>
        /// 执行所有实现了IUpdateSystem接口的系统的更新逻辑。
        /// </summary>
        public void Update()
        {
            for (int i = 0, count = this.updateSystems.Count; i < count; i++)
            {
                var system = this.updateSystems[i];
                // 遍历所有实体并调用系统更新方法
                for (var entity = 0; entity < this.entities.Count; entity++)
                {
                    if (this.entities[entity])
                    {
                        system.OnUpdate(entity);
                    }
                }
            }
        }

        /// <summary>
        /// 执行所有实现了IFixedUpdateSystem接口的系统的固定更新逻辑。
        /// </summary>
        public void FixedUpdate()
        {
            for (int i = 0, count = this.fixedUpdateSystems.Count; i < count; i++)
            {
                var system = this.fixedUpdateSystems[i];
                // 遍历所有实体并调用系统固定更新方法
                for (var entity = 0; entity < this.entities.Count; entity++)
                {
                    if (this.entities[entity])
                    {
                        system.OnFixedUpdate(entity);
                    }
                }
            }
        }

        /// <summary>
        /// 执行所有实现了ILateUpdateSystem接口的系统的延迟更新逻辑。
        /// </summary>
        public void LateUpdate()
        {
            for (int i = 0, count = this.lateUpdateSystems.Count; i < count; i++)
            {
                var system = this.lateUpdateSystems[i];
                // 遍历所有实体并调用系统延迟更新方法
                for (var entity = 0; entity < this.entities.Count; entity++)
                {
                    if (this.entities[entity])
                    {
                        system.OnLateUpdate(entity);
                    }
                }
            }
        }

        /// <summary>
        /// 绑定一个组件类型到组件池。
        /// </summary>
        /// <typeparam name="T">要绑定的组件类型。</typeparam>
        public void BindComponent<T>() where T : struct
        {
            this.componentPools[typeof(T)] = new ComponentPool<T>();
        }

        /// <summary>
        /// 绑定一个系统类型，并根据其实现的接口将其添加到相应的系统列表中。
        /// </summary>
        /// <typeparam name="T">要绑定的系统类型。</typeparam>
        public void BindSystem<T>() where T : ISystem, new()
        {
            var system = new T();
            this.systems.Add(system);

            // 根据系统实现的接口将其添加到对应的系统列表中
            if (system is IUpdateSystem updateSystem)
            {
                this.updateSystems.Add(updateSystem);
            }

            if (system is IFixedUpdateSystem fixedUpdateSystem)
            {
                this.fixedUpdateSystems.Add(fixedUpdateSystem);
            }

            if (system is ILateUpdateSystem lateUpdateSystem)
            {
                this.lateUpdateSystems.Add(lateUpdateSystem);
            }
        }

        /// <summary>
        /// 安装所有系统，将组件池注入到系统中。
        /// </summary>
        public void Install()
        {
            foreach (var system in this.systems)
            {
                Type systemType = system.GetType();
                // 获取系统类中声明的所有私有实例字段
                var fields = systemType.GetFields(
                    BindingFlags.Instance |
                    BindingFlags.DeclaredOnly |
                    BindingFlags.NonPublic
                );
                foreach (var field in fields)
                {
                    // 获取字段的泛型参数类型（即组件类型）
                    var componentType = field.FieldType.GenericTypeArguments[0];
                    var componentPool = this.componentPools[componentType];
                    // 将组件池赋值给系统中的字段
                    field.SetValue(system, componentPool);
                }
            }
        }
    }
}
