using Photon.Deterministic;
using System;

namespace Quantum
{
  [Serializable]
  [AssetObjectConfig(GenerateAssetCreateMenu = false)]
  public unsafe partial class BTCooldown : BTDecorator
  {
    // ========== PUBLIC MEMBERS ==================================================================================

    // How many time should we wait
    public FP CooldownTime;

    // An indexer so we know when the time started counting
    public BTDataIndex StartTimeIndex;


    // ========== BTNode INTERFACE ================================================================================

    public override void Init(Frame frame, AIBlackboardComponent* blackboard, BTAgent* agent)
    {
      base.Init(frame, blackboard, agent);

      // We allocate space on the BTAgent so we can store the Start Time
      agent->AddFPData(frame, 0);
      agent->AddIntData(frame, 0);

      agent->SetFPData(frame, -CooldownTime, StartTimeIndex.Index);
    }

    public override void OnExit(BTParams btParams, ref AIContext aiContext)
    {
      base.OnExit(btParams, ref aiContext);

      var frame = btParams.Frame;

      var currentTime = btParams.Frame.DeltaTime * btParams.Frame.Number;
      FP startTime = btParams.Agent->GetFPData(frame, StartTimeIndex.Index);
      if (currentTime >= startTime + CooldownTime)
      {
        btParams.Agent->SetFPData(frame, currentTime, StartTimeIndex.Index);
      }
    }

    // We get the Start Time stored on the BTAgent, then we check if the time + cooldown is already over
    // If it is not over, then we return False, blocking the execution of the children nodes
    public override Boolean DryRun(BTParams btParams, ref AIContext aiContext)
    {
      var frame = btParams.Frame;

      var entity = btParams.Entity;
      FP startTime = btParams.Agent->GetFPData(frame, StartTimeIndex.Index);

      var currentTime = btParams.Frame.DeltaTime * btParams.Frame.Number;

      return currentTime >= startTime + CooldownTime;
    }
  }
}
