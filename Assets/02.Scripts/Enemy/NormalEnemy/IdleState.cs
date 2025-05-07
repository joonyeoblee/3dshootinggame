using UnityEngine;
public class IdleState : IEnemyState
{
    private float _timer;

    public void Enter(Enemy enemy)
    {
        _timer = 0f;
        enemy.Animator.SetTrigger("Idle");
    }

    public void Execute(Enemy enemy)
    {
        if(!GameManager.Instance.IsPlaying) return;

        _timer += Time.deltaTime;
        if (_timer >= enemy.Stat.IdleToPatrolTime)
        {
            enemy.StateMachine.ChangeState(EnemyState.Patrol);
        }

        if (Vector3.Distance(enemy.transform.position, enemy.Player.transform.position) < enemy.Stat.FindDistance)
        {
            enemy.StateMachine.ChangeState(EnemyState.Trace);
        }
    }
    public void Exit(Enemy enemy)
    {

    }
}
