using System;
using UnityEngine;
using FishNet.Connection;
using FishNet.Managing.Logging;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Serializing.Helping;
using FishNet.Transporting;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _moveLerpSpeed = 50f;
    [SerializeField] private float _jumpVelocity = 5f; 

    [SerializeField] private Animator _playerAnimator; 
    // [SerializeField] private Animator _firstPersonAnimator; 
    [SerializeField] private SkinnedMeshRenderer _playerThirdPersonMesh; 
    // [SerializeField] private GameObject _playerFirstPersonArms; 

    private CharacterController _characterController;
    private Vector2 _moveInput;
    private bool _jumpQueued;
    private float _verticalVelocity;
    private Vector3 _currentHorizontalMovement;
    private AudioManager _audioManager;
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    void Update()
    {
        if (IsOwner)
        {
            Move();
        }
    }

    public override void OnOwnershipClient(NetworkConnection prevOwner)
    {
        base.OnOwnershipClient(prevOwner);
        if (IsOwner)
        {
            var playerInput = gameObject.GetComponent<PlayerInput>();
            playerInput.enabled = true;
            playerInput.actions.FindActionMap("Player").Enable();
            // _playerFirstPersonArms.SetActive(true);

            // We don't want to draw players own model
            if (_playerThirdPersonMesh != null) {
                _playerThirdPersonMesh.enabled = false;
            }
        }
    }

    private void Move()
    {
        float tickDelta = Time.deltaTime;

        Vector3 targetMovement = Vector3.zero;
        Vector3 inputDirection = transform.TransformDirection(new Vector3(_moveInput.x, 0f, _moveInput.y));
        bool isGrounded = _characterController.isGrounded;

        if (inputDirection.magnitude > 0f)
        {
            targetMovement = inputDirection.normalized * _moveSpeed;
            var position = transform.position;
            Debug.Log("Walking");
            _audioManager.PlayContinuosSfx("walking", position);
        }
        // no slowing down if mid air
        else if (!isGrounded)
        {
            targetMovement = _currentHorizontalMovement;
        }

        // Update movement with inertia
        _currentHorizontalMovement = Vector3.MoveTowards(_currentHorizontalMovement, targetMovement, _moveLerpSpeed * tickDelta);
        Vector3 finalMovement = _currentHorizontalMovement;

        // Jumping
        if (_jumpQueued && isGrounded)
        {
            _jumpQueued = false;
            _verticalVelocity = _jumpVelocity;
            var position = transform.position;
            _audioManager.PlaySfx("hngh", position);
        }
        if (isGrounded && _verticalVelocity < 0f)
        {
            _verticalVelocity = 0f;
        }
        // Falling
        _verticalVelocity += Physics.gravity.y * tickDelta;
        _verticalVelocity = Mathf.Max(-20f, _verticalVelocity);

        finalMovement += new Vector3(0f, _verticalVelocity, 0f);
        _characterController.Move(finalMovement * tickDelta);

        // Movement animation
        if (_playerAnimator)
        {
            _playerAnimator.SetFloat("Velocity", _currentHorizontalMovement.magnitude);
        }
        // _playerAnimator.SetBool("IsGrounded", isGrounded);
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        if (!IsOwner) { return; }
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!IsOwner) { return; }
        if (context.started || context.performed)
        {
            _jumpQueued = true;
        }
        else if (context.canceled)
        {
            _jumpQueued = false;
        }
    }

}
