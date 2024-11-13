using Photon.Deterministic;
using System;

namespace Quantum
{
  public unsafe abstract partial class BTService
  {
    // ========== PUBLIC MEMBERS ==================================================================================
    public FP IntervalInSec;

    /// <summary>
    /// This is not used on the Serivce's OnEnter, but rather on the OWNER node's OnEnter, as to make it possible to
    /// run a Service Update code before it descends on the tree as some child nodes might need this to poll information
    /// before descending into the tree.
    /// It is not used on the Service's OnEnter code because that is not called upon DESCENDING on the tree, but rather when
    /// bubbling up if the Leaf resulted in RUNNING
    /// </summary>
    public bool RunOnEnter;

    [BotSDKHidden] public Int32 Id;

    // ========== BTService INTERFACE =============================================================================

    public virtual void Init(FrameThreadSafe frame, BTAgent* agent, AIBlackboardComponent* blackboard)
    {
      var endTimesList = frame.ResolveList<FP>(agent->ServicesEndTimes);
      endTimesList.Add(0);
    }

    public virtual void RunUpdate(BTParams btParams, ref AIContext aiContext)
    {
      var endTime = GetEndTime(btParams.FrameThreadSafe, btParams.Agent);
      if (btParams.Frame.BotSDKGameTime() >= endTime)
      {
        OnUpdate(btParams, ref aiContext);
        SetEndTime(btParams.FrameThreadSafe, btParams.Agent);
      }
    }

    /// <summary>
    /// Called when this Service's Owner is entered (i.e Composite and Leaf nodes)
    /// </summary>
    public void OnOwnerEntered(BTParams btParams, ref AIContext aiContext)
    {
      if (RunOnEnter == true)
      {
        OnUpdate(btParams, ref aiContext);
      }
    }

    /// <summary>
    /// OnEnter for a Service is only called when such Service is added to the current context
    /// i.e when its branch results in RUNNING
    /// </summary>
    public virtual void OnEnter(BTParams btParams, ref AIContext aiContext)
    {
      SetEndTime(btParams.FrameThreadSafe, btParams.Agent);
    }

    /// <summary>
    /// Called whenever the Service is part of the current subtree
    /// and its waiting time is already over
    /// </summary>
    protected abstract void OnUpdate(BTParams btParams, ref AIContext aiContext);

    // ========== PUBLIC METHODS ==================================================================================

    public void SetEndTime(FrameThreadSafe frame, BTAgent* agent)
    {
      var endTimesList = frame.ResolveList<FP>(agent->ServicesEndTimes);
      endTimesList[Id] = ((Frame)frame).BotSDKGameTime() + IntervalInSec;
    }

    public FP GetEndTime(FrameThreadSafe frame, BTAgent* agent)
    {
      var endTime = frame.ResolveList(agent->ServicesEndTimes);
      return endTime[Id];
    }

    public static void TickServices(BTParams btParams, ref AIContext aiContext)
    {
      var activeServicesList = btParams.FrameThreadSafe.ResolveList<AssetRefBTService>(btParams.Agent->ActiveServices);

      for (int i = 0; i < activeServicesList.Count; i++)
      {
        var service = btParams.FrameThreadSafe.FindAsset<BTService>(activeServicesList[i].Id);
        try
        {
          service.RunUpdate(btParams, ref aiContext);
        }
        catch (Exception e)
        {
          Log.Error("Exception in Behaviour Tree service '{0}' ({1}) - setting node status to Failure", service.GetType().ToString(), service.Guid);
          Log.Exception(e);
        }
      }
    }
  }
}
