using Photon.Deterministic;
using System;

namespace Quantum
{
  [Serializable]
  [AssetObjectConfig(GenerateAssetCreateMenu = false)]
  public unsafe partial class BTLoop : BTDecorator
  {
    // ========== PUBLIC MEMBERS ==================================================================================

    public Int32 LoopIterations;
    public Boolean LoopForever;
    public FP LoopTimeout = -FP._1;

    public BTDataIndex StartTimeIndex;
    public BTDataIndex IterationCountIndex;

    // ========== BTNode INTERFACE ================================================================================

    public override void Init(Frame frame, AIBlackboardComponent* blackboard, BTAgent* agent)
    {
      base.Init(frame, blackboard, agent);

      agent->AddFPData(frame, 0);
      agent->AddIntData(frame, 0);
    }

    public override void OnEnter(BTParams btParams, ref AIContext aiContext)
    {
      base.OnEnter(btParams, ref aiContext);

      var frame = btParams.Frame;
      var currentTime = frame.DeltaTime * frame.Number;

      btParams.Agent->SetFPData(frame, currentTime, StartTimeIndex.Index);
      btParams.Agent->SetIntData(frame, 0, IterationCountIndex.Index);
    }

    protected override BTStatus OnUpdate(BTParams btParams, ref AIContext aiContext)
    {
      var frame = btParams.Frame;

      var childResult = BTStatus.Success;
      while (DryRun(btParams, ref aiContext) == true)
      {
        if (_childInstance != null)
        {
          // Reset the subtree as this might be the second time we're executing it
          // thus the subtree needs to start as Inactive again as to properly trigger callbacks
          _childInstance.OnReset(btParams, ref aiContext);

          // Execute the subtree
          childResult = _childInstance.Execute(btParams, ref aiContext);

          // Update the amount of times we executed (iterated)
          var iteration = btParams.Agent->GetIntData(frame, IterationCountIndex.Index) + 1;
          btParams.Agent->SetIntData(frame, iteration, IterationCountIndex.Index);

          // We stop execution for now if the child is running. This is resumed later
          // when child is finished and we Execute again
          if (childResult == BTStatus.Running)
          {
            return BTStatus.Running;
          }
        }
      }

      // Return the last result of the child subtree execution
      return childResult;
    }

    internal override bool ChildCompletedRunning(BTParams btParams, BTStatus childResult)
    {
      base.ChildCompletedRunning(btParams, childResult);
      return true;
    }

    public override Boolean DryRun(BTParams btParams, ref AIContext aiContext)
    {
      if (LoopForever && LoopTimeout < FP._0)
      {
        return true;
      }
      else if (LoopForever)
      {
        var frame = btParams.Frame;
        FP startTime = btParams.Agent->GetFPData(frame, StartTimeIndex.Index);

        var currentTime = frame.DeltaTime * frame.Number;
        if (currentTime < startTime + LoopTimeout)
        {
          return true;
        }
      }
      else
      {
        var frame = btParams.Frame;
        int iteration = btParams.Agent->GetIntData(frame, IterationCountIndex.Index);
        if (iteration < LoopIterations)
        {
          return true;
        }
      }

      return false;
    }
  }
}
