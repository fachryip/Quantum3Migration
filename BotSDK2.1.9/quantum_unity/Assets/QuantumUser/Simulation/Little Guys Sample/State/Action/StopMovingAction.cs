using System;
using System.Collections.Generic;
using Photon.Deterministic;

namespace Quantum
{
    [Serializable]
    [AssetObjectConfig(GenerateLinkingScripts = true, GenerateAssetCreateMenu = false, GenerateAssetResetMethod = false)]
    public unsafe partial class StopMovingAction : AIAction
    {
        public override unsafe void Execute(Frame f, EntityRef e, ref AIContext aiContext)
        {
            var guyComponent = f.Unsafe.GetPointer<LittleGuyComponent>(e);
            guyComponent->BotData.BotInput.Movement = FPVector2.Zero;
        }
    }
}
