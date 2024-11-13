﻿using Photon.Deterministic;
using Quantum.BotSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quantum {
  public static class SystemSetup {
    public static SystemBase[] CreateSystems(RuntimeConfig gameConfig, SimulationConfig simulationConfig) {
      return new SystemBase[] {
        // pre-defined core systems
        new Core.CullingSystem2D(), 
        new Core.CullingSystem3D(),
        
        new Core.PhysicsSystem2D(),
        new Core.PhysicsSystem3D(),

        Core.DebugCommand.CreateSystem(),

        new Core.NavigationSystem(),
        new Core.EntityPrototypeSystem(),
        new Core.PlayerConnectedSystem(),

        // user systems go here 
        // Bot SDK
        new BotSDKDebuggerSystem(),
        new BotSDKTimerSystem(),

        new MainSystem(),

        // Little Guys Sample
        new LittleGuysSampleSystems("Little Guys Sample Systems",
          new LittleGuysSystem(),
          new CollectiblesSystem()
        ),
      };
    }
  }
}
