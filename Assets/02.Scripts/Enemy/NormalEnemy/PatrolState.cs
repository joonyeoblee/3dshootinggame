using System.Collections.Generic;
using UnityEngine;
public class PatrolState : IEnemyState
{
    private readonly List<Vector3> _patrolPoints = new List<Vector3>(3);
    private Vector3 _nextPoint;

    private float _changePointTimer;
    public float RandomRange = 7f;
    public void Enter(Enemy enemy)
    {
        _changePointTimer = 0;

        if (_patrolPoints.Count != 3)
        {
            _patrolPoints.Clear();
            for (int i = 0; i < 2; i++)
            {
                _patrolPoints.Add(GetRandomPatrolPoint(enemy.StartPosition));
            }
            _patrolPoints.Add(enemy.StartPosition); // 마지막은 시작 위치
        }
        _nextPoint = _patrolPoints[0];
    }
    private Vector3 GetRandomPatrolPoint(Vector3 StartPosition)
    {
        return new Vector3(Random.Range(-RandomRange, RandomRange), 0,
            Random.Range(-RandomRange, RandomRange)) + StartPosition;
    }

    public void Execute(Enemy enemy)
    {
        _changePointTimer += Time.deltaTime;

        if (Vector3.Distance(_nextPoint, enemy.transform.position) <= enemy.Stat.ReturnDistance || _changePointTimer >= 3f)
        {
            _nextPoint = _patrolPoints[Random.Range(0, _patrolPoints.Count)];
            _changePointTimer = 0;
        }
        if (Vector3.Distance(enemy.transform.position, enemy.Player.transform.position) <= enemy.Stat.FindDistance)
        {
            enemy.StateMachine.ChangeState(EnemyState.Trace);
        }

        enemy.NavAgent.SetDestination(_nextPoint);

    }

    public void Exit(Enemy enemy)
    {
        enemy.NavAgent.ResetPath();
    }


}
