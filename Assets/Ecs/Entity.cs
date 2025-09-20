using UnityEngine;

namespace Ecs
{
    public class Entity : MonoBehaviour
    {
        public int Id
        {
            get { return this.id; }
        }

        private int id;

        private EcsWorld ecsWorld;

        public void Init(EcsWorld world)
        {
            this.id = world.CreateEntity();
            this.ecsWorld = world;
            this.OnInit();
        }

        protected virtual void OnInit()
        {
        }

        public void Dispose()
        {
            this.ecsWorld.DestroyEntity(this.id);
            this.ecsWorld = null;
            this.id = -1;
        }

        public void SetData<T>(T component) where T : struct
        {
            this.ecsWorld.SetComponent(this.id, ref component);
        }

        public ref T GetData<T>() where T : struct
        {
            return ref this.ecsWorld.GetComponent<T>(this.id);
        } 
    }
}