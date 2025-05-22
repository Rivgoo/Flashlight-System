using UnityEngine;
using Rivgo.Flashlight;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Rivgo.FlashlightSystem.Scripts
{
	[RequireComponent(typeof(AudioSource))]
	[AddComponentMenu("Rivgo/Flashlight/Flashlight Audio")]
	public class FlashlightAudio : MonoBehaviour
	{
		[Header("References")]
		[Tooltip("Sounds played when the flashlight is switched on or off.")]
		[SerializeField]
		private AudioSource _audioSource;

		[Header("Sound Clips")]
		[Tooltip("Sounds played when the flashlight is switched on or off.")]
		[SerializeField] 
		private AudioClip[] _switchSounds;

		[Tooltip("Sounds played when the flashlight starts blinking.")]
		[SerializeField] 
		private AudioClip[] _blinkingSounds;

		private IFlashlightCore _flashlightCore;
		private IFlashlightBlinker _flashlightBlinker;

		private AudioClip _lastSwitchSoundPlayed;
		private AudioClip _lastBlinkingSoundPlayed;
		private bool _isBlinkSoundLooping = false;

		private void Awake()
		{
			_flashlightCore = GetComponentInParent<IFlashlightCore>();
			if (_flashlightCore == null)
				Debug.LogError("IFlashlightCore not found on this GameObject or its parents. FlashlightAudio will not function for switch sounds.", this);

			_flashlightBlinker = GetComponentInParent<IFlashlightBlinker>() ?? GetComponentInChildren<IFlashlightBlinker>();
			if (_flashlightBlinker == null)
				Debug.LogWarning("IFlashlightBlinker not found by FlashlightAudio. Blinking-related sounds will not be played.", this);

			if (_audioSource == null)
				_audioSource = GetComponent<AudioSource>();

			if (_audioSource == null)
			{
				Debug.LogError("AudioSource component not found. FlashlightAudio requires an AudioSource.", this);
				enabled = false;
				return;
			}
		}
		private void OnEnable()
		{
			if (_flashlightCore != null)
				_flashlightCore.OnStateChanged += HandleFlashlightStateChanged;

			if (_flashlightBlinker != null)
			{
				_flashlightBlinker.OnBlinkBurstStarted += HandleBlinkBurstStarted;
				_flashlightBlinker.OnBlinkBurstEnded += HandleBlinkBurstEnded;
			}
		}
		private void OnDisable()
		{
			if (_flashlightCore != null)
				_flashlightCore.OnStateChanged -= HandleFlashlightStateChanged;

			if (_flashlightBlinker != null)
			{
				_flashlightBlinker.OnBlinkBurstStarted -= HandleBlinkBurstStarted;
				_flashlightBlinker.OnBlinkBurstEnded -= HandleBlinkBurstEnded;
			}

			if (_isBlinkSoundLooping && _audioSource != null)
			{
				_audioSource.Stop();
				_audioSource.loop = false;
				_audioSource.clip = null;
				_isBlinkSoundLooping = false;
			}
		}
		private void OnValidate()
		{
			if (_audioSource == null)
				_audioSource = GetComponent<AudioSource>();

			if (_flashlightBlinker == null && Application.isPlaying)
				_flashlightBlinker = GetComponentInParent<IFlashlightBlinker>() ?? GetComponentInChildren<IFlashlightBlinker>();

			if (_flashlightCore == null && Application.isPlaying)
				_flashlightCore = GetComponentInParent<IFlashlightCore>();
		}

		private void HandleFlashlightStateChanged(bool isOn)
		{
			PlayRandomOneShotSound(_switchSounds, ref _lastSwitchSoundPlayed);
		}
		private void HandleBlinkBurstStarted()
		{
			if (_audioSource == null || _blinkingSounds == null || _blinkingSounds.Length == 0)
				return;

			if (_isBlinkSoundLooping)
				_audioSource.Stop();

			AudioClip clipToPlay = SelectRandomClip(_blinkingSounds, ref _lastBlinkingSoundPlayed);

			if (clipToPlay != null)
			{
				_audioSource.clip = clipToPlay;
				_audioSource.loop = true;
				_audioSource.Play();
				_isBlinkSoundLooping = true;
			}
		}
		private void HandleBlinkBurstEnded()
		{
			if (_isBlinkSoundLooping && _audioSource != null)
			{
				_audioSource.Stop();
				_audioSource.loop = false;
				_audioSource.clip = null;
				_isBlinkSoundLooping = false;
			}
		}
		private AudioClip SelectRandomClip(AudioClip[] clips, ref AudioClip lastPlayedClip)
		{
			if (clips == null || clips.Length == 0)
				return null;

			if (clips.Length == 1)
				return clips[0];

			List<AudioClip> possibleClips = new List<AudioClip>();

			foreach (AudioClip clip in clips)
				if (clip != lastPlayedClip)
					possibleClips.Add(clip);

			if (possibleClips.Count > 0)
				return possibleClips[Random.Range(0, possibleClips.Count)];
			else
				return clips[Random.Range(0, clips.Length)];
		}
		private void PlayRandomOneShotSound(AudioClip[] clips, ref AudioClip lastPlayedClip)
		{
			if (_audioSource == null)
				return;

			AudioClip clipToPlay = SelectRandomClip(clips, ref lastPlayedClip);

			if (clipToPlay != null)
			{
				_audioSource.PlayOneShot(clipToPlay);
				lastPlayedClip = clipToPlay;
			}
		}
	}
}