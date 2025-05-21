using UnityEngine;

namespace Rivgo.FlashlightSystem.Scripts
{
	public class FlashlightInput : MonoBehaviour
	{
		[SerializeField]
		private FlashlightCore _flashlightService;

		public void OnFlashlight()
		{
			if (_flashlightService == null)
			{
				Debug.LogError("FlashlightService is not assigned.");
				return;
			}

			_flashlightService.Switch();
		}

		private void OnValidate()
		{
			if (_flashlightService == null && TryGetComponent<FlashlightCore>(out var flashlight))
				_flashlightService = flashlight;
			else if (_flashlightService == null)
				Debug.LogError("Flashlight Service is not assigned. Please assign it in the inspector or add it to the same GameObject.");
		}
	}
}