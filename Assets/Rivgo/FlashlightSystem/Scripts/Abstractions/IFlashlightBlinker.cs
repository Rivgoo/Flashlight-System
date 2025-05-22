using System;

namespace Rivgo.FlashlightSystem.Scripts
{
	public interface IFlashlightBlinker
	{
		event Action OnBlinkBurstStarted;
		event Action OnBlinkBurstEnded;
		event Action OnBlinkingBehaviorChanged;

		/// <summary>
		/// True if the blinking effect is active when the flashlight is on.
		/// </summary>
		bool IsBlinkingActive { get; }

		void SetBlinkingBehavior(bool active);
		void TriggerBlinkingBurst();
	}
}