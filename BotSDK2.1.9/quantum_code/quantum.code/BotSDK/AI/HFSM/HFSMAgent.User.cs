﻿using System.Collections;
using System;
using System.Collections.Generic;

namespace Quantum
{
  public unsafe partial struct HFSMAgent : IBotSDKDebugInfoProvider
  {
    // ========== PUBLIC MEMBERS ==================================================================================

    // Used to setup info on the Unity debugger
    public string GetRootAssetName(Frame frame) => frame.FindAsset<HFSMRoot>(Data.Root.Id).Path;
    public string GetRootAssetName(FrameThreadSafe frame) => frame.FindAsset<HFSMRoot>(Data.Root.Id).Path;

    // ========== PUBLIC METHODS ==================================================================================

    public AIConfig GetConfig(Frame frame)
    {
      return frame.FindAsset<AIConfig>(Config.Id);
    }

    public AIConfig GetConfig(FrameThreadSafe frame)
    {
      return frame.FindAsset<AIConfig>(Config.Id);
    }

    #region Debug Info
    public static HFSMAgentsIterator Iterator = new HFSMAgentsIterator();

    public class HFSMAgentsIterator : IEnumerator<IBotSDKDebugInfo>
    {
      private int _index = -1;

      private HFSMAgent _hfsmAgent;
      private AIBlackboardComponent _blackboardComponent;

      public void Initialize(HFSMAgent hfsmAgent, AIBlackboardComponent blackboardComponent)
      {
        Reset();

        _hfsmAgent = hfsmAgent;
        _blackboardComponent = blackboardComponent;
      }

      public IBotSDKDebugInfo Current
      {
        get
        {
          return new BotSDKDebugInfoHFSM
          {
            HFSMAgent = _hfsmAgent,
            DebuggedBlackboardComponent = _blackboardComponent,
          };
        }
      }

      object IEnumerator.Current => throw new NotImplementedException();

      public void Dispose() { }

      public bool MoveNext() {
        _index++;
        return _index == 0 ? true : false; 
      }

      public void Reset()
      {
        _index = -1;
      }
    }

    public DelegateGetDebugInfo GetDebugInfo()
    {
      return GetDebugInfoList;
    }

    public static IEnumerator<IBotSDKDebugInfo> GetDebugInfoList(Frame frame, EntityRef entity, void* ptr)
    {
      frame.TryGet<HFSMAgent>(entity, out var hfsmAgent);
      frame.TryGet<AIBlackboardComponent>(entity, out var blackboardComponent);

      Iterator.Initialize(hfsmAgent, blackboardComponent);
      return Iterator;
    }
    #endregion
  }
}
