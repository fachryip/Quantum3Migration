using System;

namespace Quantum
{
    [Serializable]
    public class CollectibleExistsFunction : AIFunction<bool>
    {
        public override unsafe bool Execute(Frame f, EntityRef e, ref AIContext aiContext)
        {
            var guyComponent = f.Unsafe.GetPointer<LittleGuyComponent>(e);

            bool targetExsits = f.Exists(guyComponent->BotData.TargetCollectible);
            return targetExsits;
        }
    }
}