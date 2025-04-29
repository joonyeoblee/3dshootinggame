using UnityEngine;
public class PlayerMove : PlayerBase
{
    private static readonly int Run = Animator.StringToHash("Run");
    private static readonly int Speed = Animator.StringToHash("Speed");

    private const float GRAVITY = -9.81f;
    private const int JUMPCOUNT = 2;

    private PlayerSO _playerData;
    [SerializeField] private float _currentStamina = 10f;

    private CharacterController _characterController;
    private Vector3 _velocity;
    private float _yVelocity;
    private float _moveSpeed;

    private bool _wantsToRun;
    private bool _canRun = true;
    private bool _isRunning;

    private int _jumpCount = JUMPCOUNT;

    private Vector3 _pushDirection;
    private float _pushTimer;

    private bool _isClimbing;
    private bool _wasClimbing;
    private bool _isTouchingWall;

    public GameObject FPSCameraPosition;
    private Transform _cameraTransform;
    private Vector3 _cameraOriginalLocalPos;
    private bool _isCameraShifted;

    protected override void Start()
    {
        base.Start();

        _playerData = _player.PlayerData;
        _characterController = GetComponent<CharacterController>();
        UI_Main.Instance.RefreshStaminaSlider(_currentStamina);
        _cameraTransform = FPSCameraPosition.transform;
        _cameraOriginalLocalPos = FPSCameraPosition.transform.localPosition;
    }
    private void Update()
    {
        if(!GameManager.Instance.IsPlaying) return;

        _isTouchingWall = false;

        HandleMovement();

        HandleStamina();

        HandleWallClimb();


    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDir = new Vector3(horizontal, 0, vertical);
        _player.Animator.SetFloat("MoveAmount", moveDir.magnitude);

        moveDir = moveDir.normalized;
        moveDir = Camera.main.transform.TransformDirection(moveDir);
        moveDir.y = 0f; // 수평 방향만 적용

        _currentStamina = Mathf.Clamp(_currentStamina, 0f, 1f);
        UI_Main.Instance.RefreshStaminaSlider(_currentStamina);

        Jumping();
        Running();
        moveDir = Rolling(moveDir);

        _wasClimbing = _isClimbing;

        if (!_isClimbing)
        {
            if (_wasClimbing)
            {
                _yVelocity = 0f;
            }
            _yVelocity += GRAVITY * Time.deltaTime;
        }

        Vector3 finalMove = moveDir * _moveSpeed;
        finalMove.y = _yVelocity;

        _characterController.Move(finalMove * Time.deltaTime);

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _player.Model.transform.localPosition = new Vector3(0.058f, -1f, -0.02f);
        }
    }


    private void HandleStamina()
    {
        bool isUsingStamina = _isRunning || _isClimbing || _pushTimer > 0f;

        bool isFallingFromWall = _wasClimbing && !_isClimbing && !_characterController.isGrounded;

        if (isUsingStamina)
        {
            if (_isRunning)
                _currentStamina -= _playerData.StaminaCost * Time.deltaTime;
            if (_isClimbing)
                _currentStamina -= _playerData.StaminaCost * Time.deltaTime;
        }
        else if (!isFallingFromWall && _characterController.isGrounded && _currentStamina < 1f)
        {
            _currentStamina += _playerData.StaminaRecovery * Time.deltaTime;
        }
    }


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // 거의 수직인 벽에 닿았는지 판단
        if (hit.collider.CompareTag("Wall") && hit.normal.y < 0.1f)
        {
            _isTouchingWall = true;
        }
    }

    private void HandleWallClimb()
    {

        if (_isTouchingWall && !_characterController.isGrounded && _currentStamina > _playerData.StaminaCost)
        {
            if (!_isClimbing)
            {
                _isClimbing = true;
            }
        }
        else
        {
            if (_isClimbing)
            {
                _isClimbing = false;
            }
        }
    }

    private void Jumping()
    {
        if (_characterController.isGrounded)
        {
            if (_jumpCount != JUMPCOUNT)
            {
                // 착지 직후에만 SetModelRotation 호출
                _player.SetModelRotation();
            }

            _jumpCount = JUMPCOUNT;
            _yVelocity = 0f;
        }

        if (Input.GetButtonDown("Jump") && _jumpCount > 0)
        {
            _player.Animator.SetTrigger("Jump");
            UI_Main.Instance.Crosshair.Recoil();
            _yVelocity = _playerData.JumpPower;
            _jumpCount--;
        }
    }



    private Vector3 Rolling(Vector3 dir)
    {
        if (_pushTimer > 0f)
        {
            dir += _pushDirection * _playerData.PushPower;
            _pushTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.E) && _characterController.isGrounded && _currentStamina >= _playerData.RollStaminaCost)
        {
            AddPush(dir);
            _currentStamina -=  _playerData.RollStaminaCost;
            UI_Main.Instance.RefreshStaminaSlider(_currentStamina);
        }

        return dir;
    }

    private void Running()
    {
        Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        float inputMagnitude = moveInput.magnitude;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _wantsToRun = true;
            if (_currentStamina > 0f)
                _canRun = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _wantsToRun = false;
        }

        if (_wantsToRun && _canRun && _characterController.isGrounded && inputMagnitude > 0.1f)
        {
            if (_currentStamina > 0f)
            {
                _isRunning = true;
                _moveSpeed = _playerData.RunSpeed;
                _currentStamina -= _playerData.StaminaCost * Time.deltaTime;
                _player.Animator.SetFloat(Speed, 1.0f);

                if (!_isCameraShifted)
                {
                    FPSCameraPosition.transform.localPosition = _cameraOriginalLocalPos + new Vector3(0, 0, 0.3f);
                    _isCameraShifted = true;
                }
            }
            else
            {
                _isRunning = false;
                _canRun = false;
                _moveSpeed = _playerData.WalkSpeed;
                _player.Animator.SetFloat(Speed, 0.5f);
            }
        }
        else if (inputMagnitude > 0.1f)
        {
            _isRunning = false;
            _moveSpeed = _playerData.WalkSpeed;
            _player.Animator.SetFloat(Speed, 0.5f);

            if (_isCameraShifted)
            {
                FPSCameraPosition.transform.localPosition = _cameraOriginalLocalPos;
                _isCameraShifted = false;
            }
        }
        else
        {
            _isRunning = false;
            _moveSpeed = 0f;
            _player.Animator.SetFloat(Speed, 0f);

            if (_isCameraShifted)
            {
                FPSCameraPosition.transform.localPosition = _cameraOriginalLocalPos;
                _isCameraShifted = false;
            }

            if (!_isClimbing)
                _currentStamina += _playerData.StaminaRecovery * Time.deltaTime;
        }
    }


    private void AddPush(Vector3 direction)
    {
        _pushDirection = direction.normalized;
        _pushTimer = _playerData.PushDuration;
    }

}
