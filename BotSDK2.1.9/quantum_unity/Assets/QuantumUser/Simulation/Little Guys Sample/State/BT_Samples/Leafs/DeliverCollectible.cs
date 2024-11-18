using Photon.Deterministic;
using System;
using System.CodeDom;

namespace Quantum
{
  [Serializable]
  public unsafe partial class DeliverCollectible : BTLeaf
  {
    protected override BTStatus OnUpdate(BTParams p, ref AIContext aiContext)
    {
      var f = p.Frame;
      var e = p.Entity;

      var guyComponent = f.Unsafe.GetPointer<LittleGuyComponent>(e);

      if (guyComponent->BotData.Loaded == false)
      {
        guyComponent->BotData.BotInput.Movement = default;
        return BTStatus.Success;
      }

      var guyPosition = f.Get<Transform2D>(e).Position;
      var deliverPosition = new FPVector2(0, 5);
      guyComponent->BotData.BotInput.Movement = deliverPosition - guyPosition;

      return BTStatus.Running;
    }
  }
}
