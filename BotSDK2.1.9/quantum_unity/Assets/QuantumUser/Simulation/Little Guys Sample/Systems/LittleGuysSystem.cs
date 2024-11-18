using Photon.Deterministic;

namespace Quantum
{
  public unsafe class LittleGuysSystem : SystemMainThread, ISignalOnPlayerDataSet
  {
    //private GOAPUpdater _goapUpdater = new GOAPUpdater();

    #region Player Joining
    // When a player joins from this callback, create a non-bot entity for him
    public void OnPlayerDataSet(Frame f, PlayerRef player)
    {
      if (MainSystem.IsSampleLoaded(f, "Little Guys Sample") == false) return;

      // First, check if there is already an entity with this PlayerRef
      // If there is, do not create a new entity for the joining player, it will just take the entity's control
      bool alreadyCreated = PlayerEntityExists(f, player);
      if (!alreadyCreated)
        CreatePlayerEntity(f, player);
    }

    // Checks if there is already an entity with this PlayerRef
    private bool PlayerEntityExists(Frame f, PlayerRef playerRef)
    {
      var littleGuys = f.GetComponentIterator<LittleGuyComponent>();
      foreach (var (entity, littleGuy) in littleGuys)
      {
        if (littleGuy.PlayerRef == playerRef)
        {
          return true;
        }
      }

      return false;
    }

    private void CreatePlayerEntity(Frame f, PlayerRef player)
    {
      var littleGuy = CreateLittleGuy(f);

      // As this entity is created from the OnPlayerDataSet callback,
      // store the PlayerRef to get the correct input later.
      // This entity CAN eventually be controlled by a bot during disconnections,
      // so storing the PlayerRef is important as the player might connect back to the game again
      // As this is supposed to be controlled by a Player, there is no need to initialize its AI here
      // Only if the game supports bot replacement, which is done on the "UpdateIsBot" method
      var littleGuyComponent = f.Unsafe.GetPointer<LittleGuyComponent>(littleGuy);
      littleGuyComponent->PlayerRef = player;
    }
    #endregion

    #region Creating Bots
    public override void OnInit(Frame f)
    {
      if (MainSystem.IsSampleLoaded(f, "Little Guys Sample") == false) return;

      if (f.RuntimeConfig.HFSMNpcs != null)
      {
        for (int i = 0; i < f.RuntimeConfig.HFSMNpcs.Length; i++)
        {
          CreateBot_HFSM(f, f.RuntimeConfig.HFSMNpcs[i]);
        }
      }

      if (f.RuntimeConfig.BTNpcs != null)
      {
        for (int i = 0; i < f.RuntimeConfig.BTNpcs.Length; i++)
        {
          CreateBot_BT(f, f.RuntimeConfig.BTNpcs[i]);
        }
      }
    }

    private void FillRoomWithBots(Frame f)
    {
      // Create as many Bot entities as needed, accordingly
      // with information gathered from the Lobby
      // As no PlayerRef is being passed, these bots are totally agnostic to the existence of Players
      // So players cannot take control of these entities. 
      // They will be Bots from the beginning to the end of the match
      for (int i = 0; i < f.PlayerCount; i++)
      {
        var needed = CheckIfNeeded(f, i);

        if (needed)
        {
          CreateBot_HFSM(f, f.RuntimeConfig.ReplacementHFSM);
        }
      }
    }

    // Check among the LittleGuys created, which PlayerRefs are not set in any of them
    // In that case, a bot needs to fill that position on the room
    private bool CheckIfNeeded(Frame f, PlayerRef playerRef)
    {
      var littleGuys = f.GetComponentIterator<LittleGuyComponent>();
      foreach (var (entity, littleGuy) in littleGuys)
      {
        if (littleGuy.PlayerRef == playerRef)
        {
          return false;
        }
      }

      return true;
    }

    // Method used to create a new entity for a Player OR a Bot.
    // PS: Bots are NOT tied to players/RuntimePlayers at all.
    // If the entity created will be controlled by a Player, then its PlayerRef is necessary
    // as further the code will use it in order to get the player input.
    // If the entity will be controlled by a Bot during the whole match (which in this case means 
    // that the room was filled by bots), then the PlayerRef on the entity will be the default one.
    private void CreateBot_HFSM(Frame f, AssetRef<HFSMRoot> hfsm)
    {
      var littleGuy = CreateLittleGuy(f);

      // As this entity will be controlled by a bot,
      // initialize the HFSM right away and don't touch the PlayerRef field so it remains with default
      // This bot is agnostic to the existence of players, so it will never
      // switch controls with any player.
      HFSMHelper.SetupHFSM(f, littleGuy, hfsm);
    }

    // Same considerations as the method "CreateBot_HFSM"
    private void CreateBot_BT(Frame f, AssetRef<BTRoot> bt)
    {
      var littleGuy = CreateLittleGuy(f);

      // As this entity will be controlled by a bot,
      // initialize the BT right away and don't touch the PlayerRef field so it remains with default
      // This bot is agnostic to the existence of players, so it will never
      // switch controls with any player.
      BTHelper.SetupBT(f, littleGuy, bt);
    }
    #endregion

    #region Helpers
    // Method responsible for setting up a LittleGuy
    // It is agnostic to PLAYERS and BOTS, so it just creates
    // the most basic information such as the DynamicBody
    private EntityRef CreateLittleGuy(Frame f)
    {
      // Create the entity and so all of the common setup
      var littleGuy = f.Create();

      // Set the component with data specific to the Little Guys
      var littleGuyComponent = new LittleGuyComponent();
      f.Set(littleGuy, littleGuyComponent);

      // Set the transform
      var transform = new Transform2D();
      var x = (f.RNG->Next() - FP._0_50) * 2;
      var y = (f.RNG->Next() - FP._0_50) * 2;
      transform.Position = new FPVector2(x, y);
      f.Set(littleGuy, transform);

      // Set the view
      var view = View.Create(f, "Resources/DB/1 - Little Guys Sample/Prefabs/LittleGuy|EntityView");
      f.Set(littleGuy, view);

      // Set physics info: Collider, PhysicsBody and Collision Callbacks
      var collider = PhysicsCollider2D.Create(f, Shape2D.CreateCircle(FP._0_20));
      collider.Layer = f.Layers.GetLayerIndex("LittleGuy");
      f.Set(littleGuy, collider);

      var physicsBody2D = PhysicsBody2D.CreateDynamic(1);
      f.Set(littleGuy, physicsBody2D);

      var flags = CallbackFlags.OnDynamicTrigger | CallbackFlags.OnStaticTrigger;
      f.Physics2D.SetCallbacks(littleGuy, flags);

      return littleGuy;
    }

    // Check if the entity has 
    private BotType GetBotType(Frame f, EntityRef littleGuy)
    {
      var littleGuyComponent = f.Unsafe.GetPointer<LittleGuyComponent>(littleGuy);
      if (f.Has<HFSMAgent>(littleGuy))
      {
        return BotType.HFSM;
      }

      //if (f.Has<GOAPAgent>(littleGuy))
      //{
      //  return BotType.GOAP;
      //}

      if (f.Has<BTAgent>(littleGuy))
      {
        return BotType.BT;
      }

      return BotType.None;
    }
    #endregion

    #region Update and Switching Player/Bot Controls
    public override void Update(Frame f)
    {
      CheckFillRoom(f);

      var guysIterator = f.Unsafe.GetComponentBlockIterator<LittleGuyComponent>();

      foreach (var (entity, guyComponent) in guysIterator)
      {
        // If there is a PlayerRef defined, then this entity belongs to a real player.
        // This means that this entity was not created as a bot from the very beginning of the match.
        // In this case, check if the player shall or not be controled by bot logic.
        if (guyComponent->PlayerRef != default)
        {
          UpdateIsBot(f, entity);
        }

        // If not controlled by bot, just get the player input.
        // If controlled by bot, let the HFSM or GOAP logic change the input.
        // Prepare the movement vector
        FPVector2 movementVector = FPVector2.Zero;
        if (!guyComponent->IsBot)
        {
          // If the entity is not a Bot, get the movement from the player input
          movementVector = f.GetPlayerInput(guyComponent->PlayerRef)->Movement;
        }
        else
        {
          var botType = GetBotType(f, entity);
          switch (botType)
          {
            case BotType.None:
              Log.Info("Trying to get input but the Bot seems not initialized.");
              break;
            case BotType.HFSM:
              movementVector = HFSMHelper.GetBotMovement(f, entity);
              break;
            case BotType.BT:
              movementVector = BTHelper.GetBotMovement(f, entity) / 2;
              break;
          }
        }

        var physicsBody = f.Unsafe.GetPointer<PhysicsBody2D>(entity);
        // Movement the entity based on the movement vector which came either from player or bot logic
        physicsBody->Velocity = movementVector * 5;

        // Apply rotation based on the velocity
        if (physicsBody->Velocity != FPVector2.Zero)
        {
          var transform = f.Unsafe.GetPointer<Transform2D>(entity);
          transform->Rotation = FPMath.Atan2(-physicsBody->Velocity.X, physicsBody->Velocity.Y);
        }
      }
    }

    // Updates if an entity should be controlled either by a player or a bot
    // on the next frames.
    // This method is only called for entities which had a player from the very beginning.
    // Entities that are defined as bot from the very beginning won't reach this part of code.
    private void UpdateIsBot(Frame f, EntityRef littleGuyEntity)
    {
      // Return if players shouldn't be replaced by bots
      if (!f.RuntimeConfig.ReplaceOnDisconnect)
        return;

      // Only update this information if this frame is Verified.
      // Long story short: PlayerInputFlags are not trustful during NOT verified frames.
      // So we only check it during verified frames.
      if (!f.IsVerified)
        return;

      var littleGuyComponent = f.Unsafe.GetPointer<LittleGuyComponent>(littleGuyEntity);
      // Get the input flags for that player
      var inputFlags = f.GetPlayerInputFlags(littleGuyComponent->PlayerRef);
      // Bitwise operations to see if the PlayerNotPresent flag is activated
      var playerDisconnected = (inputFlags & DeterministicInputFlags.PlayerNotPresent) == DeterministicInputFlags.PlayerNotPresent;

      // Store it in the IsBot field so this can be evaluated in other parts of code
      littleGuyComponent->IsBot = playerDisconnected;

      // Only initialize the entity as a bot if it doesn't have the HFSM Agent component yet
      if (playerDisconnected && f.TryGet<HFSMAgent>(littleGuyEntity, out var hfsmAgent) == false)
      {
        // We're replacing players only by the HFSM, but this could easily be changed to be GOAP instead
        HFSMHelper.SetupHFSM(f, littleGuyEntity, f.RuntimeConfig.ReplacementHFSM);
      }
    }

    // Wait for some seconds, defined on the RuntimeConfig, to actually
    // fill the room with bots. This is a time window in which players can
    // still connect to the game before being replaced by a Bot
    // These Bots cannot be further replaced by a player who joined the game later
    // If you need this, you just need to change these Bot's player refs to be different from default,
    // so player inputs can be applied to the bot
    private void CheckFillRoom(Frame f)
    {
      // Return if the rooms shouldn't be filled with Bots
      if (!f.RuntimeConfig.FillRoom)
        return;

      f.Global->FillRoomTimer += f.DeltaTime;

      if (!f.Global->FilledRoom && f.Global->FillRoomTimer >= f.RuntimeConfig.FillRoomCooldown)
      {
        FillRoomWithBots(f);
        f.Global->FilledRoom = true;
      }
    }
    #endregion
  }
}