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
    Trace,
    Elite,
    Elite1
}


public class Enemy : MonoBehaviour, IDamageable
{
    public EnemyStatsSO EnemyStats;
    public EnemyType EnemyType;

    public GameObject Player { get; private set; }
    // private CharacterController _characterController;
    public NavMeshAgent NavAgent { get; private set; }
    public Vector3 StartPosition;

    public float Health;
    public Image LateHealthBar;
    public Image HealthBar;
    private Coroutine _coroutine;
    public float Duration = 2f;
    public bool IsTrace;

    public bool IsSkill;

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


    public float Radius;
    // private readonly List<Vector3> _patrolPoints = new List<Vector3>();
    private int _patrolIndex;
    private bool _isSlashing;
    public float Angle;
    public LayerMask TargetMask;
    public GameObject Muzzle;
    public GameObject Barrel;
    protected virtual void AwakeInit()
    {
        if (EnemyType != EnemyType.Elite && EnemyType != EnemyType.Elite1)
        {
            int random = Random.Range(0, Models.Length);
            EnemyType = (EnemyType)random;
            foreach(GameObject model in Models)
            {
                model.SetActive(false);
            }
            Models[random].SetActive(true);
            Animator = Models[random].GetComponent<Animator>();
        }

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
        Health = Stat.MaxHealth;

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
        StateMachine.ChangeState(EnemyState.Idle);
    }
    private void Update()
    {
        NavAgent.isStopped = !GameManager.Instance.IsPlaying;
        Debug.Log(NavAgent.isStopped);
        if (!GameManager.Instance.IsPlaying) return;

        StateMachine.Update();
        Debug.Log(StateMachine.CurrentState);
    }


    public void TakeDamage(Damage damage)
    {
        Health -= damage.Value;
        HealthBar.fillAmount = Health / Stat.MaxHealth;
        // Debug.Log($"Health: {Health}, MaxHealth: {Stat.MaxHealth}, FillAmount: {HealthBar.fillAmount}");

        FlashRed();

        if (StateMachine.CurrentState.GetType() == typeof(DieState))
        {
            return;
        }
        float healthPercent = Health / Stat.MaxHealth;
        if ((EnemyType == EnemyType.Elite1 || EnemyType == EnemyType.Elite) && healthPercent <= 0.3f)
        {
            BossPhase();
        }

        if (Health <= 0)
        {
            StateMachine.ChangeState(EnemyState.Die);
        }
        else
        {
            StateMachine.ChangeState(EnemyState.Damaged);
        }

        if (LateHealthBar == null) return;

        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
        _coroutine = StartCoroutine(ReduceValueOverTime(LateHealthBar.fillAmount, HealthBar.fillAmount, Duration));
    }
    private IEnumerator ReduceValueOverTime(float start, float end, float time)
    {
        float t = 0f;

        while (t < time)
        {
            LateHealthBar.fillAmount = Mathf.Lerp(start, end, t / time);
            t += Time.deltaTime;
            yield return null;
        }
    }

    public void BossPhase()
    {
        transform.DOScale(1f, 0.3f);
        IsTrace = true;
        NavAgent.speed = Stat.MoveSpeed + 1.3f;

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
        yield return new WaitForSeconds(1f);

        if (EnemyType == EnemyType.Elite || EnemyType == EnemyType.Elite1)
        {
            GetComponent<Explore>().Explode();

        }
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

    private void OnDrawGizmosSelected()
    {
    #if UNITY_EDITOR
        // 1. 범위 반지름 구체
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Radius);

        // 2. 시야각 (Angle) 시각화
        Vector3 forward = transform.forward;
        Quaternion leftRot = Quaternion.AngleAxis(-Angle * 0.5f, Vector3.up);
        Quaternion rightRot = Quaternion.AngleAxis(Angle * 0.5f, Vector3.up);

        Vector3 leftDir = leftRot * forward;
        Vector3 rightDir = rightRot * forward;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + leftDir * Radius);
        Gizmos.DrawLine(transform.position, transform.position + rightDir * Radius);

        // 시각적 확인을 위해 부채꼴 내부 채우기 (옵션)
        int segments = 20;
        for (int i = 0; i <= segments; i++)
        {
            float angle = -Angle * 0.5f + Angle / segments * i;
            Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 dir = rot * forward;
            Gizmos.DrawLine(transform.position, transform.position + dir * Radius);
        }
    #endif
    }

}
