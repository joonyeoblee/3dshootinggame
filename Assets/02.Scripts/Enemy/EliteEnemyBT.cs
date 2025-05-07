using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EliteEnemyBT : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    private CharacterController _characterController;
    private Vector3 _startPosition;

    private const float GRAVITY = -9.81f;

    public float FindDistance = 7f;
    public float ReturnDistance = 10f;
    public float AttackDistance = 2f;
    public float MoveSpeed = 3.3f;
    public float AttackCoolTime = 2f;
    private float _attackTimer;
    public int Health = 100;
    public float DamagedTime = 0.5f;
    public float DeathTime = 2f;
    public float RandomRange = 7f;
    private float _idleTimer;
    private readonly float _idleDuration = 2f;


    public float Radius;
    private readonly List<Vector3> _patrolPoints = new List<Vector3>();
    private int _patrolIndex;

    private BehaviorTree _behaviorTree;
    private bool _isSlashing;
    public float Angle;
    public LayerMask TargetMask;
    private void Start()
    {
        _startPosition = transform.position;
        GeneratePatrolPoints();
        _characterController = GetComponent<CharacterController>();
        _player = GameObject.FindGameObjectWithTag("Player");

        BuildBehaviorTree();
    }

    private void Update()
    {
        Vector3 dir = Vector3.zero;
        dir.y = GRAVITY * Time.deltaTime;
        _characterController.Move(dir * Time.deltaTime);

        _behaviorTree.Tick();
    }

    private void BuildBehaviorTree()
    {
        _behaviorTree = new BehaviorTree();

        // 루트 노드 설정
        Selector root = new Selector();
        _behaviorTree.SetRoot(root);

        // 죽음 상태 체크
        Sequence deathSequence = new Sequence();
        deathSequence.AddChild(new IsDeadCondition(this));
        deathSequence.AddChild(new DieAction(this));
        root.AddChild(deathSequence);

        // 피격 상태 체크
        Sequence damagedSequence = new Sequence();
        damagedSequence.AddChild(new IsDamagedCondition(this));
        damagedSequence.AddChild(new DamagedAction(this));
        root.AddChild(damagedSequence);

        // 공격 상태 체크
        Sequence attackSequence = new Sequence();
        attackSequence.AddChild(new IsInAttackRangeCondition(this));
        attackSequence.AddChild(new AttackAction(this));
        root.AddChild(attackSequence);

        // 추적 상태 체크
        Sequence traceSequence = new Sequence();
        traceSequence.AddChild(new IsPlayerInRangeCondition(this));
        traceSequence.AddChild(new TraceAction(this));
        root.AddChild(traceSequence);

        // 순찰 상태 체크
        Sequence patrolSequence = new Sequence();
        patrolSequence.AddChild(new ShouldPatrolCondition(this));
        patrolSequence.AddChild(new PatrolAction(this));
        root.AddChild(patrolSequence);

        // 대기 상태 (기본 상태)
        root.AddChild(new IdleAction(this));
    }

    public void TakeDamage(Damage damage)
    {
        if (Health <= 0) return;

        Health -= damage.Value;
        if (Health <= 0)
        {
            StartCoroutine(Die_Coroutine());
        }
        else
        {
            StartCoroutine(Damaged_Coroutine(damage.WeaponPower));
        }
    }

    private IEnumerator Damaged_Coroutine(int weaponPower)
    {
        Vector3 direction = (transform.position - _player.transform.position).normalized;
        _characterController.Move(direction * MoveSpeed * weaponPower * Time.deltaTime);
        yield return new WaitForSeconds(DamagedTime);
    }

    private IEnumerator Die_Coroutine()
    {
        yield return new WaitForSeconds(DeathTime);
        gameObject.SetActive(false);
    }

    private void GeneratePatrolPoints()
    {
        _patrolPoints.Clear();
        for (int i = 0; i < 2; i++)
        {
            _patrolPoints.Add(GetRandomPatrolPoint());
        }
        _patrolPoints.Add(_startPosition);
    }

    private Vector3 GetRandomPatrolPoint()
    {
        return new Vector3(Random.Range(-RandomRange, RandomRange), 0,
            Random.Range(-RandomRange, RandomRange)) + _startPosition;
    }

    // 행동 트리 관련 클래스들
    public class BehaviorTree
    {
        private BTNode _root;

        public void SetRoot(BTNode root)
        {
            _root = root;
        }

        public void Tick()
        {
            _root?.Execute();
        }
    }

    public abstract class BTNode
    {
        public abstract bool Execute();
    }

    public class Selector : BTNode
    {
        private readonly List<BTNode> _children = new List<BTNode>();

        public void AddChild(BTNode child)
        {
            _children.Add(child);
        }

        public override bool Execute()
        {
            foreach(BTNode child in _children)
            {
                if (child.Execute())
                    return true;
            }
            return false;
        }
    }

    public class Sequence : BTNode
    {
        private readonly List<BTNode> _children = new List<BTNode>();

        public void AddChild(BTNode child)
        {
            _children.Add(child);
        }

        public override bool Execute()
        {
            foreach(BTNode child in _children)
            {
                if (!child.Execute())
                    return false;
            }
            return true;
        }
    }

    // 조건 노드들
    public class IsDeadCondition : BTNode
    {
        private readonly EliteEnemyBT _EliteEnemyBT;

        public IsDeadCondition(EliteEnemyBT EliteEnemyBT)
        {
            _EliteEnemyBT = EliteEnemyBT;
        }

        public override bool Execute()
        {
            return _EliteEnemyBT.Health <= 0;
        }
    }

    public class IsDamagedCondition : BTNode
    {
        private readonly EliteEnemyBT _EliteEnemyBT;

        public IsDamagedCondition(EliteEnemyBT EliteEnemyBT)
        {
            _EliteEnemyBT = EliteEnemyBT;
        }

        public override bool Execute()
        {
            return _EliteEnemyBT.Health > 0 && _EliteEnemyBT.Health < 100;
        }
    }

    public class IsInAttackRangeCondition : BTNode
    {
        private readonly EliteEnemyBT _EliteEnemyBT;

        public IsInAttackRangeCondition(EliteEnemyBT EliteEnemyBT)
        {
            _EliteEnemyBT = EliteEnemyBT;
        }

        public override bool Execute()
        {
            return Vector3.Distance(_EliteEnemyBT.transform.position, _EliteEnemyBT._player.transform.position) <= _EliteEnemyBT.AttackDistance;
        }
    }

    public class IsPlayerInRangeCondition : BTNode
    {
        private readonly EliteEnemyBT _EliteEnemyBT;

        public IsPlayerInRangeCondition(EliteEnemyBT EliteEnemyBT)
        {
            _EliteEnemyBT = EliteEnemyBT;
        }

        public override bool Execute()
        {
            float distance = Vector3.Distance(_EliteEnemyBT.transform.position, _EliteEnemyBT._player.transform.position);
            return distance <= _EliteEnemyBT.FindDistance && distance >= _EliteEnemyBT.AttackDistance;
        }
    }

    public class ShouldPatrolCondition : BTNode
    {
        private readonly EliteEnemyBT _EliteEnemyBT;

        public ShouldPatrolCondition(EliteEnemyBT EliteEnemyBT)
        {
            _EliteEnemyBT = EliteEnemyBT;
        }

        public override bool Execute()
        {
            _EliteEnemyBT._idleTimer += Time.deltaTime;
            return _EliteEnemyBT._idleTimer >= _EliteEnemyBT._idleDuration;
        }
    }

    // 액션 노드들
    public class DieAction : BTNode
    {
        private readonly EliteEnemyBT _EliteEnemyBT;

        public DieAction(EliteEnemyBT EliteEnemyBT)
        {
            _EliteEnemyBT = EliteEnemyBT;
        }

        public override bool Execute()
        {
            _EliteEnemyBT.StartCoroutine(_EliteEnemyBT.Die_Coroutine());
            return true;
        }
    }

    public class DamagedAction : BTNode
    {
        private readonly EliteEnemyBT _EliteEnemyBT;

        public DamagedAction(EliteEnemyBT EliteEnemyBT)
        {
            _EliteEnemyBT = EliteEnemyBT;
        }

        public override bool Execute()
        {
            return true;
        }
    }

    public class AttackAction : BTNode
    {
        private readonly EliteEnemyBT _EliteEnemyBT;

        public AttackAction(EliteEnemyBT EliteEnemyBT)
        {
            _EliteEnemyBT = EliteEnemyBT;
        }

        public override bool Execute()
        {
            _EliteEnemyBT._attackTimer += Time.deltaTime;
            if (_EliteEnemyBT._attackTimer > _EliteEnemyBT.AttackCoolTime)
            {
                _EliteEnemyBT.Slash();
                _EliteEnemyBT._attackTimer = 0f;
            }
            return true;
        }
    }

    public class TraceAction : BTNode
    {
        private readonly EliteEnemyBT _EliteEnemyBT;

        public TraceAction(EliteEnemyBT EliteEnemyBT)
        {
            _EliteEnemyBT = EliteEnemyBT;
        }

        public override bool Execute()
        {
            Vector3 direction = (_EliteEnemyBT._player.transform.position - _EliteEnemyBT.transform.position).normalized;
            _EliteEnemyBT._characterController.Move(direction * _EliteEnemyBT.MoveSpeed * Time.deltaTime);
            return true;
        }
    }

    public class PatrolAction : BTNode
    {
        private readonly EliteEnemyBT _EliteEnemyBT;

        public PatrolAction(EliteEnemyBT EliteEnemyBT)
        {
            _EliteEnemyBT = EliteEnemyBT;
        }

        public override bool Execute()
        {
            Vector3 target = _EliteEnemyBT._patrolPoints[_EliteEnemyBT._patrolIndex];
            Vector3 direction = (target - _EliteEnemyBT.transform.position).normalized;
            _EliteEnemyBT._characterController.Move(direction * _EliteEnemyBT.MoveSpeed * Time.deltaTime);

            if (Vector3.Distance(_EliteEnemyBT.transform.position, target) <= 0.1f)
            {
                _EliteEnemyBT._patrolIndex++;
                if (_EliteEnemyBT._patrolIndex >= _EliteEnemyBT._patrolPoints.Count)
                {
                    _EliteEnemyBT._patrolIndex = 0;
                    _EliteEnemyBT.GeneratePatrolPoints();
                    _EliteEnemyBT._idleTimer = 0f;
                }
            }
            return true;
        }
    }

    public class IdleAction : BTNode
    {
        private readonly EliteEnemyBT _EliteEnemyBT;

        public IdleAction(EliteEnemyBT EliteEnemyBT)
        {
            _EliteEnemyBT = EliteEnemyBT;
        }

        public override bool Execute()
        {
            return true;
        }
    }

    private void Slash()
    {
        if (_isSlashing || _attackTimer > 0)
        {
            return;
        }

        _isSlashing = true;
        _attackTimer = AttackCoolTime;

        Collider[] hits = Physics.OverlapSphere(transform.position, Radius, TargetMask);

        float halfAngleRad = Angle * 0.5f * Mathf.Deg2Rad;
        float cosHalfAngle = Mathf.Cos(halfAngleRad);
        // _player.Animator.SetTrigger("Slash");
        // vfx.Play(); // 또는 vfx.SendEvent("OnPlay");
        foreach(Collider hit in hits)
        {
            Vector3 dirToTarget = (hit.transform.position - transform.position).normalized;
            float dot = Vector3.Dot(transform.forward, dirToTarget);

            if (dot >= cosHalfAngle)
            {
                Debug.Log("적 타격: " + hit.name);

                IDamageable damageable = hit.GetComponent<IDamageable>();
                Damage damage = new Damage(20, 30, gameObject);
                damageable.TakeDamage(damage);
            }
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
