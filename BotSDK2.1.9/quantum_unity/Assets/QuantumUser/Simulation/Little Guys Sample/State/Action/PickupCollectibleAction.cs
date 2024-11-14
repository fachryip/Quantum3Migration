using System;

namespace Quantum
{
    [Serializable]
    [AssetObjectConfig(GenerateLinkingScripts = true, GenerateAssetCreateMenu = false, GenerateAssetResetMethod = false)]
    public unsafe partial class PickupCollectibleAction : AIAction
    {
        public override unsafe void Execute(Frame f, EntityRef e, ref AIContext aiContext)
        {
            var guyComponent = f.Unsafe.GetPointer<LittleGuyComponent>(e);
            var guyPosition = f.Get<Transform2D>(e).Position;

            if (f.Exists(guyComponent->BotData.TargetCollectible) == false)
            {
                return;
            }

            var target = guyComponent->BotData.TargetCollectible;
            var targetPosition = f.Get<Transform2D>(target).Position;

            if (target != default)
            {
                guyComponent->BotData.BotInput.Movement = targetPosition - guyPosition;
            }
        }
    }
}