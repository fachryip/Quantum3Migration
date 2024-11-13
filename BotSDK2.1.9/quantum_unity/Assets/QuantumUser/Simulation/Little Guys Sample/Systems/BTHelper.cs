using Photon.Deterministic;

namespace Quantum
{
  public unsafe class BTHelper
  {
    // Used to either initialize an entity as a bot on the beginning of the match
    // or to turn a player entity into a bot when the player gets disconnected
    public static void SetupBT(Frame f, EntityRef littleGuyEntity, AssetRef<BTRoot> bt)
    {
      var runtimeConfig = f.RuntimeConfig;

      var littleGuyComponent = f.Unsafe.GetPointer<LittleGuyComponent>(littleGuyEntity);
      // Used during the input reading to decide if the input comes from the player or from some bot logic
      littleGuyComponent->IsBot = true;

      // Create the BT Agent and pick the AIConfig, if there is any
      var btAgent = new BTAgent();
      f.Set(littleGuyEntity, btAgent);

      var btRoot = f.FindAsset<BTRoot>(bt.Id);
      BTManager.Init(f, littleGuyEntity, btRoot);

      // Setup the blackboard
      var blackboardComponent = new AIBlackboardComponent();
      var bbInitializerAsset = f.FindAsset<AIBlackboardInitializer>(runtimeConfig.BTBlackboardInitializer.Id);
      AIBlackboardInitializer.InitializeBlackboard(f, &blackboardComponent, bbInitializerAsset);
      f.Set(littleGuyEntity, blackboardComponent);
    }

    public static FPVector2 GetBotMovement(Frame f, EntityRef littleGuyEntity)
    {
      var btAgent = f.Unsafe.GetPointer<BTAgent>(littleGuyEntity);

      var blackboardComponent = f.Unsafe.GetPointer<AIBlackboardComponent>(littleGuyEntity);
      BTManager.Update(f, littleGuyEntity, blackboardComponent);

      var littleGuyComponent = f.Unsafe.GetPointer<LittleGuyComponent>(littleGuyEntity);

      // Until this point, the Actions might already have changed the movement vector,
      // stored on the entity. Then, read it.
      return littleGuyComponent->BotData.BotInput.Movement.Normalized;
    }
  }
}