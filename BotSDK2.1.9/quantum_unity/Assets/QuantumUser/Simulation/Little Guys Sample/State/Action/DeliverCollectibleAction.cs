using Photon.Deterministic;
using System;

namespace Quantum
{
    [Serializable]
    [AssetObjectConfig(GenerateLinkingScripts = true, GenerateAssetCreateMenu = false, GenerateAssetResetMethod = false)]
    public unsafe partial class DeliverCollectibleAction : AIAction
    {
        private FPVector2 _deliveryPos = new FPVector2(0, 5);

        public override unsafe void Execute(Frame f, EntityRef e, ref AIContext aiContext)
        {
            var guyComponent = f.Unsafe.GetPointer<LittleGuyComponent>(e);
            var guyPosition = f.Get<Transform2D>(e).Position;

            guyComponent->BotData.BotInput.Movement = _deliveryPos - guyPosition;
        }
    }
}