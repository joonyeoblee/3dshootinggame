using System.Collections.Generic;
public class EnemyStateMachine
{
    private readonly Enemy _enemy;
    public Dictionary<EnemyState, IEnemyState> StateDictionary { get; }

    private IEnemyState _currentState;
    public IEnemyState CurrentState
    {
        get
        {
            return _currentState;
        }
    }

    public EnemyStateMachine(Enemy enemy, Dictionary<EnemyState, IEnemyState> stateDictionary)
    {
        _enemy = enemy;
        StateDictionary = stateDictionary;
        _currentState = StateDictionary[EnemyState.Idle];
    }

    public void ChangeState(EnemyState newState)
    {
        if (StateDictionary.TryGetValue(newState, out IEnemyState state))
        {
            if (_currentState != null && _currentState != state)
            {
                _currentState.Exit(_enemy);
            }
            _currentState = state;
            _currentState.Enter(_enemy);
        }
    }

    public void Update()
    {
        if (!GameManager.Instance.IsPlaying)
        {
            _enemy.NavAgent.isStopped = true;
            _enemy.NavAgent.ResetPath();
            return;
        }
        _currentState?.Execute(_enemy);
    }

    public void ModifyState(EnemyState which, IEnemyState to)
    {
        StateDictionary[which] = to;
    }
}
