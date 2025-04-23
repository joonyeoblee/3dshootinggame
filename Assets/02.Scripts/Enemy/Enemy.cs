using System.Collections;
using UnityEngine;
public class Enemy : MonoBehaviour
{
    // 1, 상태를 열거형으로 정의한다
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

    // 2. 현재 상태를 지정한다
    public EnemyState CurrentState = EnemyState.Idle;

    [SerializeField] private GameObject _player;
    private CharacterController _characterController;
    private Vector3 _startPosition;

    private const float GRAVITY = -9.81f;

    public float FindDistance = 7f; // 탐색 범위
    public float AttackDistance = 2f; // 공격 범위
    public float MoveSpeed = 3.3f;

    public float AttackCoolTime = 2f;
    private float _attackTimer;

    public int Health = 100;

    public float DamagedTime = 0.5f;
    public float DeathTime = 2f;

    public float RandomRange = 7f;
    private float _idleTimer;
    private readonly float _idleDuration = 2f;
    private bool _isPatrolling; // 중복 방지

    private void Start()
    {
        _startPosition = transform.position;
        _characterController = GetComponent<CharacterController>();
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        Vector3 dir = Vector3.zero;

        dir.y = GRAVITY * Time.deltaTime;

        _characterController.Move(dir * Time.deltaTime);
        // 나의 현재 상태에 따라 상태 함수를 호출한다.
        switch (CurrentState)
        {
            case EnemyState.Idle:
            {
                Idle();
                break;
            }
            case EnemyState.Trace:
            {
                Trace();
                break;
            }
            case EnemyState.Return:
            {
                Return();
                break;
            }
            case EnemyState.Attack:
            {
                Attack();
                break;
            }
            case EnemyState.Patrol:
            {
                Patrol();
                break;
            }

        }
    }

    public void TakeDamage(Damage damage)
    {
        if (CurrentState == EnemyState.Damaged || CurrentState == EnemyState.Die)
        {
            return;
        }
        Health -= damage.Value;

        if (Health <= 0)
        {
            Debug.Log($"{CurrentState} -> Die");
            CurrentState = EnemyState.Die;
            StartCoroutine(Die_Coroutine());
            return;
        }

        Debug.Log($"{CurrentState} -> Damaged");

        CurrentState = EnemyState.Damaged;
        StartCoroutine(Damaged_Coroutine(damage.WeaponPower));
    }

    // 3. 상태 함수들을 구현한다
    private void Idle()
    {
        // 대기

        // 필요 속성
        // 1. 플레이어 위치와 거리
        // 2. FindDistance
        if (Vector3.Distance(transform.position, _player.transform.position) <= FindDistance)
        {
            Debug.Log($"{CurrentState}-> Trace");
            CurrentState = EnemyState.Trace;
            return;
        }

        _idleTimer += Time.deltaTime;

        if (_idleTimer >= _idleDuration)
        {
            _idleTimer = 0f;
            Debug.Log($"{CurrentState} -> Patrol");
            CurrentState = EnemyState.Patrol;
        }

    }

    private void Trace()
    {
        // 전이 : 플레이어와 멀어지면 -> Return
        if (Vector3.Distance(transform.position, _player.transform.position) >= FindDistance)
        {
            Debug.Log($"{CurrentState} -> Return");
            CurrentState = EnemyState.Return;
            return;
        }

        // 전이 : 공격 범위 만큼 가까워 지면 -> Attack
        if (Vector3.Distance(transform.position, _player.transform.position) < AttackDistance)
        {
            Debug.Log($"{CurrentState} -> Attack");
            CurrentState = EnemyState.Attack;
            return;
        }

        // 쫓아간다
        Vector3 diraction = (_player.transform.position - transform.position).normalized;
        _characterController.Move(diraction * MoveSpeed * Time.deltaTime);
    }

    private void Return()
    {
        // 전이 : 시작 위치와 가까워 지면 -> Idle
        if (Vector3.Distance(transform.position, _startPosition) <= 0.1f)
        {
            Debug.Log($"{CurrentState} -> Idle");
            transform.position = _startPosition;
            CurrentState = EnemyState.Idle;
            return;
        }

        // 전이 : 되돌아 가는 도중 적을 찾으면 다시 Trace
        if (Vector3.Distance(transform.position, _player.transform.position) <= FindDistance)
        {
            Debug.Log($"{CurrentState} -> Trace");
            CurrentState = EnemyState.Trace;
            return;
        }

        // 시작 위치와되돌아간다
        Vector3 diraction = (_startPosition - transform.position).normalized;
        _characterController.Move(diraction * MoveSpeed * Time.deltaTime);
    }

    private void Attack()
    {
        // 전이 : 공격 범위 만큼 가까워 지면 -> Attack
        if (Vector3.Distance(transform.position, _player.transform.position) > AttackDistance)
        {
            Debug.Log($"{CurrentState}-> Trace");
            CurrentState = EnemyState.Trace;
            _attackTimer = 0f;
            return;
        }

        _attackTimer += Time.deltaTime;

        if (_attackTimer > AttackCoolTime)
        { // 공격한다
            Debug.Log("플레이어 공격!");
            _attackTimer = 0f;
        }
    }

    private IEnumerator Damaged_Coroutine(int WeaponPower)
    {
        Vector3 diraction = (transform.position - _player.transform.position).normalized;
        _characterController.Move(diraction * MoveSpeed * WeaponPower * Time.deltaTime);
        yield return new WaitForSeconds(DamagedTime);

        Debug.Log($"{CurrentState} -> Trace");
        CurrentState = EnemyState.Trace;
    }

    private IEnumerator Die_Coroutine()
    {
        yield return new WaitForSeconds(DeathTime);
        gameObject.SetActive(false);
    }

    private void Patrol()
    {
        if (!_isPatrolling)
        {
            _isPatrolling = true;
            StartCoroutine(Patrol_Coroutine());
        }

    }

    private IEnumerator Patrol_Coroutine()
    {
        Vector3[] points = { GetRandomPatrolPoint(), GetRandomPatrolPoint(), _startPosition };

        foreach (Vector3 point in points)
        {
            if (Vector3.Distance(transform.position, _player.transform.position) <= FindDistance)
            {
                Debug.Log($"{CurrentState} -> Trace (도중)");
                CurrentState = EnemyState.Trace;
                _isPatrolling = false;
                yield break;
            }

            yield return StartCoroutine(PointMove_Coroutine(point));
        }

        Debug.Log($"{CurrentState} -> Idle");
        CurrentState = EnemyState.Idle;
        _isPatrolling = false;
    }

    private IEnumerator PointMove_Coroutine(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.1f)
        {
            if (Vector3.Distance(transform.position, _player.transform.position) <= FindDistance)
            {
                Debug.Log($"{CurrentState} -> Trace (이동 중)");
                CurrentState = EnemyState.Trace;
                _isPatrolling = false;
                yield break;
            }

            Vector3 dir = (target - transform.position).normalized;
            _characterController.Move(dir * MoveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private Vector3 GetRandomPatrolPoint()
    {
        return new Vector3(Random.Range(-RandomRange, RandomRange), 0,
            Random.Range(-RandomRange, RandomRange)) + _startPosition;
    }
}
