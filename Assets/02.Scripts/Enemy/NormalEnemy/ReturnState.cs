using UnityEngine;
public class ReturnState : IEnemyState
{
    public void Enter(Enemy enemy)
    {
    }

    public void Execute(Enemy enemy)
    {
        if (Vector3.Distance(enemy.transform.position, enemy.StartPosition) <= enemy.Stat.ReturnDistance)
        {
            enemy.transform.position = enemy.StartPosition;
            enemy.StateMachine.ChangeState(EnemyState.Idle);
            return;
        }
        if (Vector3.Distance(enemy.transform.position, enemy.Player.transform.position) <= enemy.Stat.FindDistance)
        {
            enemy.StateMachine.ChangeState(EnemyState.Trace);
            return;
        }

        enemy.NavAgent.SetDestination(enemy.StartPosition);
        //Vector3 direction = (enemy.StartPosition - enemy.transform.position).normalized;
        //direction.y = enemy.YVelocity;
        //enemy.CharacterController.Move(direction * enemy.Stat.MoveSpeed * Time.deltaTime);
    }
    void IEnemyState.Exit(Enemy enemy)
    {

    }
}
