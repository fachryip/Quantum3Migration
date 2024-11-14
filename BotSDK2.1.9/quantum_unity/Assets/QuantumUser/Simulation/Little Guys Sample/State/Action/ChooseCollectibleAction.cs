using Photon.Deterministic;
using System;

namespace Quantum
{
    [Serializable]
    [AssetObjectConfig(GenerateLinkingScripts = true, GenerateAssetCreateMenu = false, GenerateAssetResetMethod = false)]
    public unsafe partial class ChooseCollectibleAction : AIAction
    {
        public override unsafe void Execute(Frame f, EntityRef e, ref AIContext aiContext)
        {
            var collectibles = f.GetComponentIterator<CollectibleComponent>();

            var guyTransform = f.Unsafe.GetPointer<Transform2D>(e);

            EntityRef closestCollectible = default;
            FP min = FP.UseableMax;

            foreach (var (entity, collectible) in collectibles)
            {
                var collTransform = f.Get<Transform2D>(entity);
                var distance = (guyTransform->Position - collTransform.Position).SqrMagnitude;

                if (closestCollectible == default || distance < min)
                {
                    closestCollectible = entity;
                    min = distance;
                }
            }

            if (closestCollectible != null)
            {
                f.Unsafe.GetPointer<LittleGuyComponent>(e)->BotData.TargetCollectible = closestCollectible;
            }
        }
    }
}