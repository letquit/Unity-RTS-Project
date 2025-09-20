using System;
using System.Collections.Generic;
using System.Reflection;

namespace Ecs
{
    public sealed class EcsWorld
    {
        private readonly List<ISystem> systems = new();
        private readonly List<IUpdateSystem> updateSystems = new();
        private readonly List<IFixedUpdateSystem> fixedUpdateSystems = new();
        private readonly List<ILateUpdateSystem> lateUpdateSystems = new();

        private readonly Dictionary<Type, IComponentPool> componentPools = new();
        private readonly List<bool> entities = new();

        public int CreateEntity()
        {
            var id = 0;
            var count = this.entities.Count;

            for (; id < count; id++)
            {
                if (!this.entities[id])
                {
                    this.entities[id] = true;
                    return id;
                }
            }

            id = count;
            this.entities.Add(true);

            foreach (var pool in this.componentPools.Values)
            {
                pool.AllocateComponent();
            }

            return id;
        }

        public void DestroyEntity(int entity)
        {
            this.entities[entity] = false;
            foreach (var pool in this.componentPools.Values)
            {
                pool.RemoveComponent(entity);
            }
        }

        public ref T GetComponent<T>(int entity) where T : struct
        {
            var pool = (ComponentPool<T>) this.componentPools[typeof(T)];
            return ref pool.GetComponent(entity);
        }

        public void SetComponent<T>(int entity, ref T component) where T : struct
        {
            var pool = (ComponentPool<T>) this.componentPools[typeof(T)];
            pool.SetComponent(entity, ref component);
        }

        public void Update()
        {
            for (int i = 0, count = this.updateSystems.Count; i < count; i++)
            {
                var system = this.updateSystems[i];
                for (var entity = 0; entity < this.entities.Count; entity++)
                {
                    if (this.entities[entity])
                    {
                        system.OnUpdate(entity);
                    }
                }
            }
        }

        public void FixedUpdate()
        {
            for (int i = 0, count = this.fixedUpdateSystems.Count; i < count; i++)
            {
                var system = this.fixedUpdateSystems[i];
                for (var entity = 0; entity < this.entities.Count; entity++)
                {
                    if (this.entities[entity])
                    {
                        system.OnFixedUpdate(entity);
                    }
                }
            }
        }

        public void LateUpdate()
        {
            for (int i = 0, count = this.lateUpdateSystems.Count; i < count; i++)
            {
                var system = this.lateUpdateSystems[i];
                for (var entity = 0; entity < this.entities.Count; entity++)
                {
                    if (this.entities[entity])
                    {
                        system.OnLateUpdate(entity);
                    }
                }
            }
        }

        public void BindComponent<T>() where T : struct
        {
            this.componentPools[typeof(T)] = new ComponentPool<T>();
        }

        public void BindSystem<T>() where T : ISystem, new()
        {
            var system = new T();
            this.systems.Add(system);

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

        public void Install()
        {
            foreach (var system in this.systems)
            {
                Type systemType = system.GetType();
                var fields = systemType.GetFields(
                    BindingFlags.Instance |
                    BindingFlags.DeclaredOnly |
                    BindingFlags.NonPublic
                );
                foreach (var field in fields)
                {
                    var componentType = field.FieldType.GenericTypeArguments[0];
                    var componentPool = this.componentPools[componentType];
                    field.SetValue(system, componentPool);
                }
            }
        }
    }
}