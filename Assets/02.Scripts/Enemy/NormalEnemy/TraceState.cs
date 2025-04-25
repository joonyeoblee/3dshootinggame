using UnityEngine;
public class TraceState : IEnemyState
{

    public void Enter(Enemy enemy)
    {
        enemy.Animator.SetBool("Trace", true);
    }

    public void Execute(Enemy enemy)
    {
        if (Vector3.Distance(enemy.Player.transform.position, enemy.transform.position) <= enemy.Stat.AttackDistance)
        {
            enemy.StateMachine.ChangeState(EnemyState.Attack);
            return;
        }
        if (Vector3.Distance(enemy.Player.transform.position, enemy.transform.position) >= enemy.Stat.FindDistance)
        {
            enemy.StateMachine.ChangeState(EnemyState.Return);
            return;
        }

        enemy.NavAgent.SetDestination(enemy.Player.transform.position);

        //Vector3 direction = (enemy.Player.transform.position - enemy.transform.position).normalized;
        //direction.y = enemy.YVelocity;
        //enemy.CharacterController.Move(enemy.Stat.MoveSpeed * Time.deltaTime * direction);
    }

    public void Exit(Enemy enemy)
    {
        enemy.NavAgent.ResetPath();
        enemy.Animator.SetBool("Trace", false);
    }
}
