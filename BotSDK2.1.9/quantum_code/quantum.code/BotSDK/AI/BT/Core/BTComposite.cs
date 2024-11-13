using Photon.Deterministic;
using System;
using System.Runtime.Remoting.Contexts;

namespace Quantum
{
  public unsafe abstract partial class BTComposite : BTNode
  {
    // ========== PUBLIC MEMBERS ==================================================================================

    public AssetRefBTNode[] Children;
    public AssetRefBTService[] Services;
    [BotSDKHidden] public AssetRefBTNode TopmostDecorator;

    public BTDataIndex CurrentChildIndex;

    public bool IsDynamic;

    public BTNode[] ChildInstances
    {
      get
      {
        return _childInstances;
      }
    }

    public BTService[] ServiceInstances
    {
      get
      {
        return _serviceInstances;
      }
    }

    public override BTNodeType NodeType
    {
      get
      {
        return BTNodeType.Composite;
      }
    }

    // ========== PROTECTED MEMBERS ===============================================================================

    [NonSerialized] protected BTNode[] _childInstances;
    [NonSerialized] protected BTService[] _serviceInstances;
    [NonSerialized] protected BTNode _topmostDecoratorInstance;

    // ========== BTNode INTERFACE ================================================================================

    public override void Init(FrameThreadSafe frame, AIBlackboardComponent* blackboard, BTAgent* agent)
    {
      base.Init(frame, blackboard, agent);

      agent->AddIntData(frame, 0);

      for (Int32 i = 0; i < Services.Length; i++)
      {
        BTService service = frame.FindAsset<BTService>(Services[i].Id);
        service.Init(frame, agent, blackboard);
      }
    }

    public override void OnEnter(BTParams btParams, ref AIContext aiContext)
    {
      BotSDKEditorEvents.BT.InvokeOnNodeEnter(btParams.Entity, Guid.Value);
      SetCurrentChildId(btParams.FrameThreadSafe, 0, btParams.Agent);

      // Some Service nodes execute their Update logic upon its onwer's OnEnter
      for (Int32 i = 0; i < _serviceInstances.Length; i++)
      {
        _serviceInstances[i].OnOwnerEntered(btParams, ref aiContext);
      }
    }

    public override void OnStartedRunning(BTParams btParams, ref AIContext aiContext)
    {
      var activeServicesList = btParams.FrameThreadSafe.ResolveList<AssetRefBTService>(btParams.Agent->ActiveServices);

      for (Int32 i = 0; i < _serviceInstances.Length; i++)
      {
        _serviceInstances[i].OnEnter(btParams, ref aiContext);

        activeServicesList.Add(Services[i]);
      }

      if (IsDynamic == true)
      {
        var dynamicComposites = btParams.FrameThreadSafe.ResolveList<AssetRefBTComposite>(btParams.Agent->DynamicComposites);
        dynamicComposites.Add(this);
      }
    }

    public override void OnReset(BTParams btParams, ref AIContext aiContext)
    {
      base.OnReset(btParams, ref aiContext);

      for (Int32 i = 0; i < _childInstances.Length; i++)
        _childInstances[i].OnReset(btParams, ref aiContext);
    }

    public override void OnExit(BTParams btParams, ref AIContext aiContext)
    {
      base.OnExit(btParams, ref aiContext);

      BotSDKEditorEvents.BT.InvokeOnNodeExit(btParams.Entity, Guid.Value);

      var activeServicesList = btParams.FrameThreadSafe.ResolveList<AssetRefBTService>(btParams.Agent->ActiveServices);
      for (Int32 i = 0; i < _serviceInstances.Length; i++)
      {
        activeServicesList.Remove(Services[i]);
      }

      if (IsDynamic == true)
      {
        var dynamicComposites = btParams.FrameThreadSafe.ResolveList<AssetRefBTComposite>(btParams.Agent->DynamicComposites);
        dynamicComposites.Remove(this);
      }
    }

    public override bool OnDynamicRun(BTParams btParams, ref AIContext aiContext)
    {
      if (_topmostDecoratorInstance != null)
      {
        return _topmostDecoratorInstance.OnDynamicRun(btParams, ref aiContext);
      }

      return true;
    }

    // ========== AssetObject INTERFACE ===========================================================================

    public override void Loaded(IResourceManager resourceManager, Native.Allocator allocator)
    {
      base.Loaded(resourceManager, allocator);

      // Cache the child assets links
      _childInstances = new BTNode[Children.Length];
      for (Int32 i = 0; i < Children.Length; i++)
      {
        _childInstances[i] = (BTNode)resourceManager.GetAsset(Children[i].Id);
        _childInstances[i].Parent = this;
        _childInstances[i].ParentIndex = i;
      }

      // Cache the service assets links
      _serviceInstances = new BTService[Services.Length];
      for (Int32 i = 0; i < Services.Length; i++)
      {
        _serviceInstances[i] = (BTService)resourceManager.GetAsset(Services[i].Id);
      }

      if (TopmostDecorator != null)
      {
        _topmostDecoratorInstance = (BTDecorator)resourceManager.GetAsset(TopmostDecorator.Id);
      }
    }

    // ========== PUBLIC METHODS ==================================================================================

    public void AbortNodes(BTParams btParams, ref AIContext aiContext, BTAbort abortType, Int32 firstIndex = 0)
    {
      for (int i = firstIndex; i < _childInstances.Length; i++)
      {
        _childInstances[i].SetStatus(btParams.FrameThreadSafe, BTStatus.Abort, btParams.Agent);
        _childInstances[i].OnAbort(btParams, ref aiContext, abortType);
      }
    }

    // ========== INTERNAL METHODS ================================================================================

    internal Int32 GetCurrentChildId(FrameThreadSafe frame, BTAgent* agent)
    {
      Byte currentChild = (Byte)agent->GetIntData(frame, CurrentChildIndex.Index);
      return currentChild;
    }

    internal void SetCurrentChildId(FrameThreadSafe frame, Int32 currentIndex, BTAgent* agent)
    {
      agent->SetIntData(frame, currentIndex, CurrentChildIndex.Index);
    }
  }
}