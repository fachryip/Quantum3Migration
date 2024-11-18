using System;
using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CollectiblesSystem : SystemMainThread, ISignalOnTrigger2D
    {
        // Create the collectibles to be delivered by Players and Bots
        public override void OnInit(Frame f)
        {
            Log.Debug("On Init CollectiblesSystem");
            if (MainSystem.IsSampleLoaded(f, "Little Guys Sample") == false) return;
            Log.Debug("CollectiblesSystem IsSampleLoaded true");

            for (Int32 i = 0; i < 16; i++)
            {
                var collectible = f.Create();

                var collectibleComponent = new CollectibleComponent();
                f.Set(collectible, collectibleComponent);

                var transform2D = new Transform2D();
                var x = (f.RNG->Next() - FP._0_50) * 10;
                var y = (f.RNG->Next() - FP._0_50) * 10;
                transform2D.Position = new FPVector2(x, y);
                f.Set(collectible, transform2D);

                var collider = PhysicsCollider2D.Create(f, Shape2D.CreateCircle(FP._0_33));
                collider.IsTrigger = true;
                f.Set(collectible, collider);

                // Set the view
                var view = View.Create(f, "Resources/DB/1 - Little Guys Sample/Prefabs/Jewel|EntityView");
                f.Set(collectible, view);
            }
        }


        public override void Update(Frame f)
        {
        }


        public void OnTrigger2D(Frame frame, TriggerInfo2D info)
        {
            if (info.IsStatic == false)
            {
                // If collision isn't static...
                // Destroy the collectible and tell that the littleGuy entity is loaded,
                // which means that it can only pick new collectibles after delivering the current one.
                // PS: this CAN be done using Blackboard variables instead of fixed data on the entity.
                // Is is currently done this way just to show that the Blackboard is not mandatory
                var guyComponent = frame.Unsafe.GetPointer<LittleGuyComponent>(info.Entity);

                if (guyComponent->BotData.Loaded)
                    return;

                // Destroy the collectible
                frame.Destroy(info.Other);

                guyComponent->BotData.TargetCollectible = default;
                guyComponent->BotData.Loaded = true;
            }
            else
            {
                var guyComponent = frame.Unsafe.GetPointer<LittleGuyComponent>(info.Entity);

                // Delivery performed
                if (guyComponent->BotData.Loaded)
                {
                    guyComponent->BotData.Loaded = false;
                }
            }
        }
    }
}