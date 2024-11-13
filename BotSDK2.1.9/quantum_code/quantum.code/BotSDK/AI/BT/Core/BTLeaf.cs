using Photon.Deterministic;
using System;

namespace Quantum
{
  [AssetObjectConfig(GenerateAssetCreateMenu = false)]
  public unsafe abstract partial class BTLeaf : BTNode
  {
    // ========== PUBLIC MEMBERS ==================================================================================

    public AssetRefBTService[] Services;
    public BTService[] ServiceInstances
    {
      get
      {
        return _serviceInstances;
      }
    }

    // ========== BTNode INTERFACE ================================================================================

    public override BTNodeType NodeType
    {
      get
      {
        return BTNodeType.Leaf;
      }
    }

    // ========== PROTECTED MEMBERS ===============================================================================

    [NonSerialized] protected BTService[] _serviceInstances;

    // ========== BTDecorator INTERFACE ===========================================================================

    public override unsafe void Init(FrameThreadSafe frame, AIBlackboardComponent* blackboard, BTAgent* agent)
    {
      base.Init(frame, blackboard, agent);

			if(Services != null)
			{
				for (int i = 0; i < Services.Length; i++)
				{
					BTService service = frame.FindAsset<BTService>(Services[i].Id);
					service.Init(frame, agent, blackboard);
				}
			}
    }

    public override void OnStartedRunning(BTParams btParams, ref AIContext aiContext)
    {
      var activeServicesList = btParams.FrameThreadSafe.ResolveList<AssetRefBTService>(btParams.Agent->ActiveServices);
      for (int i = 0; i < _serviceInstances.Length; i++)
      {
        _serviceInstances[i].OnEnter(btParams, ref aiContext);
        activeServicesList.Add(Services[i]);
      }
    }

    public override void OnEnter(BTParams btParams, ref AIContext aiContext)
    {
      base.OnEnter(btParams, ref aiContext);

      // Some Service nodes execute their Update logic upon its onwer's OnEnter
      for (Int32 i = 0; i < _serviceInstances.Length; i++)
      {
        _serviceInstances[i].OnOwnerEntered(btParams, ref aiContext);
      }

      BotSDKEditorEvents.BT.InvokeOnNodeEnter(btParams.Entity, Guid.Value);
    }

    public override void OnExit(BTParams btParams, ref AIContext aiContext)
    {
      base.OnExit(btParams, ref aiContext);

      var activeServicesList = btParams.FrameThreadSafe.ResolveList<AssetRefBTService>(btParams.Agent->ActiveServices);
      for (Int32 i = 0; i < _serviceInstances.Length; i++)
      {
        activeServicesList.Remove(Services[i]);
      }

      BotSDKEditorEvents.BT.InvokeOnNodeExit(btParams.Entity, Guid.Value);
    }

    // ========== AssetObject INTERFACE ===========================================================================

    public override void Loaded(IResourceManager resourceManager, Native.Allocator allocator)
    {
      base.Loaded(resourceManager, allocator);

      // Cache the service assets links
      _serviceInstances = new BTService[Services == null ? 0 : Services.Length];
			if(Services != null)
			{
				for (int i = 0; i < Services.Length; i++)
				{
					_serviceInstances[i] = (BTService)resourceManager.GetAsset(Services[i].Id);
				}
			}
    }
  }
}