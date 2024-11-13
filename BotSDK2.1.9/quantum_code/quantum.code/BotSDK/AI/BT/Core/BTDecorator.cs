﻿using Photon.Deterministic;
using System;

namespace Quantum
{
  [AssetObjectConfig(GenerateAssetCreateMenu = false)]
  public abstract unsafe partial class BTDecorator : BTNode
  {
    // ========== PUBLIC MEMBERS ==================================================================================
    [BotSDKHidden] public AssetRefBTNode Child;
    public BTAbort AbortType;

    public BTNode ChildInstance
    {
      get
      {
        return _childInstance;
      }
    }

    // ========== BTNode INTERFACE ================================================================================

    public override BTNodeType NodeType
    {
      get
      {
        return BTNodeType.Decorator;
      }
    }

    // ========== PROTECTED MEMBERS ===============================================================================

    [NonSerialized] protected BTNode _childInstance;

    // ========== BTDecorator INTERFACE ===========================================================================

    public override void OnReset(BTParams btParams, ref AIContext aiContext)
    {
      base.OnReset(btParams, ref aiContext);

      if (_childInstance != null)
        _childInstance.OnReset(btParams, ref aiContext);

			BotSDKEditorEvents.BT.InvokeOnDecoratorReset(btParams.Entity, Guid.Value);
		}

    protected override BTStatus OnUpdate(BTParams btParams, ref AIContext aiContext)
    {
      if (DryRun(btParams, ref aiContext) == true)
      {
        BotSDKEditorEvents.BT.InvokeOnDecoratorChecked(btParams.Entity, Guid.Value, true);

        if (_childInstance != null)
        {
          var childResult = _childInstance.Execute(btParams, ref aiContext);
          if (childResult == BTStatus.Abort)
          {
            EvaluateAbortNode(btParams);
            SetStatus(btParams.FrameThreadSafe, BTStatus.Abort, btParams.Agent);
            return BTStatus.Abort;
          }

          return childResult;
        }

        return BTStatus.Success;
      }

      BotSDKEditorEvents.BT.InvokeOnDecoratorChecked(btParams.Entity, Guid.Value, false);

      return BTStatus.Failure;
    }

    public override bool OnDynamicRun(BTParams btParams, ref AIContext aiContext)
    {
      var result = DryRun(btParams, ref aiContext);
      if (result == false)
      {
        return false;
      }
      else if (ChildInstance.NodeType != BTNodeType.Decorator)
      {
        return true;
      }
      else
      {
        return ChildInstance.OnDynamicRun(btParams, ref aiContext);
      }
    }

    // ========== AssetObject INTERFACE ===========================================================================

    public override void Loaded(IResourceManager resourceManager, Native.Allocator allocator)
    {
      base.Loaded(resourceManager, allocator);

      // Cache the child
			if(Child != null)
			{
				_childInstance = (BTNode)resourceManager.GetAsset(Child.Id);
				_childInstance.Parent = this;
			}
    }
  }
}