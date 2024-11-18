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

  [Serializable]
  [AssetObjectConfig(GenerateLinkingScripts = true, GenerateAssetCreateMenu = false, GenerateAssetResetMethod = false)]
  public partial class TargetExistDecision : HFSMDecision
  {
    public override unsafe bool Decide(Frame f, EntityRef e, ref AIContext aiContext)
    {
      var guyComponent = f.Unsafe.GetPointer<LittleGuyComponent>(e);

      return f.Exists(guyComponent->BotData.TargetCollectible);
    }
  }

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
