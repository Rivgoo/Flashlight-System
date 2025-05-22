using System;
using UnityEngine;

namespace Rivgo.FlashlightSystem.Scripts.Abstractions
{
	/// <summary>
	/// Defines the core functionalities and properties of a flashlight.
	/// This interface provides a contract for managing the flashlight's state,
	/// accessing its light source, and its physical representation in the scene.
	/// </summary>
	public interface IFlashlightCore
	{
		/// <summary>
		/// Occurs when the flashlight's power state (on/off) changes.
		/// The boolean parameter indicates the new state: <c>true</c> if the flashlight is now on, <c>false</c> if it's off.
		/// </summary>
		event Action<bool> OnStateChanged;

		/// <summary>
		/// Occurs specifically when the flashlight is turned on.
		/// </summary>
		event Action OnTurnedOn;

		/// <summary>
		/// Occurs specifically when the flashlight is turned off.
		/// </summary>
		event Action OnTurnedOff;

		/// <summary>
		/// Gets a value indicating whether the flashlight is currently turned on.
		/// </summary>
		/// <value><c>true</c> if the flashlight is on; otherwise, <c>false</c>.</value>
		bool IsOn { get; }

		/// <summary>
		/// Gets the Unity <see cref="Light"/> component that acts as the light source for this flashlight.
		/// </summary>
		/// <value>The <see cref="Light"/> component instance.</value>
		/// <remarks>
		/// Implementers should ensure this property returns a valid <see cref="Light"/> component.
		/// </remarks>
		Light LightSource { get; }

		/// <summary>
		/// Gets the <see cref="Transform"/> of the GameObject representing the flashlight.
		/// This can be used to determine the flashlight's position and orientation in the world.
		/// </summary>
		/// <value>The flashlight's <see cref="Transform"/>.</value>
		Transform FlashlightTransform { get; }

		/// <summary>
		/// Toggles the flashlight's state. If it's on, it will be turned off, and vice-versa.
		/// </summary>
		/// <remarks>
		/// This method should trigger the <see cref="OnStateChanged"/>, <see cref="OnTurnedOn"/>,
		/// or <see cref="OnTurnedOff"/> events as appropriate.
		/// </remarks>
		void Switch();

		/// <summary>
		/// Turns the flashlight on. If it's already on, this method may have no effect.
		/// </summary>
		/// <remarks>
		/// This method should trigger the <see cref="OnStateChanged"/> and <see cref="OnTurnedOn"/>
		/// events if the state changes.
		/// </remarks>
		void TurnOn();

		/// <summary>
		/// Turns the flashlight off. If it's already off, this method may have no effect.
		/// </summary>
		/// <remarks>
		/// This method should trigger the <see cref="OnStateChanged"/> and <see cref="OnTurnedOff"/>
		/// events if the state changes.
		/// </remarks>
		void TurnOff();
	}
}