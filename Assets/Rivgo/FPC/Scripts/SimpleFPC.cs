using UnityEngine;
using UnityEngine.InputSystem;

namespace Rivgo.FPC
{
	[RequireComponent(typeof(CharacterController))]
	[RequireComponent(typeof(PlayerInput))]
	public class SimpleFPC : MonoBehaviour
	{
		[Header("Player Movement Settings")]
		[Tooltip("The speed of the player's movement.")]
		[SerializeField] private float _moveSpeed = 5.0f;
		[Tooltip("The height of the player's jump.")]
		[SerializeField] private float _jumpHeight = 1.2f;
		[Tooltip("The value of gravity. The standard value is -9.81, but can be changed.")]
		[SerializeField] private float _gravityValue = -9.81f;

		[Header("Camera Look Settings")]
		[Tooltip("Transform the camera that is a child of the player. Required for vertical viewing.")]
		[SerializeField] private Transform _cameraTransform;
		[Tooltip("Mouse/gamepad sensitivity for review.")]
		[SerializeField] private float _lookSensitivity = 0.1f;
		[Tooltip("The minimum vertical viewing angle of the camera (in degrees).")]
		[SerializeField] private float _minVerticalLookAngle = -90.0f;
		[Tooltip("The maximum vertical viewing angle of the camera (in degrees).")]
		[SerializeField] private float _maxVerticalLookAngle = 90.0f;
		[Tooltip("Whether to lock the cursor when the game starts. If false, the cursor will be visible and not locked.")]
		[SerializeField] private bool _lockCursor = true;

		private CharacterController _characterController;

		private Vector2 _moveInput;
		private Vector2 _lookInput;
		private bool _jumpPressedThisFrame;

		private Vector3 _playerVelocity;
		private float _verticalLookRotation;
		private bool _isGrounded;

		private void Awake()
		{
			_characterController = GetComponent<CharacterController>();

			if (_cameraTransform == null)
			{
				Camera childCamera = GetComponentInChildren<Camera>();
				if (childCamera != null)
				{
					_cameraTransform = childCamera.transform;
					Debug.LogWarning("Camera Transform was not assigned in the inspector. A child camera was automatically found: " + _cameraTransform.name, this);
				}
				else
				{
					Debug.LogError("Camera Transform is not assigned in the inspector, and the child camera is not found! The vertical view will not work.", this);
				}
			}

			if (_lockCursor)
			{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
		}
		private void Update()
		{
			HandleGroundedState();
			HandleLook();
			HandleMovement();
			HandleJumpAndGravity();

			_jumpPressedThisFrame = false;
		}

		private void HandleGroundedState()
		{
			_isGrounded = _characterController.isGrounded;

			if (_isGrounded && _playerVelocity.y < 0)
				_playerVelocity.y = -2.0f;
		}
		private void HandleLook()
		{
			if (_cameraTransform == null) return;

			transform.Rotate(_lookInput.x * _lookSensitivity * Vector3.up);

			_verticalLookRotation -= _lookInput.y * _lookSensitivity;
			_verticalLookRotation = Mathf.Clamp(_verticalLookRotation, _minVerticalLookAngle, _maxVerticalLookAngle);

			_cameraTransform.localRotation = Quaternion.Euler(_verticalLookRotation, 0f, 0f);
		}
		private void HandleMovement()
		{
			Vector3 moveDirection = transform.right * _moveInput.x + transform.forward * _moveInput.y;

			if (moveDirection.sqrMagnitude > 1.0f)
				moveDirection.Normalize();

			_characterController.Move(_moveSpeed * Time.deltaTime * moveDirection);
		}
		private void HandleJumpAndGravity()
		{
			if (_jumpPressedThisFrame && _isGrounded)
				_playerVelocity.y = Mathf.Sqrt(_jumpHeight * -2.0f * _gravityValue);

			if (!_isGrounded)
				_playerVelocity.y += _gravityValue * Time.deltaTime;

			_characterController.Move(_playerVelocity * Time.deltaTime);
		}

		public void OnMove(InputValue value)
		{
			_moveInput = value.Get<Vector2>();
		}
		public void OnLook(InputValue value)
		{
			_lookInput = value.Get<Vector2>();
		}
		public void OnJump(InputValue value)
		{
			if (value.isPressed)
				_jumpPressedThisFrame = true;
		}
	}
}