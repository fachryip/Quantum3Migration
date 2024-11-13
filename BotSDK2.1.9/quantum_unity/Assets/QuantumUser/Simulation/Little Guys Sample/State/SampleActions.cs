using System;
using System.Collections.Generic;
using Photon.Deterministic;

namespace Quantum
{
  [Serializable]
  [AssetObjectConfig(GenerateLinkingScripts = true, GenerateAssetCreateMenu = false, GenerateAssetResetMethod = false)]
  public unsafe partial class ChooseCollectibleAction : AIAction
  {
    public override unsafe void Update(Frame f, EntityRef e, ref AIContext aiContext)
    {
      var collectibles = f.GetComponentIterator<CollectibleComponent>();

      var guyTransform = f.Unsafe.GetPointer<Transform2D>(e);

      EntityRef closestCollectible = default;
      FP min = FP.UseableMax;

      foreach (var (entity, collectible) in collectibles)
      {
        var collTransform = f.Get<Transform2D>(entity);
        var distance = (guyTransform->Position - collTransform.Position).SqrMagnitude;

        if (closestCollectible == default || distance < min)
        {
          closestCollectible = entity;
          min = distance;
        }
      }

      if (closestCollectible != null)
      {
        f.Unsafe.GetPointer<LittleGuyComponent>(e)->BotData.TargetCollectible = closestCollectible;
      }
    }
  }

  [Serializable]
  [AssetObjectConfig(GenerateLinkingScripts = true, GenerateAssetCreateMenu = false, GenerateAssetResetMethod = false)]
  public unsafe partial class PickupCollectibleAction : AIAction
  {
    public override unsafe void Update(Frame f, EntityRef e, ref AIContext aiContext)
    {
      var guyComponent = f.Unsafe.GetPointer<LittleGuyComponent>(e);
      var guyPosition = f.Get<Transform2D>(e).Position;

      if (f.Exists(guyComponent->BotData.TargetCollectible) == false)
      {
        return;
      }

      var target = guyComponent->BotData.TargetCollectible;
      var targetPosition = f.Get<Transform2D>(target).Position;

      if (target != default)
      {
        guyComponent->BotData.BotInput.Movement = targetPosition - guyPosition;
      }
    }
  }

  [Serializable]
  [AssetObjectConfig(GenerateLinkingScripts = true, GenerateAssetCreateMenu = false, GenerateAssetResetMethod = false)]
  public unsafe partial class DeliverCollectibleAction : AIAction
  {
    private FPVector2 _deliveryPos = new FPVector2(0, 5);

    public override unsafe void Update(Frame f, EntityRef e, ref AIContext aiContext)
    {
      var guyComponent = f.Unsafe.GetPointer<LittleGuyComponent>(e);
      var guyPosition = f.Get<Transform2D>(e).Position;

      guyComponent->BotData.BotInput.Movement = _deliveryPos - guyPosition;
    }
  }

  [Serializable]
  [AssetObjectConfig(GenerateLinkingScripts = true, GenerateAssetCreateMenu = false, GenerateAssetResetMethod = false)]
  public unsafe partial class StopMovingAction : AIAction
  {
    public override unsafe void Update(Frame f, EntityRef e, ref AIContext aiContext)
    {
      var guyComponent = f.Unsafe.GetPointer<LittleGuyComponent>(e);
      guyComponent->BotData.BotInput.Movement = FPVector2.Zero;
    }
  }
}
