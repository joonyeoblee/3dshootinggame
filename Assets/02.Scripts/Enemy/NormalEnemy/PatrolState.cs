using System.Collections.Generic;
using UnityEngine;
public class PatrolState : IEnemyState
{
    private readonly List<Vector3> _patrolPoints = new List<Vector3>(3);
    private Vector3 _nextPoint;

    private float _changePointTimer;
    public float RandomRange = 7f;
    private bool _destinationReached;

    public void Enter(Enemy enemy)
    {
        _changePointTimer = 0;
        _destinationReached = true;
        enemy.Animator.SetTrigger("Walk");

        if (_patrolPoints.Count != 3)
        {
            _patrolPoints.Clear();
            for (int i = 0; i < 2; i++)
            {
                _patrolPoints.Add(GetRandomPatrolPoint(enemy.StartPosition));
            }
            _patrolPoints.Add(enemy.StartPosition);
        }
    }

    private Vector3 GetRandomPatrolPoint(Vector3 startPosition)
    {
        return new Vector3(Random.Range(-RandomRange, RandomRange), 0,
            Random.Range(-RandomRange, RandomRange)) + startPosition;
    }

    public void Execute(Enemy enemy)
    {
        if (!GameManager.Instance.IsPlaying)
        {
            enemy.NavAgent.isStopped = true;
            enemy.NavAgent.ResetPath();
            return;
        }

        if (Vector3.Distance(enemy.transform.position, enemy.Player.transform.position) <= enemy.Stat.FindDistance)
        {
            enemy.StateMachine.ChangeState(EnemyState.Trace);
            return;
        }

        _changePointTimer += Time.deltaTime;

        if (_destinationReached || _changePointTimer >= 3f)
        {
            _nextPoint = _patrolPoints[Random.Range(0, _patrolPoints.Count)];
            enemy.NavAgent.SetDestination(_nextPoint);

            _destinationReached = false;
            _changePointTimer = 0;
        }

        if (!enemy.NavAgent.pathPending && enemy.NavAgent.remainingDistance <= enemy.Stat.ReturnDistance)
        {
            _destinationReached = true;
        }
    }

    public void Exit(Enemy enemy)
    {
        enemy.NavAgent.ResetPath();
    }
}
