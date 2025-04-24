using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemyBT : MonoBehaviour
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

    private readonly List<Vector3> _patrolPoints = new List<Vector3>();
    private int _patrolIndex;

    private BehaviorTree _behaviorTree;

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
        private readonly EnemyBT _EnemyBT;

        public IsDeadCondition(EnemyBT EnemyBT)
        {
            _EnemyBT = EnemyBT;
        }

        public override bool Execute()
        {
            return _EnemyBT.Health <= 0;
        }
    }

    public class IsDamagedCondition : BTNode
    {
        private readonly EnemyBT _EnemyBT;

        public IsDamagedCondition(EnemyBT EnemyBT)
        {
            _EnemyBT = EnemyBT;
        }

        public override bool Execute()
        {
            return _EnemyBT.Health > 0 && _EnemyBT.Health < 100;
        }
    }

    public class IsInAttackRangeCondition : BTNode
    {
        private readonly EnemyBT _EnemyBT;

        public IsInAttackRangeCondition(EnemyBT EnemyBT)
        {
            _EnemyBT = EnemyBT;
        }

        public override bool Execute()
        {
            return Vector3.Distance(_EnemyBT.transform.position, _EnemyBT._player.transform.position) <= _EnemyBT.AttackDistance;
        }
    }

    public class IsPlayerInRangeCondition : BTNode
    {
        private readonly EnemyBT _EnemyBT;

        public IsPlayerInRangeCondition(EnemyBT EnemyBT)
        {
            _EnemyBT = EnemyBT;
        }

        public override bool Execute()
        {
            float distance = Vector3.Distance(_EnemyBT.transform.position, _EnemyBT._player.transform.position);
            return distance <= _EnemyBT.FindDistance && distance >= _EnemyBT.AttackDistance;
        }
    }

    public class ShouldPatrolCondition : BTNode
    {
        private readonly EnemyBT _EnemyBT;

        public ShouldPatrolCondition(EnemyBT EnemyBT)
        {
            _EnemyBT = EnemyBT;
        }

        public override bool Execute()
        {
            _EnemyBT._idleTimer += Time.deltaTime;
            return _EnemyBT._idleTimer >= _EnemyBT._idleDuration;
        }
    }

    // 액션 노드들
    public class DieAction : BTNode
    {
        private readonly EnemyBT _EnemyBT;

        public DieAction(EnemyBT EnemyBT)
        {
            _EnemyBT = EnemyBT;
        }

        public override bool Execute()
        {
            _EnemyBT.StartCoroutine(_EnemyBT.Die_Coroutine());
            return true;
        }
    }

    public class DamagedAction : BTNode
    {
        private readonly EnemyBT _EnemyBT;

        public DamagedAction(EnemyBT EnemyBT)
        {
            _EnemyBT = EnemyBT;
        }

        public override bool Execute()
        {
            return true;
        }
    }

    public class AttackAction : BTNode
    {
        private readonly EnemyBT _EnemyBT;

        public AttackAction(EnemyBT EnemyBT)
        {
            _EnemyBT = EnemyBT;
        }

        public override bool Execute()
        {
            _EnemyBT._attackTimer += Time.deltaTime;
            if (_EnemyBT._attackTimer > _EnemyBT.AttackCoolTime)
            {
                Debug.Log("플레이어 공격!");
                _EnemyBT._attackTimer = 0f;
            }
            return true;
        }
    }

    public class TraceAction : BTNode
    {
        private readonly EnemyBT _EnemyBT;

        public TraceAction(EnemyBT EnemyBT)
        {
            _EnemyBT = EnemyBT;
        }

        public override bool Execute()
        {
            Vector3 direction = (_EnemyBT._player.transform.position - _EnemyBT.transform.position).normalized;
            _EnemyBT._characterController.Move(direction * _EnemyBT.MoveSpeed * Time.deltaTime);
            return true;
        }
    }

    public class PatrolAction : BTNode
    {
        private readonly EnemyBT _EnemyBT;

        public PatrolAction(EnemyBT EnemyBT)
        {
            _EnemyBT = EnemyBT;
        }

        public override bool Execute()
        {
            Vector3 target = _EnemyBT._patrolPoints[_EnemyBT._patrolIndex];
            Vector3 direction = (target - _EnemyBT.transform.position).normalized;
            _EnemyBT._characterController.Move(direction * _EnemyBT.MoveSpeed * Time.deltaTime);

            if (Vector3.Distance(_EnemyBT.transform.position, target) <= 0.1f)
            {
                _EnemyBT._patrolIndex++;
                if (_EnemyBT._patrolIndex >= _EnemyBT._patrolPoints.Count)
                {
                    _EnemyBT._patrolIndex = 0;
                    _EnemyBT.GeneratePatrolPoints();
                    _EnemyBT._idleTimer = 0f;
                }
            }
            return true;
        }
    }

    public class IdleAction : BTNode
    {
        private readonly EnemyBT _EnemyBT;

        public IdleAction(EnemyBT EnemyBT)
        {
            _EnemyBT = EnemyBT;
        }

        public override bool Execute()
        {
            return true;
        }
    }
}
