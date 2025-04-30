using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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

    public float DyingTime { get; private set; } = 2f;


    public Renderer[] Renderers;
    public Color HitColor = Color.red;
    public float FlashDuration = 0.1f;

    private Color[] originalColors;

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
                EnemyState.Patrol, new PatrolState()
            },
            {
                EnemyState.Die, new DieState()
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
        originalColors = new Color[Renderers.Length];
        for (int i = 0; i < Renderers.Length; i++)
        {
            originalColors[i] = Renderers[i].material.color;
        }
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
        Health -= damage.Value;
        HealthBar.fillAmount = Health / 100f;
        FlashRed();

        if (StateMachine.CurrentState.GetType() == typeof(DieState))
        {
            return;
        }

        if (Health <= 0)
        {
            StateMachine.ChangeState(EnemyState.Die);
        }
        else
        {
            StateMachine.ChangeState(EnemyState.Damaged);
        }
    }
    public void DealDamage()
    {
        Debug.Log("애니메이션 이벤트로 공격함!");

        if (Player == null) return;

        Damage damage = new Damage(Stat.AttackDamage, 0, gameObject);
        Player.GetComponent<Player>().TakeDamage(damage);
    }
    public void Die()
    {
        StartCoroutine(Die_Coroutine());
    }
    private IEnumerator Die_Coroutine()
    {
        Animator.SetTrigger("Die");
        yield return null; // 한 프레임 기다림
        Animator.ResetTrigger("Die"); // 트리거 초기화
        yield return new WaitForSeconds(2f);
        GetComponent<ItemSpawner>().DropItem();
        
        gameObject.SetActive(false);
        _poolItem.ReturnToPoolAs<Enemy>();

    }

    public void FlashRed()
    {
        for (int i = 0; i < Renderers.Length; i++)
        {
            Material mat = Renderers[i].material;

            // 먼저 즉시 붉은색으로
            mat.color = HitColor;

            // 일정 시간 후 원래 색으로 부드럽게 복귀
            mat.DOColor(originalColors[i], FlashDuration);
        }
    }

}
