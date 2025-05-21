using UnityEngine;
using System.Collections;
using Rivgo.Flashlight;

namespace Rivgo.FlashlightSystem.Scripts
{
	[RequireComponent(typeof(IFlashlightCore))]
	public class FlashlightBlinker : MonoBehaviour
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

		private Light LightSource => _flashlight.LightSource;
		private IFlashlightCore _flashlight;
		private Coroutine _mainCycleCoroutine;
		private float _originalIntensity;
		private bool _isInBlinkingBurstPhase = false;

		/// <summary>
		/// Sets the active state of the overall blinking behavior.
		/// If set to true, it can immediately trigger a blinking burst if the flashlight is on.
		/// If set to false, it stops all blinking and returns to steady light (if flashlight is on).
		/// </summary>
		/// <param name="active">True to activate blinking behavior, false to deactivate.</param>
		public void SetBlinkingBehavior(bool active)
		{
			_isBlinkingActive = active;

			if (_flashlight == null || LightSource == null) return;

			if (_isBlinkingActive)
			{
				if (_flashlight.IsOn)
				{
					_isInBlinkingBurstPhase = true;
					StartMainCycle();
				}
			}
			else
			{
				StopAllBlinkingEffects();
			}
		}

		/// <summary>
		/// Call this to manually trigger a blinking burst, regardless of the current interval.
		/// The overall _isBlinkingActive must be true for this to have an effect.
		/// </summary>
		public void TriggerBlinkingBurst()
		{
			if (!_isBlinkingActive || _flashlight == null || LightSource == null || !_flashlight.IsOn) return;

			_isInBlinkingBurstPhase = true;
			if (_mainCycleCoroutine != null)
				StopCoroutine(_mainCycleCoroutine);

			_mainCycleCoroutine = StartCoroutine(MainBlinkCycleRoutine());
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

			_originalIntensity = LightSource.intensity;

			_flashlight.OnTurnedOn += HandleFlashlightTurnedOn;
			_flashlight.OnTurnedOff += HandleFlashlightTurnedOff;
		}

		private void OnDestroy()
		{
			if (_flashlight != null)
			{
				_flashlight.OnTurnedOn -= HandleFlashlightTurnedOn;
				_flashlight.OnTurnedOff -= HandleFlashlightTurnedOff;
			}

			StopAllBlinkingEffects();
		}

		private void OnEnable()
		{
			if (_flashlight == null || LightSource == null) return;

			if (_flashlight.IsOn)
			{
				_originalIntensity = LightSource.intensity;

				if (_isBlinkingActive)
					StartMainCycle();
				else
				{
					LightSource.intensity = _originalIntensity;
					LightSource.enabled = true;
				}
			}
		}

		private void OnDisable()
		{
			if (_flashlight == null || LightSource == null) return;
			StopAllBlinkingEffects();

			if (_restoreOriginalIntensityOnStop && _flashlight.IsOn)
			{
				LightSource.intensity = _originalIntensity;
				LightSource.enabled = true;
			}
		}

		private void HandleFlashlightTurnedOn()
		{
			if (LightSource == null) return;
			_originalIntensity = LightSource.intensity;

			if (_isBlinkingActive)
			{
				StartMainCycle();
			}
			else
			{
				LightSource.intensity = _originalIntensity;
				LightSource.enabled = true;
			}
		}

		private void HandleFlashlightTurnedOff()
		{
			StopAllBlinkingEffects();
		}

		private void StartMainCycle()
		{
			if (!enabled || LightSource == null || !_flashlight.IsOn || !_isBlinkingActive)
				return;

			if (_mainCycleCoroutine != null)
				StopCoroutine(_mainCycleCoroutine);

			_mainCycleCoroutine = StartCoroutine(MainBlinkCycleRoutine());
		}

		private void StopAllBlinkingEffects()
		{
			if (_mainCycleCoroutine != null)
			{
				StopCoroutine(_mainCycleCoroutine);
				_mainCycleCoroutine = null;
			}

			_isInBlinkingBurstPhase = false;

			if (LightSource != null && _flashlight != null && _flashlight.IsOn && _restoreOriginalIntensityOnStop)
			{
				LightSource.intensity = _originalIntensity;
				LightSource.enabled = true;
			}
		}

		private IEnumerator MainBlinkCycleRoutine()
		{
			if (LightSource == null) yield break;

			if (_flashlight.IsOn)
			{
				LightSource.enabled = true;
				LightSource.intensity = _originalIntensity;
			}


			while (_flashlight.IsOn && _isBlinkingActive && enabled)
			{
				if (_isInBlinkingBurstPhase)
				{
					float burstDuration = Random.Range(_minBlinkingBurstDuration, _maxBlinkingBurstDuration);
					float burstEndTime = Time.time + burstDuration;

					while (Time.time < burstEndTime && _flashlight.IsOn && _isBlinkingActive && enabled && _isInBlinkingBurstPhase)
					{
						LightSource.intensity = _originalIntensity;
						LightSource.enabled = true;
						float onDuration = Random.Range(_minIndividualBlinkOnDuration, _maxIndividualBlinkOnDuration);
						yield return new WaitForSeconds(onDuration);

						if (!(Time.time < burstEndTime && _flashlight.IsOn && _isBlinkingActive && enabled && _isInBlinkingBurstPhase)) break;

						float targetIntensityFactor = Random.Range(_minIntensityFactorDuringBlink, _maxIntensityFactorDuringBlink);
						LightSource.intensity = _originalIntensity * targetIntensityFactor;
						LightSource.enabled = (LightSource.intensity > 0.001f);

						float offDuration = Random.Range(_minIndividualBlinkOffDuration, _maxIndividualBlinkOffDuration);
						yield return new WaitForSeconds(offDuration);
					}
					_isInBlinkingBurstPhase = false;
					if (_flashlight.IsOn && _isBlinkingActive && enabled)
					{
						LightSource.intensity = _originalIntensity;
						LightSource.enabled = true;
					}
				}
				else
				{
					if (_flashlight.IsOn)
					{
						LightSource.intensity = _originalIntensity;
						LightSource.enabled = true;
					}

					float interval = Random.Range(_minIntervalBetweenBlinkBursts, _maxIntervalBetweenBlinkBursts);
					yield return new WaitForSeconds(interval);

					if (_flashlight.IsOn && _isBlinkingActive && enabled)
						_isInBlinkingBurstPhase = true;
				}
			}

			if (LightSource != null && _flashlight != null)
			{
				if (_restoreOriginalIntensityOnStop && _flashlight.IsOn)
				{
					LightSource.intensity = _originalIntensity;
					LightSource.enabled = true;
				}
			}
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