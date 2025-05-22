using UnityEngine;
using System.Collections;
using Rivgo.Flashlight;

namespace Rivgo.FlashlightSystem.Scripts
{
	[RequireComponent(typeof(IFlashlightCore))]
	[AddComponentMenu("Rivgo/Flashlight/Flashlight Blinker")]
	public class FlashlightBlinker : MonoBehaviour, IFlashlightBlinker
	{
		[Header("Blinking Activation")]
		[SerializeField]
		[Tooltip("If true, the blinking effect (alternating steady light and blinking bursts) will be active when the flashlight is on.")]
		private bool _isBlinkingActive = false;

		[Header("Overall Cycle Timing")]
		[SerializeField]
		[Range(0.001f, 1000f)]
		[Tooltip("The minimum time (in seconds) the light will be in a steady state (or off if blinking is globally inactive) before a blinking burst can start.")]
		private float _minIntervalBetweenBlinkBursts = 5f;

		[SerializeField]
		[Range(0.001f, 1000f)]
		[Tooltip("The maximum time (in seconds) the light will be in a steady state (or off if blinking is globally inactive) before a blinking burst can start.")]
		private float _maxIntervalBetweenBlinkBursts = 30f;

		[Space(10)]
		[SerializeField]
		[Range(0.001f, 1000f)]
		[Tooltip("The minimum total duration (in seconds) for a blinking burst phase.")]
		private float _minBlinkingBurstDuration = 1f;

		[SerializeField]
		[Range(0.001f, 1000f)]
		[Tooltip("The maximum total duration (in seconds) for a blinking burst phase.")]
		private float _maxBlinkingBurstDuration = 5f;

		[Header("Individual Blink Settings (During a Burst)")]
		[SerializeField]
		[Range(0.001f, 10f)]
		[Tooltip("The minimum time (in seconds) the light will be effectively 'off' (low intensity) within a blinking burst.")]
		private float _minIndividualBlinkOffDuration = 0.05f;

		[SerializeField]
		[Range(0.001f, 10f)]
		[Tooltip("The maximum time (in seconds) the light will be effectively 'off' (low intensity) within a blinking burst.")]
		private float _maxIndividualBlinkOffDuration = 0.5f;

		[Space(10)]
		[SerializeField]
		[Range(0.001f, 10f)]
		[Tooltip("The minimum time (in seconds) the light will be 'on' (full intensity) within a blinking burst.")]
		private float _minIndividualBlinkOnDuration = 0.01f;

		[SerializeField]
		[Range(0.001f, 10f)]
		[Tooltip("The maximum time (in seconds) the light will be 'on' (full intensity) within a blinking burst.")]
		private float _maxIndividualBlinkOnDuration = 0.2f;

		[Header("Intensity Modulation (During a Burst)")]
		[SerializeField]
		[Range(0f, 1f)]
		[Tooltip("The minimum intensity factor the light will drop to during the 'off' part of an individual blink (0 = completely off).")]
		private float _minIntensityFactorDuringBlink = 0.0f;

		[SerializeField]
		[Range(0f, 1f)]
		[Tooltip("The maximum intensity factor the light will drop to during the 'off' part of an individual blink (0 = completely off).")]
		private float _maxIntensityFactorDuringBlink = 0.2f;

		[SerializeField]
		[Tooltip("If true, the light's intensity will be restored to its original value when all blinking effects stop or the component is disabled. Otherwise, it might stay at the last blinked intensity.")]
		private bool _restoreOriginalIntensityOnStop = true;

		private Light LightSource => _flashlight?.LightSource;
		private IFlashlightCore _flashlight;
		private Coroutine _manageBlinkingLifecycleCoroutine;
		private float _originalIntensity;

		private enum BlinkerState
		{
			Idle,
			WaitingForNextBurst,
			InBurst
		}

		private BlinkerState _currentState = BlinkerState.Idle;

		/// <summary>
		/// Sets the active state of the overall blinking behavior.
		/// If set to true, it can immediately trigger a blinking burst if the flashlight is on.
		/// If set to false, it stops all blinking and returns to steady light (if flashlight is on).
		/// </summary>
		/// <param name="active">True to activate blinking behavior, false to deactivate.</param>
		public void SetBlinkingBehavior(bool active)
		{
			if (_flashlight == null || LightSource == null)
			{
				Debug.LogWarning("FlashlightCore or LightSource not available. Cannot set blinking behavior.", this);
				_isBlinkingActive = false;
				return;
			}

			_isBlinkingActive = active;

			if (_isBlinkingActive)
			{
				if (_flashlight.IsOn && _currentState == BlinkerState.Idle)
					_currentState = BlinkerState.WaitingForNextBurst;
			}
			else
				_currentState = BlinkerState.Idle;
		}

		/// <summary>
		/// Call this to manually trigger a blinking burst if _isBlinkingActive is true.
		/// </summary>
		public void TriggerBlinkingBurst()
		{
			if (!_isBlinkingActive || _flashlight == null || LightSource == null || !_flashlight.IsOn)
			{
				Debug.LogWarning("Cannot trigger burst: Blinking is inactive, flashlight is off, or core components missing.", this);
				return;
			}

			if (_currentState == BlinkerState.WaitingForNextBurst || _currentState == BlinkerState.Idle)
				_currentState = BlinkerState.InBurst;
		}

		private void Awake()
		{
			_flashlight = GetComponent<IFlashlightCore>();

			if (_flashlight == null)
			{
				Debug.LogError("FlashlightBlinker requires an IFlashlightCore component to work.", this);
				enabled = false;

				return;
			}

			if (LightSource == null)
			{
				Debug.LogError("IFlashlightCore does not provide a valid LightSource.", this);
				enabled = false;

				return;
			}

			_flashlight.OnTurnedOn += HandleFlashlightTurnedOn;
			_flashlight.OnTurnedOff += HandleFlashlightTurnedOff;
		}

		private void OnDestroy()
		{
			if (_flashlight == null)
				return;

			_flashlight.OnTurnedOn -= HandleFlashlightTurnedOn;
			_flashlight.OnTurnedOff -= HandleFlashlightTurnedOff;
		}

		private void OnEnable()
		{
			if (_flashlight == null || LightSource == null)
			{
				Debug.LogError("FlashlightCore or LightSource not available. FlashlightBlinker cannot operate.", this);
				enabled = false;
				return;
			}

			if (_flashlight.IsOn)
			{
				_originalIntensity = LightSource.intensity;
				if (_isBlinkingActive)
				{
					_currentState = BlinkerState.WaitingForNextBurst;
				}
				else
				{
					_currentState = BlinkerState.Idle;
					RestoreIntensityAndEnsureOn();
				}
			}
			else
				_currentState = BlinkerState.Idle;

			if (_manageBlinkingLifecycleCoroutine != null)
				StopCoroutine(_manageBlinkingLifecycleCoroutine);

			_manageBlinkingLifecycleCoroutine = StartCoroutine(ManageBlinkingLifecycleRoutine());
		}

		private void OnDisable()
		{
			if (_manageBlinkingLifecycleCoroutine != null)
			{
				StopCoroutine(_manageBlinkingLifecycleCoroutine);
				_manageBlinkingLifecycleCoroutine = null;
			}

			_currentState = BlinkerState.Idle;

			if (_flashlight != null && LightSource != null && _flashlight.IsOn && _restoreOriginalIntensityOnStop)
				RestoreIntensityAndEnsureOn();
		}

		private void RestoreIntensityAndEnsureOn()
		{
			if (LightSource == null)
				return;

			LightSource.intensity = _originalIntensity;
			LightSource.enabled = true;
		}

		private void HandleFlashlightTurnedOn()
		{
			if (LightSource == null)
			{
				Debug.LogError("LightSource is null in HandleFlashlightTurnedOn. Disabling Blinker.", this);
				enabled = false;
				return;
			}

			_originalIntensity = LightSource.intensity;

			if (_isBlinkingActive)
			{
				_currentState = BlinkerState.WaitingForNextBurst;
			}
			else
			{
				_currentState = BlinkerState.Idle;
				RestoreIntensityAndEnsureOn();
			}
		}

		private void HandleFlashlightTurnedOff()
		{
			_currentState = BlinkerState.Idle;
		}

		/// <summary>
		/// Main coroutine to manage the blinking lifecycle based on the current state.
		/// </summary>
		private IEnumerator ManageBlinkingLifecycleRoutine()
		{
			if (_flashlight == null || LightSource == null)
			{
				Debug.LogError("FlashlightCore or LightSource is missing. Stopping blinking lifecycle.", this);
				enabled = false;
				yield break;
			}

			if (_flashlight.IsOn)
				_originalIntensity = LightSource.intensity;

			while (enabled)
			{
				if (!_flashlight.IsOn || !_isBlinkingActive)
				{
					_currentState = BlinkerState.Idle;
					if (_flashlight.IsOn && _restoreOriginalIntensityOnStop)
						RestoreIntensityAndEnsureOn();

					yield return new WaitUntil(() => _flashlight.IsOn && _isBlinkingActive && enabled);

					if (_flashlight.IsOn) _originalIntensity = LightSource.intensity;
					_currentState = BlinkerState.WaitingForNextBurst;
				}

				switch (_currentState)
				{
					case BlinkerState.Idle:
						if (_flashlight.IsOn && _isBlinkingActive)
							_currentState = BlinkerState.WaitingForNextBurst;
						else
							yield return null;

						break;

					case BlinkerState.WaitingForNextBurst:
						if (_flashlight.IsOn) RestoreIntensityAndEnsureOn();

						float waitDuration = Random.Range(_minIntervalBetweenBlinkBursts, _maxIntervalBetweenBlinkBursts);
						yield return StartCoroutine(WaitPhaseRoutine(waitDuration));

						if (_currentState == BlinkerState.WaitingForNextBurst && _flashlight.IsOn && _isBlinkingActive)
							_currentState = BlinkerState.InBurst;

						break;

					case BlinkerState.InBurst:
						yield return StartCoroutine(BlinkingBurstPhaseRoutine());

						if (_flashlight.IsOn && _isBlinkingActive)
							_currentState = BlinkerState.WaitingForNextBurst;
						else
							_currentState = BlinkerState.Idle;

						break;
				}
			}
		}

		private IEnumerator WaitPhaseRoutine(float duration)
		{
			float startTime = Time.time;

			while (Time.time < startTime + duration)
			{
				if (_currentState != BlinkerState.WaitingForNextBurst || !_flashlight.IsOn || !_isBlinkingActive)
					yield break;

				yield return null;
			}
		}
		private IEnumerator BlinkingBurstPhaseRoutine()
		{
			if (LightSource == null || _flashlight == null || !_flashlight.IsOn || !_isBlinkingActive)
				yield break;

			float burstDuration = Random.Range(_minBlinkingBurstDuration, _maxBlinkingBurstDuration);
			float burstEndTime = Time.time + burstDuration;

			while (Time.time < burstEndTime && _currentState == BlinkerState.InBurst && _flashlight.IsOn && _isBlinkingActive)
			{
				if (LightSource != null)
				{
					LightSource.intensity = _originalIntensity;
					LightSource.enabled = true;
				}
				float onDuration = Random.Range(_minIndividualBlinkOnDuration, _maxIndividualBlinkOnDuration);
				yield return new WaitForSeconds(onDuration);

				if (!(Time.time < burstEndTime && _currentState == BlinkerState.InBurst && _flashlight.IsOn && _isBlinkingActive)) break;

				if (LightSource != null)
				{
					float targetIntensityFactor = Random.Range(_minIntensityFactorDuringBlink, _maxIntensityFactorDuringBlink);
					LightSource.intensity = _originalIntensity * targetIntensityFactor;
					LightSource.enabled = (LightSource.intensity > 0.001f);
				}

				float offDuration = Random.Range(_minIndividualBlinkOffDuration, _maxIndividualBlinkOffDuration);
				yield return new WaitForSeconds(offDuration);
			}

			if (_flashlight.IsOn && _isBlinkingActive && LightSource != null)
				RestoreIntensityAndEnsureOn();
		}

		private void OnValidate()
		{
			if (_minIntervalBetweenBlinkBursts > _maxIntervalBetweenBlinkBursts)
				_maxIntervalBetweenBlinkBursts = _minIntervalBetweenBlinkBursts;

			if (_minBlinkingBurstDuration > _maxBlinkingBurstDuration)
				_maxBlinkingBurstDuration = _minBlinkingBurstDuration;

			if (_minIndividualBlinkOffDuration > _maxIndividualBlinkOffDuration)
				_maxIndividualBlinkOffDuration = _minIndividualBlinkOffDuration;

			if (_minIndividualBlinkOnDuration > _maxIndividualBlinkOnDuration)
				_maxIndividualBlinkOnDuration = _minIndividualBlinkOnDuration;

			if (_minIntensityFactorDuringBlink > _maxIntensityFactorDuringBlink)
				_maxIntensityFactorDuringBlink = _minIntensityFactorDuringBlink;

			_minIntensityFactorDuringBlink = Mathf.Clamp01(_minIntensityFactorDuringBlink);
			_maxIntensityFactorDuringBlink = Mathf.Clamp01(_maxIntensityFactorDuringBlink);
		}
	}
}