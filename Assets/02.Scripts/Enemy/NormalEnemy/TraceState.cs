using UnityEngine;
public class TraceState : IEnemyState
{

    public void Enter(Enemy enemy)
    {
        enemy.Animator.SetBool("Trace", true);
    }

    public void Execute(Enemy enemy)
    {
        if (!GameManager.Instance.IsPlaying)
        {
            enemy.NavAgent.isStopped = true;
            enemy.NavAgent.ResetPath();
            return;
        }

        int randomNumber = Random.Range(0, 20);

        if (randomNumber < 1 && (enemy.EnemyType == EnemyType.Elite || enemy.EnemyType == EnemyType.Elite1))
        {
            enemy.StateMachine.ChangeState(EnemyState.Attack);
            enemy.IsSkill = true;
            return;
        }

        if (Vector3.Distance(enemy.Player.transform.position, enemy.transform.position) <= enemy.Stat.AttackDistance && !enemy.IsTrace)
        {
            enemy.StateMachine.ChangeState(EnemyState.Attack);
            return;
        }
        if (Vector3.Distance(enemy.Player.transform.position, enemy.transform.position) >= enemy.Stat.FindDistance && !enemy.IsTrace)
        {
            enemy.StateMachine.ChangeState(EnemyState.Return);
            return;
        }

        enemy.NavAgent.SetDestination(enemy.Player.transform.position);

    }

    public void Exit(Enemy enemy)
    {
        enemy.NavAgent.ResetPath();
        enemy.Animator.SetBool("Trace", false);
    }
}
