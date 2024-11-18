using System;

namespace Quantum
{
    [Serializable]
    public class AnyCollectibleExistFunction : AIFunction<bool>
    {
        public override unsafe bool Execute(Frame f, EntityRef e, ref AIContext aiContext)
        {
            var collectiblesCount = f.ComponentCount<CollectibleComponent>();
            return collectiblesCount > 0;
        }
    }
}
