using System;

namespace Quantum
{

  public unsafe abstract partial class BTNode
  {
    // ========== PUBLIC MEMBERS ==================================================================================

    [BotSDKHidden] public String Label;
    [BotSDKHidden] public Int32 Id;

    // ========== INTERNAL MEMBERS ================================================================================

    [NonSerialized] internal BTNode Parent;
    [NonSerialized] internal Int32 ParentIndex;

    // ========== BTNode INTERFACE ================================================================================

    public abstract BTNodeType NodeType { get; }

    /// <summary>
    /// Called once, for every Node, when the BT is being initialized
    /// </summary>
    public virtual void Init(FrameThreadSafe frame, AIBlackboardComponent* blackboard, BTAgent* agent)
    {
      var statusList = frame.ResolveList(agent->NodesStatus);
      statusList.Add(0);
    }

    /// <summary>
    /// Called once, for every Node, when the BT is being initialized
    /// </summary>
    public virtual void Init(Frame frame, AIBlackboardComponent* blackboard, BTAgent* agent)
    {
      Init((FrameThreadSafe)frame, blackboard, agent);
    }

    /// <summary>
    /// Called whenever the BT execution includes this node as part of the current context
    /// </summary>
    /// <param name="btParams"></param>
    public virtual void OnEnter(BTParams btParams, ref AIContext aiContext) { }

    public virtual void OnStartedRunning(BTParams btParams, ref AIContext aiContext) { }

    /// <summary>
    /// Called when traversing the tree upwards and the node is already finished with its job.
    /// Used by Composites and Leafs to remove their Services from the list of active services
    /// as it is not anymore part of the current subtree.
    /// Dynamic Composites also remove themselves
    /// </summary>
    /// <param name="btParams"></param>
    public virtual void OnExit(BTParams btParams, ref AIContext aiContext) { }

    public virtual void OnAbort(BTParams btParams, ref AIContext aiContext, BTAbort abortType)
    {
    }

    /// <summary>
    /// Called when getting out of a sub-branch and this node is being discarded
    /// </summary>
    /// <param name="btParams"></param>
    public unsafe virtual void OnReset(BTParams btParams, ref AIContext aiContext)
    {
      SetStatus(btParams.FrameThreadSafe, BTStatus.Inactive, btParams.Agent);
    }

    /// <summary>
    /// Used by Decorators to evaluate if a condition succeeds or not.
    /// Upon success, allow the flow to continue.
    /// Upon failure, blocks the execution so another path is taken
    /// </summary>
    /// <param name="btParams"></param>
    /// <returns></returns>
    public virtual Boolean DryRun(BTParams btParams, ref AIContext aiContext)
    {
      return false;
    }

    public virtual Boolean OnDynamicRun(BTParams btParams, ref AIContext aiContext)
    {
      return true;
    }

    /// <summary>
    /// Called every tick while this Node is part of the current sub-tree.
    /// Returning "Success/Failure" will make the tree continue its execution.
    /// Returning "Running" will store this Node as the Current Node and re-execute it on the next frame
    /// unless something else interrputs
    /// </summary>
    /// <param name="btParams"></param>
    /// <returns></returns>
    protected abstract BTStatus OnUpdate(BTParams btParams, ref AIContext aiContext);

    /// <summary>
    /// When a Composite node is Updated, it only increase the current child updated
    /// when the child results in either FAIL/SUCCESS. So we need this callback
    /// to be used when the child was RUNNING and then had some result, to properly increase the current 
    /// child ID
    /// </summary>
    /// <param name="btParams"></param>
    /// <param name="childResult"></param>
    /// <returns> If the node should repeat it's execution again (as to allow for resuming a previous execution) </returns>
    internal virtual bool ChildCompletedRunning(BTParams btParams, BTStatus childResult)
    {
      return false;
    }

    // ========== PUBLIC METHODS ==================================================================================

    // -- STATUS --
    public BTStatus GetStatus(Frame frame, BTAgent* agent)
    {
      return GetStatus((FrameThreadSafe)frame, agent);
    }

    public void SetStatus(Frame frame, BTStatus status, BTAgent* agent)
    {
      SetStatus((FrameThreadSafe)frame, status, agent);
    }

    public BTStatus GetStatus(FrameThreadSafe frame, BTAgent* agent)
    {
      var nodesAndStatus = frame.ResolveList(agent->NodesStatus);
      return (BTStatus)nodesAndStatus[Id];
    }

    public void SetStatus(FrameThreadSafe frame, BTStatus status, BTAgent* agent)
    {
      var nodesAndStatus = frame.ResolveList(agent->NodesStatus);
      nodesAndStatus[Id] = (Byte)status;
    }

    public void EvaluateAbortNode(BTParams btParams)
    {
      if (btParams.Agent->AbortNodeId == Id)
      {
        btParams.Agent->AbortNodeId = 0;
      }
    }

    public BTStatus Execute(BTParams btParams, ref AIContext aiContext, bool continuingAbort = false)
    {
      var oldStatus = GetStatus(btParams.FrameThreadSafe, btParams.Agent);

      if (oldStatus == BTStatus.Success || oldStatus == BTStatus.Failure)
      {
        return oldStatus;
      }

      if (oldStatus == BTStatus.Abort)
      {
        if (btParams.Agent->IsAborting == true)
        {
          EvaluateAbortNode(btParams);
        }
        
        return oldStatus;
      }

      // If this node was inactive, this means that we're entering on it for the first time, so we call OnEnter
      // An exception from this rule is when we chose this node to continue an abort process. In that case,
      // we already executed OnEnter before, so we don't repeat it
      if (oldStatus == BTStatus.Inactive && continuingAbort == false)
      {
        OnEnter(btParams, ref aiContext);
        
        // OnEnter might trigger a Service logic if it is marked as RunOnEnter = true
        // This means that, at this point, it is possible that a Self Abort routine starts
        // If it does, then this node should not be further updated. It should return the Abort status and Exit right away
        if (btParams.Agent->IsAborting)
        {
          SetStatus(btParams.FrameThreadSafe, BTStatus.Abort, btParams.Agent);
          OnExit(btParams, ref aiContext);
          return BTStatus.Abort;
        }
      }

      var newStatus = BTStatus.Failure;
      try
      {
        newStatus = OnUpdate(btParams, ref aiContext);

        // It is possible that a Self Abort routine starts after this node's update as it can trigger Reactive Decorators logic
        // If the Abort starts, then this node should result in Abort and exit
        if (btParams.Agent->IsAborting)
        {
          newStatus = BTStatus.Abort;
          OnExit(btParams, ref aiContext);
        }

        if (newStatus == BTStatus.Success)
        {
          if (oldStatus == BTStatus.Inactive)
          {
            OnExit(btParams, ref aiContext);
          }

          BotSDKEditorEvents.BT.InvokeOnNodeSuccess(btParams.Entity, Guid.Value);
          BotSDKEditorEvents.BT.InvokeOnNodeExit(btParams.Entity, Guid.Value);
        }

        if (newStatus == BTStatus.Failure)
        {
          if (oldStatus == BTStatus.Inactive)
          {
            OnExit(btParams, ref aiContext);
          }

          BotSDKEditorEvents.BT.InvokeOnNodeFailure(btParams.Entity, Guid.Value);
          BotSDKEditorEvents.BT.InvokeOnNodeExit(btParams.Entity, Guid.Value);
        }

        SetStatus(btParams.FrameThreadSafe, newStatus, btParams.Agent);

        if (oldStatus == BTStatus.Inactive && newStatus == BTStatus.Running)
        {
          OnStartedRunning(btParams, ref aiContext);
        }

        if (newStatus == BTStatus.Running && NodeType == BTNodeType.Leaf)
        {
          // If we are a leaf, we can store the current node
          // We know that there has only one leaf node running at any time, no parallel branches possible
          btParams.Agent->Current = this;
        }
      }
      catch (Exception e)
      {
        Log.Error($"Exception in Behaviour Tree. Node name: {Label} ({Guid})");
        Log.Exception(e);
      }
      return newStatus;
    }
  }
}