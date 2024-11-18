﻿using Photon.Deterministic;
using System;

namespace Quantum
{
	[Serializable]
	public unsafe partial class FindCollectible : BTLeaf
	{
		protected override BTStatus OnUpdate(BTParams p, ref AIContext aiContext)
		{
      var f = p.Frame;
      var e = p.Entity;

      var collectibles = f.GetComponentIterator<CollectibleComponent>();

      var guyTransform = f.Unsafe.GetPointer<Transform2D>(e);

      EntityRef closestCollectible = default;
      FP min = FP.UseableMax;
      foreach (var (entity, collectible) in collectibles)
      {
        var collTransform = f.Get<Transform2D>(entity);
        var distance = FPVector2.Distance(guyTransform->Position, collTransform.Position);

        if (closestCollectible == default || distance < min)
        {
          closestCollectible = entity;
          min = distance;
        }
      }

      if (closestCollectible != default)
      {
        f.Unsafe.GetPointer<LittleGuyComponent>(e)->BotData.TargetCollectible = closestCollectible;
        return BTStatus.Success;
      }
      else
      {
        return BTStatus.Failure;
      }
    }
	}
}
