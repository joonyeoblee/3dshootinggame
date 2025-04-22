using UnityEngine;
public class PlayerFire : MonoBehaviour
{
    // 목표 : 마우스 왼쪽 버튼을 누르면 카메라가 바라보는 방향으로 총을 발사하고 싶다

    // 필요 속성
    // -발사 위치
    public GameObject FirePosition;

    // - 던지는 힘
    public float MaxPower = 15f;
    public float ThrowPower;
    public float ThrowPowerPerDeltaTime = 0.3f;

    private readonly int _maxBulletCount = 50;
    private int _currentBulletCount;

    private readonly float _fireCooldown = 0.1f;
    private float _currentTime;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _currentBulletCount = _maxBulletCount;
    }

    private void Update()
    {
        if (Input.GetMouseButton(1))
        {
            // 파워 모으기
            ThrowPower += Time.deltaTime * ThrowPowerPerDeltaTime;
            UI_Main.Instance.RefreshBombGaugeSlider(ThrowPower);
        }

        // 2. 오른쪽 버튼 입력 받기
        // 0: 왼쪽, 1: 오른쪽, 2: 휠
        if (Input.GetMouseButtonUp(1))
        {
            // 3. 발사 위치에 수류탄 생성하기
            Bomb bomb = GameManager.Instance.PoolManager.GetFromPool<Bomb>();

            bomb.transform.position = FirePosition.transform.position;

            // 4. 생성된 수류탄을 카메라 방향으로 물리적인 힘 가하기
            Rigidbody bombRigidbody = bomb.GetComponent<Rigidbody>();
            bombRigidbody.AddForce(Camera.main.transform.forward * ThrowPower, ForceMode.Impulse);
            bombRigidbody.AddTorque(Vector3.one);

            // 파워 다시 원위치
            ThrowPower = 1f;
        }
        if (_currentTime > 0f)
        {
            _currentTime -= Time.deltaTime;
        }

        // 1. 왼쪽 버튼 입력 받기
        if (Input.GetMouseButton(0) && _currentTime <= 0f)
        {
            Debug.Log("좌클릭 감지됨");

            Ray ray = new Ray(FirePosition.transform.position, Camera.main.transform.forward);
            Debug.DrawRay(FirePosition.transform.position, Camera.main.transform.forward * 100f, Color.red, 1f);

            RaycastHit hitInfo;
            bool isHit = Physics.Raycast(ray, out hitInfo);
            if (isHit)
            {
                BulletEffect bulletEffect = GameManager.Instance.PoolManager.GetFromPool<BulletEffect>();

                bulletEffect.transform.position = hitInfo.point;
                bulletEffect.transform.forward = hitInfo.normal;
                bulletEffect.GetComponent<ParticleSystem>().Play();

            }
            _currentBulletCount--;
            _currentTime = _fireCooldown;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            _currentBulletCount = _maxBulletCount;
        }

        // 2. 레이케스트를 생성하고 발사 위치와 진행 방향을 설정
        // 3. 레이케스트와 부딛힌 물체의 정보를 저장할 변수 생성, 이 변수에 데이터가 있다면(부딛혔다면) 피격 이펙트 생성(표시)
        // 4. 레이케이트를 발사한 다음

        // Ray : 레이저(시작 위치, 방향)
        // RayCast : 레이저를 발사
        // RayCastHit : 레이저와 물체가 부딛혔다면 그 정보를 저장하는 구조체
    }
}
