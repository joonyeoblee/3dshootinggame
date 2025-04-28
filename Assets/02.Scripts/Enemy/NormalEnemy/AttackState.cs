using UnityEngine;
public class AttackState : IEnemyState
{
    private float _timer;

    public void Enter(Enemy enemy)
    {
        _timer = enemy.Stat.AttackCoolTime;
    }

    public void Execute(Enemy enemy)
    {
        if (!GameManager.Instance.IsPlaying) return;

        _timer += Time.deltaTime;

        if (Vector3.Distance(enemy.transform.position, enemy.Player.transform.position) >= enemy.Stat.AttackDistance)
        {
            enemy.StateMachine.ChangeState(EnemyState.Trace);
            return;
        }

        if (_timer >= enemy.Stat.AttackCoolTime)
        {
            enemy.Animator.SetTrigger("Attack"); // 애니메이션 트리거만 건다
            _timer = 0f;
        }
    }

    public void Exit(Enemy enemy)
    {
        // Exit 필요 없음
    }
}
