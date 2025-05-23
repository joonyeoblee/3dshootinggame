using DG.Tweening;
using UnityEngine;
using UnityEngine.VFX;
public class PlayerFire : PlayerBase
{
    // 목표 : 마우스 왼쪽 버튼을 누르면 카메라가 바라보는 방향으로 총을 발사하고 싶다

    // 필요 속성
    // -발사 위치
    public GameObject FirePosition;
    public ParticleSystem FireParticles;
    // [SerializeField] private Crosshair crosshair;
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

    public float Angle = 30f;
    public float Radius;
    public LayerMask TargetMask;
    private bool _isSlashing;
    private readonly float _slashCooldown = 2f;
    private float _slashTimer;
    [SerializeField] private VisualEffect vfx;

    public float ZoomInSize = 15f;
    public float ZoomOutSize = 60f;
    private bool _zoomMode;

    protected override void Start()
    {
        base.Start();
        _currentBulletCount = _maxBulletCount;
        UI_Main.Instance.RefreshBulletText($"{_currentBulletCount} / {_maxBulletCount}");

        _currentBombCount = _maxBombCount;
        UI_Main.Instance.RefreshBombText($"{_currentBombCount} / {_maxBombCount}");
    }

    private void Update()
    {
        if (_currentTime > 0f)
        {
            _currentTime -= Time.deltaTime;
        }

        SniperZoom();
        // TODO: 폭탄 발싸 3번으로 바꿔야함
        if (_player.BombMode)
        {
            Bomb();
        }

        if (_player.GunMode)
        {
            Fire();
        }


        if (_isSlashing)
        {
            _slashTimer -= Time.deltaTime;
            if (_slashTimer <= 0f)
            {
                _isSlashing = false;
            }
        }

        if (_player.KnifeMode && Input.GetMouseButton(0))
        {
            Slash();
        }

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

            _player.Animator.SetTrigger("Shoot");
            FireParticles.Play();

            // 조준선 퍼지기
            UI_Main.Instance.Crosshair.Recoil();

            Vector3 dir = GetBulletDirection();

            Ray ray = new Ray(Camera.main.transform.position, dir);
            Debug.DrawRay(Camera.main.transform.position, dir * 100f, Color.red, 1f);

            RaycastHit hitInfo;
            bool isHit = Physics.Raycast(ray, out hitInfo);
            if (isHit)
            {
                BulletEffect bulletEffect = GameManager.Instance.PoolManager.GetFromPool<BulletEffect>();
                Bullet bullet = GameManager.Instance.PoolManager.GetFromPool<Bullet>();

                bullet.transform.position = FirePosition.transform.position;

                bullet.gameObject.transform.DOMove(hitInfo.point, 0.1f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    bulletEffect.transform.position = hitInfo.point;
                    bulletEffect.transform.forward = hitInfo.normal;
                    bulletEffect.GetComponent<ParticleSystem>().Play();

                    bullet.PoolItem.ReturnToPoolAs<Bullet>();
                    bullet.gameObject.SetActive(false);

                    if (hitInfo.collider.gameObject.CompareTag("Enemy"))
                    {
                        Debug.Log(hitInfo.collider.gameObject.name);
                        IDamageable enemy = hitInfo.collider.GetComponent<IDamageable>();

                        Damage damage = new Damage(10, 30, gameObject);
                        enemy.TakeDamage(damage);
                    }

                });
            }
            _currentBulletCount--;
            UI_Main.Instance.RefreshBulletText($"{_currentBulletCount} / {_maxBulletCount}");
            _currentTime = _fireCooldown;

            // 총 쏘는 블럭 안에
            _currentSpread = Mathf.Min(_currentSpread + 0.5f, maxSpread);
            Camera.main.GetComponent<CameraRotate>().AddRecoil(0.4f);

        }
    }

    private void Slash()
    {
        if (_isSlashing || _slashTimer > 0)
        {
            return;
        }

        _isSlashing = true;
        _slashTimer = _slashCooldown;

        Collider[] hits = Physics.OverlapSphere(transform.position, Radius, TargetMask);

        float halfAngleRad = Angle * 0.5f * Mathf.Deg2Rad;
        float cosHalfAngle = Mathf.Cos(halfAngleRad);
        _player.Animator.SetTrigger("Slash");
        // vfx.Play(); // 또는 vfx.SendEvent("OnPlay");
        foreach (Collider hit in hits)
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

    private void SniperZoom()
    {
        if (Input.GetMouseButtonDown(1) && _player.GunMode)
        {
            _zoomMode = !_zoomMode;

            if (_zoomMode)
            {
                UI_Main.Instance.ToggleSniperImage();
                Camera.main.fieldOfView = ZoomInSize;
            }
            else
            {
                UI_Main.Instance.ToggleSniperImage();
                Camera.main.fieldOfView = ZoomOutSize;
            }
        }
    }
    [SerializeField] private string throwAnimName = "ThrowBomb";
    private void Bomb()
    {
        if (Input.GetMouseButton(0) && _currentBombCount > 0)
        {
            // 애니메이션 재생 + 멈춤 (0초 지점)
            _player.Animator.Play(throwAnimName, 0, 1.5f / 3.0f);
            _player.Animator.speed = 0f;

            // 파워 모으기
            ThrowPower += Time.deltaTime * ThrowPowerPerDeltaTime;
            UI_Main.Instance.RefreshBombGaugeSlider(ThrowPower);
        }

        if (Input.GetMouseButtonUp(0) && _currentBombCount > 0)
        {
            // 애니메이션 다시 재생
            _player.Animator.speed = 1f;

            // 수류탄 생성
            Bomb bomb = GameManager.Instance.PoolManager.GetFromPool<Bomb>();
            bomb.transform.position = FirePosition.transform.position;

            // 힘 가하기
            Rigidbody bombRigidbody = bomb.GetComponent<Rigidbody>();
            bombRigidbody.AddForce(Camera.main.transform.forward * ThrowPower, ForceMode.Impulse);
            bombRigidbody.AddTorque(Vector3.one);

            _currentBombCount--;
            UI_Main.Instance.RefreshBombText($"{_currentBombCount} / {_maxBombCount}");

            // 파워 초기화
            ThrowPower = 0f;
            UI_Main.Instance.RefreshBombGaugeSlider(ThrowPower);
        }
    }
    // private void Bomb()
    // {
    //
    //     if (Input.GetKey(KeyCode.Alpha3) && _currentBombCount > 0)
    //     {
    //         // 파워 모으기
    //         ThrowPower += Time.deltaTime * ThrowPowerPerDeltaTime;
    //         UI_Main.Instance.RefreshBombGaugeSlider(ThrowPower);
    //     }
    //
    //     // 2. 오른쪽 버튼 입력 받기
    //     // 0: 왼쪽, 1: 오른쪽, 2: 휠
    //     if (Input.GetKeyUp(KeyCode.Alpha3) && _currentBombCount > 0)
    //     {
    //         // 3. 발사 위치에 수류탄 생성하기
    //         Bomb bomb = GameManager.Instance.PoolManager.GetFromPool<Bomb>();
    //
    //         bomb.transform.position = FirePosition.transform.position;
    //
    //         // 4. 생성된 수류탄을 카메라 방향으로 물리적인 힘 가하기
    //         Rigidbody bombRigidbody = bomb.GetComponent<Rigidbody>();
    //         bombRigidbody.AddForce(Camera.main.transform.forward * ThrowPower, ForceMode.Impulse);
    //         bombRigidbody.AddTorque(Vector3.one);
    //
    //         _currentBombCount--;
    //         UI_Main.Instance.RefreshBombText($"{_currentBombCount} / {_maxBombCount}");
    //
    //         // 파워 다시 원위치
    //         ThrowPower = 0f;
    //         UI_Main.Instance.RefreshBombGaugeSlider(ThrowPower);
    //     }
    //
    // }

    private Vector3 GetBulletDirection()
    {
        Vector3 baseDir = Camera.main.transform.forward;

        float horizontal = Random.Range(-_currentSpread, _currentSpread);
        float vertical = Random.Range(-_currentSpread, _currentSpread) + _verticalRecoilOffset;

        Quaternion spreadRot = Quaternion.Euler(-vertical, horizontal, 0f);

        return spreadRot * baseDir;
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
