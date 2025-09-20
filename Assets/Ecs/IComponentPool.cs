using System;

namespace Ecs
{
    public interface IComponentPool
    {
        void AllocateComponent();

        void RemoveComponent(int entity);
    }
    
    public class ComponentPool<T> : IComponentPool where T : struct
    {
        private Component[] components = new Component[256];
        private int size;

        void IComponentPool.AllocateComponent()
        {
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

        public ref T GetComponent(int entity) //Index
        {
            ref var component = ref this.components[entity];
            return ref component.value;
        }

        public void SetComponent(int entity, ref T data)
        {
            ref var component = ref this.components[entity];
            component.exists = true;
            component.value = data;
        }

        public bool HasComponent(int entity)
        {
            return this.components[entity].exists;
        }

        public void RemoveComponent(int entity)
        {
            ref var component = ref this.components[entity];
            component.exists = false;
        }

        private struct Component
        {
            public bool exists;
            public T value;
        }
    }
}