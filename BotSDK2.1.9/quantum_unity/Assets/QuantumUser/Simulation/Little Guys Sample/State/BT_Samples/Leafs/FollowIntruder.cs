using Photon.Deterministic;

namespace Quantum
{
  [System.Serializable]
  public unsafe class FollowIntruder : BTLeaf
  {
    public AIBlackboardValueKey IntruderRef;

    protected override BTStatus OnUpdate(BTParams p, ref AIContext aiContext)
    {
      var f = p.Frame;
      var e = p.Entity;
      var bb = p.Blackboard;

      var guyComponent = f.Unsafe.GetPointer<LittleGuyComponent>(e);
      var guyPosition = f.Get<Transform2D>(e).Position;

      var targetEntity = bb->GetEntityRef(f, IntruderRef.Key);

      if (targetEntity == default)
      {
        return BTStatus.Failure;
      }
      else
      {
        var targetPosition = f.Get<Transform2D>(targetEntity).Position;
        guyComponent->BotData.BotInput.Movement = (targetPosition - guyPosition).Normalized;

        var distance = FPVector2.Distance(guyPosition, targetPosition);
        if (distance < FP._0_10)
        {
          return BTStatus.Success;
        }
        else
        {
          return BTStatus.Running;
        }
      }
    }

    public override void OnExit(BTParams p, ref AIContext aiContext)
    {
      var guyComponent = p.Frame.Unsafe.GetPointer<LittleGuyComponent>(p.Entity);
      guyComponent->BotData.BotInput.Movement = default;

      var blackboard = p.Blackboard;
      var targetEntity = blackboard->Set(p.Frame, IntruderRef.Key, default(EntityRef));
    }
  }
}
