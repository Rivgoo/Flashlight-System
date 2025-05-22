using Rivgo.FlashlightSystem.Scripts.Abstractions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Rivgo.FlashlightSystem.Scripts
{
	/// <summary>
	/// Handles player input for controlling a flashlight using Unity's Input System.
	/// This component listens for a specific input action (e.g., a key press) and
	/// interacts with an <see cref="IFlashlightCore"/> service to toggle the flashlight's state.
	/// </summary>
	/// <remarks>
	/// Requires a <see cref="PlayerInput"/> component on the same GameObject to function.
	/// It also needs to find an <see cref="IFlashlightCore"/> implementation on this GameObject or its children.
	/// The input action (e.g., "SwitchFlashlight") must be defined in the Input Actions asset
	/// and connected to the <see cref="OnSwitchFlashlight"/> method via the PlayerInput component's events.
	/// </remarks>
	[RequireComponent(typeof(PlayerInput))]
	public class FlashlightInput : MonoBehaviour
	{
		private IFlashlightCore _flashlightService;

		/// <summary>
		/// Handles the 'SwitchFlashlight' input action event when triggered by the <see cref="PlayerInput"/> component.
		/// This method calls the <see cref="IFlashlightCore.Switch()"/> method on the associated flashlight service
		/// to toggle the flashlight's on/off state.
		/// </summary>
		/// <remarks>
		/// This method is designed to be invoked by the Unity Input System via the <see cref="PlayerInput"/> component's events.
		/// Ensure that an <see cref="IFlashlightCore"/> instance is available and has been found by this component
		/// (typically in <c>Awake</c> or <c>OnValidate</c>). If no service is found, an error will be logged.
		/// </remarks>
		public void OnSwitchFlashlight()
		{
			if (_flashlightService == null)
			{
				Debug.LogError("Flashlight Service (IFlashlightCore) is not assigned or found. Cannot switch flashlight.", this);
				return;
			}

			_flashlightService.Switch();
		}

		private void Awake()
		{
			if (_flashlightService == null)
			{
				_flashlightService = GetComponent<IFlashlightCore>();

				_flashlightService ??= GetComponentInChildren<IFlashlightCore>();
			}

			if (_flashlightService == null)
			{
				Debug.LogError("Flashlight Service (IFlashlightCore) not found on this GameObject or its children. Disabling FlashlightInput.", this);
				enabled = false;
			}
		}
		private void OnValidate()
		{
			if (_flashlightService == null)
			{
				_flashlightService = GetComponent<IFlashlightCore>();
				_flashlightService ??= GetComponentInChildren<IFlashlightCore>();
			}

			if (_flashlightService == null)
				if (gameObject.scene.IsValid())
					Debug.LogError("Flashlight Service (IFlashlightCore) is not assigned and could not be found on this GameObject or its children. Please assign it in the Inspector or ensure one is present.", this);
		}
	}
}