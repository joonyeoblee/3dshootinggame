using UnityEngine;
public class DieState : IEnemyState
{
    private float _dieTimer;

    public void Enter(Enemy enemy)
    {
        _dieTimer = 0f;
        enemy.NavAgent.isStopped = true;
    }

    public void Execute(Enemy enemy)
    {
        _dieTimer += Time.deltaTime;
        // if (_dieTimer >= enemy.DyingTime)
        // {
        //     enemy.StateMachine.ChangeState(EnemyState.Idle);
        // }
    }
    public void Exit(Enemy enemy)
    {
        enemy.NavAgent.isStopped = false;
        enemy.NavAgent.ResetPath();
        Debug.Log("Nav 초기화됨");
        enemy.Die();
    }
}
