using UnityEngine;
[CreateAssetMenu(fileName = "DieState", menuName = "Enemy/States/DieState")]
public class DieState : ScriptableObject, IEnemyState
{
    private float _dieTimer;

    public void Enter(Enemy enemy)
    {
        _dieTimer = 0f;
        enemy.NavAgent.isStopped = true;
    }

    public void Execute(Enemy enemy)
    {
        _dieTimer += Time.deltaTime;
        if (_dieTimer >= enemy.DyingTime)
        {
            enemy.Die();
        }
    }
    public void Exit(Enemy enemy)
    {
        enemy.NavAgent.isStopped = false;
        enemy.NavAgent.ResetPath();
        Debug.Log("Nav 초기화됨");
    }
}
