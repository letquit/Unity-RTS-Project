namespace Ecs
{
    public interface ISystem
    {
    }

    public interface IUpdateSystem : ISystem
    {
        void OnUpdate(int entity);
    }

    public interface IFixedUpdateSystem : ISystem
    {
        void OnFixedUpdate(int entity);
    }

    public interface ILateUpdateSystem : ISystem
    {
        void OnLateUpdate(int entity);
    }
}