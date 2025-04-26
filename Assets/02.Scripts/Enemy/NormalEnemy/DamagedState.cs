using UnityEngine;
public class DamagedState : IEnemyState
{
    private float _damagedTimer;

    public void Enter(Enemy enemy)
    {
        _damagedTimer = 0f;
    }

    public void Execute(Enemy enemy)
    {
         if(!GameManager.Instance.IsPlaying) return;

        _damagedTimer += Time.deltaTime;

        if (_damagedTimer >= enemy.Stat.DamagedTime)
        {
            enemy.StateMachine.ChangeState(EnemyState.Trace);
        }
    }

    public void Exit(Enemy enemy)
    {

    }
}
