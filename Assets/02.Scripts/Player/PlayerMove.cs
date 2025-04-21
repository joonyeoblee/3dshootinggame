using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    CharacterController _characterController;

    [Header("이동")]
    public float RunSpeed = 12f;
    public float WalkSpeed = 7f;
    private float _moveSpeed;
    public float JumpPower = 5f;

    private const float GRAVITY = -9.81f;
    private const int JUMPCOUNT = 2;
    private Vector3 _velocity;
    private float _yVelocity;
    private int _jumpCount = JUMPCOUNT;
    private bool _wantsToRun;
    private bool _canRun = true;
    private bool _isRunning;

    [Header("스테미너")]
    [SerializeField] private float _currentStamina = 1f;
    [SerializeField] private float _staminaCost = 0.05f;
    [SerializeField] private float _staminaRecovery = 0.4f;
    [SerializeField] private float _rollStaminaCost = 0.3f;

    [Header("구르기")]
    [SerializeField] private Vector3 _pushDirection = Vector3.zero;
    [SerializeField] private float _pushPower = 10f;
    [SerializeField] private float _pushDuration = 0.2f;
    [SerializeField] private float _pushTimer = 0f;

    [Header("벽타기")]
    [SerializeField] private float _wallClimbCooldown = 0.5f;
    private float _wallClimbCooldownTimer = 0f;
    private bool _isStaminaDepleted = false;
    private bool _isClimbing;
    private bool _wasClimbing;


    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        UI_Main.Instance.RefreshStamina(_currentStamina);
    }

    private void Update()
    {
        HandleMovement();

        HandleStamina();

        HandleWallClimb();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 dir = new Vector3(horizontal, 0, vertical).normalized;

        dir = Camera.main.transform.TransformDirection(dir);

        _currentStamina = Mathf.Clamp(_currentStamina, 0f, 1f);
        UI_Main.Instance.RefreshStamina(_currentStamina);

        Jumping();

        Running();

        dir = Rolling(dir);

        _wasClimbing = _isClimbing;

        if (!_isClimbing)
        {
            if (_wasClimbing)
            {
                _yVelocity = 0f;
            }
            _yVelocity += GRAVITY * Time.deltaTime;
            dir.y = _yVelocity;
        }

        _characterController.Move(dir * _moveSpeed * Time.deltaTime);
    }

    private void HandleStamina()
    {
        bool isUsingStamina = _isRunning || _isClimbing || _pushTimer > 0f;

        bool isFallingFromWall = _wasClimbing && !_isClimbing && !_characterController.isGrounded;

        if (isUsingStamina)
        {
            if (_isRunning)
                _currentStamina -= _staminaCost * Time.deltaTime;
            if (_isClimbing)
                _currentStamina -= _staminaCost * Time.deltaTime;
        }
        else if (!isFallingFromWall && _characterController.isGrounded)
        {
            _currentStamina += _staminaRecovery * Time.deltaTime;
        }
    }

    private void HandleWallClimb()
    {
        if (_wallClimbCooldownTimer > 0f)
        {
            _isClimbing = false;
            return;
        }

        Vector3 origin = transform.position + Vector3.up;
        Vector3 direction = transform.forward;
        float distance = 1f;

        Debug.DrawRay(origin, direction * distance, Color.red);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance) && !_characterController.isGrounded)
        {
            if (hit.collider.CompareTag("Wall") && hit.normal.y < 0.1f && _currentStamina > 0f && !_isStaminaDepleted)
            {
                _isClimbing = true;
                return;
            }
        }

        if (_wasClimbing && !_isClimbing)
        {
            _wallClimbCooldownTimer = _wallClimbCooldown;
        }

        if (_characterController.isGrounded)
        {
            _isStaminaDepleted = false;
        }

        _isClimbing = false;
    }

    private void Jumping()
    {
        if (_characterController.isGrounded)
        {
            _jumpCount = JUMPCOUNT;
        }

        if (Input.GetButtonDown("Jump") && _jumpCount > 0)
        {
            _yVelocity = JumpPower;
            _jumpCount--;
        }
    }

    private Vector3 Rolling(Vector3 dir)
    {
        if (_pushTimer > 0f)
        {
            dir += _pushDirection * _pushPower;
            _pushTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.E) && _characterController.isGrounded && _currentStamina >= _rollStaminaCost)
        {
            AddPush(dir);
            _currentStamina -= _rollStaminaCost;
            UI_Main.Instance.RefreshStamina(_currentStamina);
        }

        return dir;
    }

    private void Running()
    {
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

        if (_wantsToRun && _canRun)
        {
            if (_currentStamina > 0f)
            {
                _isRunning = true;
                _moveSpeed = RunSpeed;
                _currentStamina -= _staminaCost * Time.deltaTime;
            }
            else
            {
                _isRunning = false;
                _canRun = false;
                _moveSpeed = WalkSpeed;
            }
        }
        else
        {
            _isRunning = false;
            _moveSpeed = WalkSpeed;
            if (!_isClimbing)
                _currentStamina += _staminaRecovery * Time.deltaTime;
        }
    }

    private void AddPush(Vector3 direction)
    {
        _pushDirection = direction.normalized;
        _pushTimer = _pushDuration;
    }
}
