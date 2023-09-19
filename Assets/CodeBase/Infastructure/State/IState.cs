public interface IState : IExitableState
{
    void Enter();
}

public interface IExitableState
{
    void Exit();
}

public interface IPayloadedState<TPayLoad> : IExitableState
{
    void Enter(TPayLoad payLoad);
}