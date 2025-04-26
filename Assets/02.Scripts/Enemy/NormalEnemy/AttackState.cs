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
        if(!GameManager.Instance.IsPlaying) return;

        _timer += Time.deltaTime;

        if (Vector3.Distance(enemy.transform.position, enemy.Player.transform.position) >= enemy.Stat.AttackDistance)
        {
            enemy.StateMachine.ChangeState(EnemyState.Trace);
            return;
        }

        if (_timer >= enemy.Stat.AttackCoolTime)
        {
            PerformAttack(enemy);

            _timer = 0f;
        }
    }

    private void PerformAttack(Enemy enemy)
    {
        Debug.Log("공격!");
        enemy.Animator.SetTrigger("Attack");

        Damage damage = new Damage(enemy.Stat.AttackDamage,0,enemy.gameObject );
        enemy.Player.GetComponent<Player>().TakeDamage(damage);
    }

    public void Exit(Enemy enemy)
    {

    }
}
