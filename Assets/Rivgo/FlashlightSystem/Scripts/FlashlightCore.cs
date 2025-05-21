using Rivgo.Flashlight;
using System;
using UnityEngine;

namespace Rivgo.FlashlightSystem.Scripts
{
	[AddComponentMenu("Rivgo/Flashlight/Flashlight Core")]
	public class FlashlightCore : MonoBehaviour, IFlashlightCore
	{
		public event Action<bool> OnStateChanged;
		public event Action OnTurnedOn;
		public event Action OnTurnedOff;
		public bool IsOn { get; private set; }
		public Light LightSource => _lightSource;
		public Transform FlashlightTransform => transform;

		[field: SerializeField]
		[Tooltip("The light source of the flashlight. This is usually a child object of the flashlight. If not set, it will be found automatically.")]
		private Light _lightSource;

		[SerializeField]
		[Tooltip("If true, the flashlight will be on when the game starts.")]
		private bool _startsOn = false;

		public void Switch()
		{
			if (IsOn)
				TurnOff();
			else
				TurnOn();
		}
		public void TurnOff()
		{
			if (!IsOn)
				return;

			IsOn = false;
			UpdateLight();

			OnStateChanged?.Invoke(IsOn);
			OnTurnedOff?.Invoke();

		}
		public void TurnOn()
		{
			if (IsOn)
				return;

			IsOn = true;
			UpdateLight();

			OnStateChanged?.Invoke(IsOn);
			OnTurnedOn?.Invoke();
		}

		private void Awake()
		{
			if (_lightSource == null)
			{
				_lightSource = GetComponentInChildren<Light>();

				if (_lightSource == null)
				{
					Debug.LogError("LightSource not found on FlashlightCore or its children!", this);
					enabled = false;

					return;
				}
			}
		}
		private void Start()
		{
			if (_startsOn) TurnOn();
			else TurnOff();
		}
		private void UpdateLight()
		{
			if (_lightSource == null)
			{
				Debug.LogError("Flashlight light is not assigned.");
				return;
			}

			_lightSource.enabled = IsOn;
		}

		private void OnValidate()
		{
			if (_lightSource == null)
			{
				_lightSource = GetComponentInChildren<Light>();

				if (_lightSource == null)
				{
					Debug.LogError("LightSource not found on FlashlightCore or its children!", this);
					enabled = false;
					return;
				}
			}
		}
	}
}