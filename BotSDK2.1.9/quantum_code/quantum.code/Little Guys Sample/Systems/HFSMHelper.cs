using Photon.Deterministic;

namespace Quantum
{
  public unsafe class HFSMHelper
  {
    // Used to either initialize an entity as a bot on the beginning of the match
    // or to turn a player entity into a bot when the player gets disconnected
    public static void SetupHFSM(Frame f, EntityRef littleGuyEntity, AssetRefHFSMRoot hfsm)
    {
      var runtimeConfig = f.RuntimeConfig;
      
      var littleGuyComponent = f.Unsafe.GetPointer<LittleGuyComponent>(littleGuyEntity);
      // Used during the input reading to decide if the input comes from the player or from some bot logic
      littleGuyComponent->IsBot = true;

      // Create the HFSM Agent and pick the AIConfig, if there is any
      var hfsmAgent = new HFSMAgent();
      hfsmAgent.Config = runtimeConfig.AIConfig;

      var hfsmRoot = f.FindAsset<HFSMRoot>(hfsm.Id);
      HFSMManager.Init(f, &hfsmAgent.Data, littleGuyEntity, hfsmRoot);
      
      f.Set(littleGuyEntity, hfsmAgent);

      // Setup the blackboard
      var blackboardComponent = new AIBlackboardComponent();
      var bbInitializerAsset = f.FindAsset<AIBlackboardInitializer>(runtimeConfig.HFSMBlackboardInitializer.Id);
      AIBlackboardInitializer.InitializeBlackboard(f, &blackboardComponent, bbInitializerAsset);
      f.Set(littleGuyEntity, blackboardComponent);
    }

    public static FPVector2 GetBotMovement(Frame f, EntityRef littleGuyEntity)
    {
      var hfsmAgent = f.Unsafe.GetPointer<HFSMAgent>(littleGuyEntity);

      HFSMManager.Update(f, f.DeltaTime, &hfsmAgent->Data, littleGuyEntity);

      var littleGuyComponent = f.Unsafe.GetPointer<LittleGuyComponent>(littleGuyEntity);
      // Until this point, the Actions might already have changed the movement vector,
      // stored on the entity. Then, read it.
      return littleGuyComponent->BotData.BotInput.Movement.Normalized;
    }
  }
}