using DG.Tweening;
using UnityEngine;
public class PlayerFire : MonoBehaviour
{
    // 목표 : 마우스 왼쪽 버튼을 누르면 카메라가 바라보는 방향으로 총을 발사하고 싶다

    // 필요 속성
    // -발사 위치
    public GameObject FirePosition;
    [SerializeField] private Crosshair crosshair;

    // - 던지는 힘
    public float MaxPower = 15f;
    public float ThrowPower;
    public float ThrowPowerPerDeltaTime = 0.3f;

    private readonly int _maxBulletCount = 50;
    private int _currentBulletCount;

    private readonly int _maxBombCount = 3;
    private int _currentBombCount;

    private readonly float _fireCooldown = 0.1f;
    private float _currentTime;

    private float _currentReloadGaugeValue;
    private Tween _reloadTween;

    [SerializeField] private float maxSpread = 15f;
    [SerializeField] private float recoilPerShot = 1f;
    [SerializeField] private float spreadRecoverySpeed = 2f;

    private float _currentSpread;
    private float _verticalRecoilOffset;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        _currentBulletCount = _maxBulletCount;
        UI_Main.Instance.RefreshBulletText($"{_currentBulletCount} / {_maxBulletCount}");

        _currentBombCount = _maxBombCount;
        UI_Main.Instance.RefreshBombText($"{_currentBombCount} / {_maxBombCount}");
    }

    private void Update()
    {
        Bomb();

        Fire();

        _currentSpread = Mathf.MoveTowards(_currentSpread, 0f, spreadRecoverySpeed * Time.deltaTime);
        _verticalRecoilOffset = Mathf.MoveTowards(_verticalRecoilOffset, 0f, spreadRecoverySpeed * Time.deltaTime);

        Reload();

        // 2. 레이케스트를 생성하고 발사 위치와 진행 방향을 설정
        // 3. 레이케스트와 부딛힌 물체의 정보를 저장할 변수 생성, 이 변수에 데이터가 있다면(부딛혔다면) 피격 이펙트 생성(표시)
        // 4. 레이케이트를 발사한 다음

        // Ray : 레이저(시작 위치, 방향)
        // RayCast : 레이저를 발사
        // RayCastHit : 레이저와 물체가 부딛혔다면 그 정보를 저장하는 구조체
    }
    private void Reload()
    {

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (_reloadTween != null && _reloadTween.IsActive())
                return;
            UI_Main.Instance.ActiveReloadSlider();
            _reloadTween = DOTween.To(() => _currentReloadGaugeValue,
                    x => _currentReloadGaugeValue = x,
                    1f,
                    2f)
                .OnUpdate(() => { UI_Main.Instance.RefreshReloadSlider(_currentReloadGaugeValue); }).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _currentBulletCount = _maxBulletCount;
                    UI_Main.Instance.RefreshBulletText($"{_currentBulletCount} / {_maxBulletCount}");
                    _reloadTween = null;
                    _currentReloadGaugeValue = 0f;
                    UI_Main.Instance.DeactiveReloadSlider();
                });

        }
    }
    private void Fire()
    {

        // 1. 왼쪽 버튼 입력 받기
        if (Input.GetMouseButton(0) && _currentTime <= 0f && _currentBulletCount > 0)
        {
            if (_reloadTween != null && _reloadTween.IsActive())
            {
                _reloadTween.Kill();
                UI_Main.Instance.DeactiveReloadSlider();
                _currentReloadGaugeValue = 0f;
                _reloadTween = null;
            }
            crosshair.Recoil();

            Vector3 dir = GetBulletDirection();

            Ray ray = new Ray(FirePosition.transform.position, dir); // ✅ 퍼짐 반영된 방향으로 발사
            Debug.DrawRay(FirePosition.transform.position, dir * 100f, Color.red, 1f);

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
            UI_Main.Instance.RefreshBulletText($"{_currentBulletCount} / {_maxBulletCount}");
            _currentTime = _fireCooldown;

            // 총 쏘는 블럭 안에
            _currentSpread = Mathf.Min(_currentSpread + 0.5f, maxSpread);
            Camera.main.GetComponent<CameraRotate>().AddRecoil(0.4f); // ✅ 카메라 위로 반동 추가

        }
    }
    private void Bomb()
    {

        if (Input.GetMouseButton(1) && _currentBombCount > 0)
        {
            // 파워 모으기
            ThrowPower += Time.deltaTime * ThrowPowerPerDeltaTime;
            UI_Main.Instance.RefreshBombGaugeSlider(ThrowPower);
        }

        // 2. 오른쪽 버튼 입력 받기
        // 0: 왼쪽, 1: 오른쪽, 2: 휠
        if (Input.GetMouseButtonUp(1) && _currentBombCount > 0)
        {
            // 3. 발사 위치에 수류탄 생성하기
            Bomb bomb = GameManager.Instance.PoolManager.GetFromPool<Bomb>();

            bomb.transform.position = FirePosition.transform.position;

            // 4. 생성된 수류탄을 카메라 방향으로 물리적인 힘 가하기
            Rigidbody bombRigidbody = bomb.GetComponent<Rigidbody>();
            bombRigidbody.AddForce(Camera.main.transform.forward * ThrowPower, ForceMode.Impulse);
            bombRigidbody.AddTorque(Vector3.one);

            _currentBombCount--;
            UI_Main.Instance.RefreshBombText($"{_currentBombCount} / {_maxBombCount}");

            // 파워 다시 원위치
            ThrowPower = 0f;
            UI_Main.Instance.RefreshBombGaugeSlider(ThrowPower);
        }
        if (_currentTime > 0f)
        {
            _currentTime -= Time.deltaTime;
        }
    }

    private Vector3 GetBulletDirection()
    {
        Vector3 baseDir = Camera.main.transform.forward;

        float horizontal = Random.Range(-_currentSpread, _currentSpread);
        float vertical = Random.Range(-_currentSpread, _currentSpread) + _verticalRecoilOffset;

        Quaternion spreadRot = Quaternion.Euler(-vertical, horizontal, 0f);

        return spreadRot * baseDir;
    }
}
