using UnityEngine;
using UnityEngine.InputSystem;
using Rivgo.Flashlight;

namespace Rivgo.FlashlightSystem.Scripts
{
	[RequireComponent(typeof(PlayerInput))]
	public class FlashlightInput : MonoBehaviour
	{
		private IFlashlightCore _flashlightService;

		/// <summary>
		/// Called by PlayerInput component when the 'SwitchFlashlight' action is performed.
		/// </summary>
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
					Debug.LogError("Flashlight Service (IFlashlightCore) is not assigned and could not be found on this GameObject or its children. Please assign it in the Inspector.", this);
		}
	}
}