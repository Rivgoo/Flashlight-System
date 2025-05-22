using UnityEngine;
using UnityEngine.InputSystem;

namespace Rivgo.FPC
{
	/// <summary>
	/// Implements a simple First-Person Controller (FPC) for player movement and camera look.
	/// It utilizes Unity's CharacterController for physics-based movement and the Input System for handling player input.
	/// </summary>
	/// <remarks>
	/// This component requires a <see cref="UnityEngine.CharacterController"/> and a <see cref="UnityEngine.InputSystem.PlayerInput"/>
	/// component to be present on the same GameObject.
	/// Input actions for "Move", "Look", and "Jump" must be configured in an Input Actions asset and linked
	/// to the <see cref="OnMove"/>, <see cref="OnLook"/>, and <see cref="OnJump"/> methods respectively,
	/// via the PlayerInput component's events.
	/// The camera for the first-person view should typically be a child of this GameObject.
	/// </remarks>
	/// <example>
	/// <code>
	/// // To use SimpleFPC:
	/// // 1. Create a GameObject for the player.
	/// // 2. Add a CharacterController component to it.
	/// // 3. Add a PlayerInput component to it.
	/// // 4. Configure an Input Actions asset with "Move", "Look", and "Jump" actions.
	/// // 5. Assign this Input Actions asset to the PlayerInput component.
	/// // 6. Set the PlayerInput component's "Behavior" to "Invoke Unity Events".
	/// // 7. In the "Events" section of PlayerInput, map the "Move", "Look", and "Jump"
	/// //    actions to the OnMove, OnLook, and OnJump methods of this SimpleFPC script.
	/// // 8. Add a Camera as a child of the player GameObject and assign its Transform to the _cameraTransform field.
	/// // 9. Attach this SimpleFPC script to the player GameObject.
	/// </code>
	/// </example>
	[RequireComponent(typeof(CharacterController))]
	[RequireComponent(typeof(PlayerInput))]
	public class SimpleFPC : MonoBehaviour
	{
		[Header("Player Movement Settings")]
		[Tooltip("The speed of the player's movement in units per second.")]
		[SerializeField] private float _moveSpeed = 5.0f;
		[Tooltip("The height the player can jump, in units.")]
		[SerializeField] private float _jumpHeight = 1.2f;
		[Tooltip("The gravitational acceleration applied to the player. Standard Earth gravity is approximately -9.81 m/s^2.")]
		[SerializeField] private float _gravityValue = -9.81f;

		private const float _defaultGroundedVelocityY = -2.0f;
		private const float _defaultLookSensitivity = 0.1f;
		private const float _defaultMinVerticalLookAngle = -90.0f;
		private const float _defaultMaxVerticalLookAngle = 90.0f;
		private const float _maxMoveInputMagnitude = 1.0f;

		[Header("Camera Look Settings")]
		[Tooltip("The Transform of the camera used for first-person view. This camera should typically be a child of the player GameObject.")]
		[SerializeField] private Transform _cameraTransform;
		[Tooltip("Sensitivity of mouse or gamepad look input. Adjust to control how quickly the camera responds to look movements.")]
		[SerializeField] private float _lookSensitivity = _defaultLookSensitivity;
		[Tooltip("The minimum angle (in degrees) the camera can look downwards.")]
		[SerializeField] private float _minVerticalLookAngle = _defaultMinVerticalLookAngle;
		[Tooltip("The maximum angle (in degrees) the camera can look upwards.")]
		[SerializeField] private float _maxVerticalLookAngle = _defaultMaxVerticalLookAngle;
		[Tooltip("If true, the mouse cursor will be locked to the center of the screen and made invisible when the game starts. Set to false for UI interactions or debugging.")]
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
			InitializeCharacterController();
			InitializeCameraTransform();
			InitializeCursorState();
		}

		/// <summary>
		/// Initializes the CharacterController component reference.
		/// </summary>
		private void InitializeCharacterController()
			=> _characterController = GetComponent<CharacterController>();

		/// <summary>
		/// Initializes the camera transform. If not assigned in the inspector, it attempts to find a child Camera component.
		/// </summary>
		private void InitializeCameraTransform()
		{
			if (_cameraTransform != null)
				return;

			Camera childCamera = GetComponentInChildren<Camera>();

			if (childCamera != null)
			{
				_cameraTransform = childCamera.transform;
				Debug.LogWarning("Camera Transform was not assigned in the inspector. A child camera was automatically found: " + _cameraTransform.name, this);
			}
			else
				Debug.LogError("Camera Transform is not assigned in the inspector, and no child camera was found! Vertical camera look will not function correctly.", this);
		}

		/// <summary>
		/// Initializes the cursor state (locked and invisible, or visible) based on the <see cref="_lockCursor"/> setting.
		/// </summary>
		private void InitializeCursorState()
		{
			if (!_lockCursor)
				return;

			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		private void Update()
		{
			HandleGroundedState();
			HandleLook();
			HandleMovement();
			HandleJump();
			ApplyGravity();

			_jumpPressedThisFrame = false;
		}

		/// <summary>
		/// Checks if the player is grounded and resets vertical velocity if necessary to prevent accumulation.
		/// </summary>
		private void HandleGroundedState()
		{
			_isGrounded = _characterController.isGrounded;

			if (_isGrounded && _playerVelocity.y < 0)
				_playerVelocity.y = _defaultGroundedVelocityY;
		}
		/// <summary>
		/// Handles player looking around based on mouse or gamepad input.
		/// Manages horizontal rotation of the player body and vertical rotation of the camera.
		/// </summary>
		private void HandleLook()
		{
			if (_cameraTransform == null) return;

			transform.Rotate(_lookInput.x * _lookSensitivity * Vector3.up);

			_verticalLookRotation -= _lookInput.y * _lookSensitivity;
			_verticalLookRotation = Mathf.Clamp(_verticalLookRotation, _minVerticalLookAngle, _maxVerticalLookAngle);

			_cameraTransform.localRotation = Quaternion.Euler(_verticalLookRotation, 0f, 0f);
		}

		/// <summary>
		/// Handles player movement based on directional input.
		/// Moves the CharacterController in the direction specified by the input, scaled by move speed.
		/// </summary>
		private void HandleMovement()
		{
			Vector3 moveDirection = transform.right * _moveInput.x + transform.forward * _moveInput.y;

			if (moveDirection.sqrMagnitude > _maxMoveInputMagnitude)
				moveDirection.Normalize();

			_characterController.Move(_moveSpeed * Time.deltaTime * moveDirection);
		}
		/// <summary>
		/// Handles player jumping. If a jump was requested and the player is grounded, applies an upward velocity.
		/// </summary>
		private void HandleJump()
		{
			if (_jumpPressedThisFrame && _isGrounded)
				_playerVelocity.y = Mathf.Sqrt(_jumpHeight * -2.0f * _gravityValue);
		}

		/// <summary>
		/// Applies gravity to the player if they are not grounded, and moves the CharacterController with the resulting velocity.
		/// </summary>
		private void ApplyGravity()
		{
			if (!_isGrounded)
				_playerVelocity.y += _gravityValue * Time.deltaTime;

			_characterController.Move(_playerVelocity * Time.deltaTime);
		}

		/// <summary>
		/// Callback method for the "Move" input action from the Unity Input System.
		/// Updates the internal movement input vector.
		/// </summary>
		/// <param name="value">The <see cref="InputValue"/> containing the 2D vector for movement (typically X for strafe, Y for forward/backward).</param>
		public void OnMove(InputValue value)
		{
			_moveInput = value.Get<Vector2>();
		}

		/// <summary>
		/// Callback method for the "Look" input action from the Unity Input System.
		/// Updates the internal look input vector.
		/// </summary>
		/// <param name="value">The <see cref="InputValue"/> containing the 2D vector for looking (typically X for horizontal, Y for vertical).</param>
		public void OnLook(InputValue value)
		{
			_lookInput = value.Get<Vector2>();
		}

		/// <summary>
		/// Callback method for the "Jump" input action from the Unity Input System.
		/// Sets a flag indicating that a jump was pressed in the current frame.
		/// </summary>
		/// <param name="value">The <see cref="InputValue"/> representing the jump button state (pressed or not).</param>
		public void OnJump(InputValue value)
		{
			if (value.isPressed)
				_jumpPressedThisFrame = true;
		}
	}
}