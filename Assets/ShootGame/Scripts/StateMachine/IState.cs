//状态机状态接口
public interface IState
{
    void OnEnter();

    void OnUpdate();

    void OnExit();
}
