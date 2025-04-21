using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private const float GRAVITY = -9.81f;
    private const int JUMPCOUNT = 2;

    CharacterController _characterController;

    [SerializeField] private PlayerSO _playerData;

    [SerializeField] private float _currentStamina = 1f;

    private Vector3 _velocity;
    private float _yVelocity;

    private bool _wantsToRun;
    private bool _canRun = true;
    private bool _isRunning;
    [SerializeField] private Vector3 _pushDirection = Vector3.zero;

    private bool _isClimbing;
    private bool _wasClimbing;
    private float _moveSpeed;
    private float _pushTimer = 0f;

    private int _jumpCount = JUMPCOUNT;

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
                _currentStamina -= _playerData.StaminaCost * Time.deltaTime;
            if (_isClimbing)
                _currentStamina -= _playerData.StaminaCost * Time.deltaTime;
        }
        else if (!isFallingFromWall && _characterController.isGrounded)
        {
            _currentStamina += _playerData.StaminaRecovery * Time.deltaTime;
        }
    }

    private void HandleWallClimb()
    {

        Vector3 origin = transform.position + Vector3.up;
        Vector3 direction = transform.forward;
        float distance = 1f;

        Debug.DrawRay(origin, direction * distance, Color.red);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance) && !_characterController.isGrounded)
        {
            if (hit.collider.CompareTag("Wall") && hit.normal.y < 0.1f && _currentStamina > _playerData.StaminaCost )
            {
                _isClimbing = true;
                Debug.Log("Climbing Wall");
                return;
            }
        }


        _isClimbing = false;
    }

    private void Jumping()
    {
        if (_characterController.isGrounded)
        {
            _jumpCount = JUMPCOUNT;
            _yVelocity = 0f; // Resetting vertical velocity when grounded
            Debug.Log("Grounded - Jump Count Reset");
        }

        if (Input.GetButtonDown("Jump") && _jumpCount > 0)
        {
            _yVelocity = _playerData.JumpPower;
            _jumpCount--;
            Debug.Log($"Jumping! Y-Velocity: {_yVelocity}");
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
                _moveSpeed = _playerData.RunSpeed;
                _currentStamina -= _playerData.StaminaCost * Time.deltaTime;
            }
            else
            {
                _isRunning = false;
                _canRun = false;
                _moveSpeed = _playerData.WalkSpeed;
            }
        }
        else
        {
            _isRunning = false;
            _moveSpeed = _playerData.WalkSpeed;
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
