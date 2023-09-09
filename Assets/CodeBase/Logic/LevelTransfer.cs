using UnityEngine;

namespace CodeBase.Logic
{
  public class LevelTransfer : MonoBehaviour
  {
    public string TransferTo;
    private IGameStateMachine _stateMachine;

        void Start() => _stateMachine = AllServices.Container.Single<IGameStateMachine>();

    public void Change()
    {
        _stateMachine.Enter<LoadLevelState, string>(TransferTo);
    }
  }
}