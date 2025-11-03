public interface IFsmState
{
    StatusType Type { get; }

    void Enter(params object[] objs);

    void Update(float deltaTime);
    void LateUpdate(float deltaTime);
    void Exit();
}