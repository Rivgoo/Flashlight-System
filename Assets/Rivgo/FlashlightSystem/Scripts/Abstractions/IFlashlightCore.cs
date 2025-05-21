using System;
using UnityEngine;

namespace Rivgo.Flashlight
{
	public interface IFlashlightCore
	{
		event Action<bool> OnStateChanged;
		event Action OnTurnedOn;
		event Action OnTurnedOff;

		bool IsOn { get; }
		Light LightSource { get; }
		Transform FlashlightTransform { get; }

		void Switch();
		void TurnOn();
		void TurnOff();
	}
}