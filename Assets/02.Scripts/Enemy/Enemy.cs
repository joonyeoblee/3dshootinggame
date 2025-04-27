using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

// 1, 상태를 열거형으로 정의한다.
public enum EnemyState
{
    Idle,
    Trace,
    Return,
    Attack,
    Damaged,
    Die,
    Patrol
}

public enum EnemyType
{
    Normal,
    Trace
}

public class Enemy : MonoBehaviour, IDamageable
{
    public EnemyStatsSO EnemyStats;
    public EnemyType EnemyType;

    public GameObject Player { get; private set; }
    // private CharacterController _characterController;
    public NavMeshAgent NavAgent { get; private set; }
    public Vector3 StartPosition;

    public int Health { get; set; }
    public Image HealthBar;

    private PoolItem _poolItem;

    public EnemyStat Stat { get; private set; }
    [Header("# StateMachine")]
    public EnemyStateMachine StateMachine { get; protected set; }

    public Animator Animator;

    public GameObject[] Models;
    /// <summary>
    ///     행동 정의
    /// </summary>
    protected virtual void AwakeInit()
    {
        int random = Random.Range(0, Models.Length);
        EnemyType = (EnemyType)random;
        foreach (GameObject model in Models)
        {
            model.SetActive(false);
        }
        Models[random].SetActive(true);
        Animator = Models[random].GetComponent<Animator>();

        Dictionary<EnemyState, IEnemyState> dict = new Dictionary<EnemyState, IEnemyState>
        {
            {
                EnemyState.Idle, new IdleState()
            },
            {
                EnemyState.Trace, new TraceState()
            },
            {
                EnemyState.Return, new ReturnState()
            },
            {
                EnemyState.Attack, new AttackState()
            },
            {
                EnemyState.Damaged, new DamagedState()
            },
            {
                EnemyState.Die, new DieState()
            },
            {
                EnemyState.Patrol, new PatrolState()
            }
        };
        StateMachine = new EnemyStateMachine(this, dict);
    }

    private void Awake()
    {
        AwakeInit();
    }
    private void Start()
    {

        StartPosition = transform.position;
        Stat = EnemyStats.GetData(EnemyType);
        Player = GameObject.FindGameObjectWithTag("Player");
        _poolItem = GetComponent<PoolItem>();

        // 에이전트 사용으로 변경
        NavAgent = GetComponent<NavMeshAgent>();
        NavAgent.speed = Stat.MoveSpeed;

    }

    private void OnEnable()
    {
        Init();
    }

    public void Init()
    {
        Health = 100;
        StateMachine.ChangeState(EnemyState.Idle);
    }
    private void Update()
    {
        StateMachine.Update();
        Debug.Log(StateMachine.CurrentState);
    }


    public void TakeDamage(Damage damage)
    {
        Debug.Log($"{name} From{damage.DamageFrom} Take {damage.Value} remain {Health}");
        if (StateMachine.CurrentState.GetType() == typeof(DieState))
        {
            return;
        }

        Health -= damage.Value;
        HealthBar.fillAmount = Health / 100f;
        if (Health <= 0)
        {
            StateMachine.ChangeState(EnemyState.Die);
            return;
        }

        StateMachine.ChangeState(EnemyState.Damaged);
    }

    public void Die()
    {
        StartCoroutine(Die_Coroutine());
    }
    private IEnumerator Die_Coroutine()
    {
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
        _poolItem.ReturnToPoolAs<Enemy>();
    }

}
