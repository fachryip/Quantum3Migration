using System;

namespace Quantum
{
  [Serializable]
  public class VerifyLoadedFunction : AIFunction<bool>
  {
    public override unsafe bool Execute(Frame f, EntityRef e, ref AIContext aiContext)
    {
      var guyComponent = f.Unsafe.GetPointer<LittleGuyComponent>(e);

      return guyComponent->BotData.Loaded == true;
    }
  }


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
