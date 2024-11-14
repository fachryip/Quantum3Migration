using System;
using Photon.Deterministic;

namespace Quantum
{
    [Serializable]
    [AssetObjectConfig(GenerateLinkingScripts = true, GenerateAssetCreateMenu = false, GenerateAssetResetMethod = false)]
    public partial class VerifyLoadedDecision : HFSMDecision
    {
        public override unsafe bool Decide(Frame f, EntityRef e, ref AIContext aiContext)
        {
            var guyComponent = f.Unsafe.GetPointer<LittleGuyComponent>(e);

            return guyComponent->BotData.Loaded;
        }
    }
}
