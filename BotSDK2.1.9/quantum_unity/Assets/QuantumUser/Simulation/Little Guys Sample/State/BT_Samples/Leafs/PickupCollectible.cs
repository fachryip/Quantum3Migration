using System;

namespace Quantum
{
	[Serializable]
	public unsafe partial class PickupCollectible : BTLeaf
	{
		protected override BTStatus OnUpdate(BTParams p, ref AIContext aiContext)
		{
      var f = p.Frame;
      var e = p.Entity;

      var guyComponent = f.Unsafe.GetPointer<LittleGuyComponent>(e);
      var guyPosition = f.Get<Transform2D>(e).Position;

      // If the Bot is Loaded, it already has a collectible
      // Bot Succeeded
      if(guyComponent->BotData.Loaded == true)
      {
        return BTStatus.Success;
      }

      if (f.Exists(guyComponent->BotData.TargetCollectible) == false)
      {
        return BTStatus.Failure;
      }

      // Otherwise, the Bot is still trying, so Bot is Running
      var target = guyComponent->BotData.TargetCollectible;
      var targetPosition = f.Get<Transform2D>(target).Position;

      guyComponent->BotData.BotInput.Movement = (targetPosition - guyPosition).Normalized;
      
      return BTStatus.Running;
    }
	}
}
