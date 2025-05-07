using UnityEngine;
public class AttackState : IEnemyState
{
    private float _timer;

    public void Enter(Enemy enemy)
    {
        _timer = 0f;
    }

    public void Execute(Enemy enemy)
    {
        if (!GameManager.Instance.IsPlaying) return;

        _timer += Time.deltaTime;

        if (enemy.IsSkill)
        {
            Throw(enemy);
            enemy.IsSkill = false;
            enemy.StateMachine.ChangeState(EnemyState.Trace);
            return;
        }

        if (Vector3.Distance(enemy.transform.position, enemy.Player.transform.position) >= enemy.Stat.AttackDistance)
        {
            enemy.StateMachine.ChangeState(EnemyState.Trace);
            return;
        }

        if (_timer < enemy.Stat.AttackCoolTime) return;

        if (enemy.EnemyType == EnemyType.Elite)
        {

            Smash(enemy);

            return;
        }


        if (_timer >= enemy.Stat.AttackCoolTime)
        {
            enemy.Animator.SetTrigger("Attack"); // 애니메이션 트리거만 건다
            enemy.StateMachine.ChangeState(EnemyState.Trace);
        }
    }
    private void Smash(Enemy enemy)
    {

        Collider[] hits = Physics.OverlapSphere(enemy.transform.position, enemy.Radius, enemy.TargetMask);

        float halfAngleRad = enemy.Angle * 0.5f * Mathf.Deg2Rad;
        float cosHalfAngle = Mathf.Cos(halfAngleRad);

        foreach(Collider hit in hits)
        {
            Vector3 dirToTarget = (hit.transform.position - enemy.transform.position).normalized;
            float dot = Vector3.Dot(enemy.transform.forward, dirToTarget);

            if (dot >= cosHalfAngle)
            {
                Debug.Log("적 타격: " + hit.name);
                enemy.Animator.SetTrigger("Attack");
                enemy.StateMachine.ChangeState(EnemyState.Trace);
                // 공격 처리

            }
            else
            {
                // 플레이어는 감지되었으나 시야각 바깥 -> 회전
                Quaternion lookRotation = Quaternion.LookRotation(dirToTarget);
                enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, lookRotation, Time.deltaTime * 2f);
            }
        }
    }

    private void Throw(Enemy enemy)
    {
        enemy.Animator.SetTrigger("Skill");
    }

    public void Exit(Enemy enemy)
    {
        // Exit 필요 없음
    }


}
