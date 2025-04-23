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
        Die
    }

    // 2. 현재 상태를 지정한다
    public EnemyState CurrentState = EnemyState.Idle;

    [SerializeField] private GameObject _player;
    public float FindDistance = 7f; // 탐색 범위
    public float AttackDistance = 2f; // 공격 범위
    private Vector3 _startPosition;

    private CharacterController _characterController;
    public float MoveSpeed = 3.3f;

    private void Start()
    {
        _startPosition = transform.position;
        _characterController = GetComponent<CharacterController>();
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
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
            case EnemyState.Damaged:
            {
                Damaged();
                break;
            }
            case EnemyState.Die:
            {
                Die();
                break;
            }
        }
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
            Debug.Log("Idle -> Trace");
            CurrentState = EnemyState.Trace;
        }
    }

    private void Trace()
    {
        // 전이 : 플레이어와 멀어지면 -> Return
        if (Vector3.Distance(transform.position, _player.transform.position) >= FindDistance)
        {
            Debug.Log("Trace -> Return");
            CurrentState = EnemyState.Return;
            return;
        }

        // 전이 : 공격 범위 만큼 가까워 지면 -> Attack
        if (Vector3.Distance(transform.position, _player.transform.position) < AttackDistance)
        {
            Debug.Log("Trace -> Attack");
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
            Debug.Log("Return -> Idle");
            transform.position = _startPosition;
            CurrentState = EnemyState.Idle;
            return;
        }

        // 전이 : 되돌아 가는 도중 적을 찾으면 다시 Trace
        if (Vector3.Distance(transform.position, _player.transform.position) <= FindDistance)
        {
            Debug.Log("Return -> Trace");
            CurrentState = EnemyState.Trace;
            return;
        }

        // 시작 위치와되돌아간다
        Vector3 diraction = (_startPosition - transform.position).normalized;
        _characterController.Move(diraction * MoveSpeed * Time.deltaTime);
    }

    private void Attack()
    {
        // 공격한다
    }

    private void Damaged()
    {
        // 맞는다
    }

    private void Die()
    {
        // 죽는다
    }
}
