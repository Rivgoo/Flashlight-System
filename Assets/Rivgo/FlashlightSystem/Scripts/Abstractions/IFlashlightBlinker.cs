namespace Rivgo.FlashlightSystem.Scripts
{
	public interface IFlashlightBlinker
	{
		void SetBlinkingBehavior(bool active);
		void TriggerBlinkingBurst();
	}
}