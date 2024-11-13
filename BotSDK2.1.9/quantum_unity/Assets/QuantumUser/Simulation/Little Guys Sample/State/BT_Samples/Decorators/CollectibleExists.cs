using System;

namespace Quantum
{
  [Serializable]
  public unsafe partial class CollectibleExists : BTDecorator
  {
    public override Boolean CheckConditions(BTParams p, ref AIContext aiContext)
    {
      var f = p.Frame;
      var e = p.Entity;

      var guyComponent = f.Unsafe.GetPointer<LittleGuyComponent>(e);

      bool targetExsits = f.Exists(guyComponent->BotData.TargetCollectible);
      return targetExsits;
    }
  }
}
