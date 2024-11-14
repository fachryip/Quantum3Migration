using System;

namespace Quantum
{
    [Serializable]
    [AssetObjectConfig(GenerateLinkingScripts = true, GenerateAssetCreateMenu = false, GenerateAssetResetMethod = false)]
    public partial class AnyCollectibleExist : HFSMDecision
    {
        public override unsafe bool Decide(Frame f, EntityRef e, ref AIContext aiContext)
        {
            var collectiblesCount = f.ComponentCount<CollectibleComponent>();

            return collectiblesCount > 0;
        }
    }
}
