using System;

namespace Rivgo.FlashlightSystem.Scripts.Abstractions
{
	/// <summary>
	/// Defines the contract for a component that manages the blinking behavior of a flashlight.
	/// This interface allows for controlling whether blinking is active, triggering manual blink bursts,
	/// and subscribing to events related to blinking state changes.
	/// </summary>
	public interface IFlashlightBlinker
	{
		/// <summary>
		/// Occurs when a blinking burst (a sequence of on/off flickers) starts.
		/// </summary>
		event Action OnBlinkBurstStarted;

		/// <summary>
		/// Occurs when a blinking burst ends, and the light either returns to a steady state or off.
		/// </summary>
		event Action OnBlinkBurstEnded;

		/// <summary>
		/// Occurs when the overall blinking behavior (e.g., enabled or disabled via <see cref="SetBlinkingBehavior"/>) changes.
		/// </summary>
		event Action OnBlinkingBehaviorChanged;

		/// <summary>
		/// Gets a value indicating whether the blinking effect is currently configured to be active
		/// if the flashlight is turned on.
		/// </summary>
		/// <value><c>true</c> if blinking is active and will occur; otherwise, <c>false</c>.</value>
		/// <remarks>
		/// Note that even if <see cref="IsBlinkingActive"/> is true, blinking will only manifest
		/// when the associated <see cref="Flashlight.IFlashlightCore"/> indicates the flashlight is on.
		/// </remarks>
		bool IsBlinkingActive { get; }

		/// <summary>
		/// Sets the overall blinking behavior for the flashlight.
		/// </summary>
		/// <param name="active">
		/// Pass <c>true</c> to enable the blinking behavior, allowing the flashlight to potentially enter blinking bursts.
		/// Pass <c>false</c> to disable blinking behavior, stopping any current blinking and preventing new bursts.
		/// </param>
		/// <remarks>
		/// Calling this method will invoke the <see cref="OnBlinkingBehaviorChanged"/> event if the state changes.
		/// If blinking is deactivated while a burst is in progress, <see cref="OnBlinkBurstEnded"/> may also be invoked.
		/// </remarks>
		void SetBlinkingBehavior(bool active);

		/// <summary>
		/// Manually initiates a blinking burst, provided that <see cref="IsBlinkingActive"/> is <c>true</c>
		/// and the flashlight is currently on.
		/// </summary>
		/// <remarks>
		/// If conditions are met, this will typically cause the <see cref="OnBlinkBurstStarted"/> event to fire.
		/// If blinking is not active or the flashlight is off, this method may have no effect or log a warning.
		/// </remarks>
		void TriggerBlinkingBurst();
	}
}