namespace Ecs
{
    /// <summary>
    /// 系统接口，作为所有ECS系统的基接口
    /// </summary>
    public interface ISystem
    {
    }

    /// <summary>
    /// 更新系统接口，继承自ISystem接口
    /// 用于处理游戏逻辑的每帧更新操作
    /// </summary>
    public interface IUpdateSystem : ISystem
    {
        /// <summary>
        /// 每帧更新时调用的方法
        /// </summary>
        /// <param name="entity">需要更新的实体ID</param>
        void OnUpdate(int entity);
    }

    /// <summary>
    /// 固定更新系统接口，继承自ISystem接口
    /// 用于处理物理模拟等需要固定时间间隔的更新操作
    /// </summary>
    public interface IFixedUpdateSystem : ISystem
    {
        /// <summary>
        /// 固定时间间隔更新时调用的方法
        /// </summary>
        /// <param name="entity">需要更新的实体ID</param>
        void OnFixedUpdate(int entity);
    }

    /// <summary>
    /// 延迟更新系统接口，继承自ISystem接口
    /// 用于处理在所有Update执行完成后的延迟更新操作
    /// </summary>
    public interface ILateUpdateSystem : ISystem
    {
        /// <summary>
        /// 延迟更新时调用的方法
        /// </summary>
        /// <param name="entity">需要更新的实体ID</param>
        void OnLateUpdate(int entity);
    }
}
