using System.Collections.Generic;

namespace Quantum
{
  public interface IBotSDKDebugInfo
  {
    public IBotSDKDebugInfoProvider Agent { get; set; }
    public AIBlackboardComponent BlackboardComponent { get; }
  }

  public struct BotSDKDebugInfoHFSM : IBotSDKDebugInfo
  {
    public EntityRef EntityRef;
    public HFSMAgent HFSMAgent;
    public AIBlackboardComponent DebuggedBlackboardComponent;

    IBotSDKDebugInfoProvider IBotSDKDebugInfo.Agent
    {
      get => HFSMAgent;
      set => HFSMAgent = (HFSMAgent)value;
    }

    public AIBlackboardComponent BlackboardComponent => DebuggedBlackboardComponent;
  }

  public struct BotSDKDebugInfoBT : IBotSDKDebugInfo
  {
    public EntityRef EntityRef;
    public BTAgent BTAgent;
    public AIBlackboardComponent DebuggedBlackboardComponent;

    IBotSDKDebugInfoProvider IBotSDKDebugInfo.Agent
    {
      get => BTAgent;
      set => BTAgent = (BTAgent)value;
    }

    public AIBlackboardComponent BlackboardComponent => DebuggedBlackboardComponent;
  }

  public struct BotSDKDebugInfoUT : IBotSDKDebugInfo
  {
    public EntityRef EntityRef;
    public UTAgent UTAgent;
    public AIBlackboardComponent DebuggedBlackboardComponent;

    IBotSDKDebugInfoProvider IBotSDKDebugInfo.Agent
    {
      get => UTAgent;
      set => UTAgent = (UTAgent)value;
    }
    public AIBlackboardComponent BlackboardComponent => DebuggedBlackboardComponent;
  }

  public unsafe delegate IEnumerator<IBotSDKDebugInfo> DelegateGetDebugInfo(Frame frame, EntityRef entity, void* ptr);
  public interface IBotSDKDebugInfoProvider { DelegateGetDebugInfo GetDebugInfo(); }
}
