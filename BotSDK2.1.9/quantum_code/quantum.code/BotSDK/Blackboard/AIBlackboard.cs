using Photon.Deterministic;
using System;
using System.CodeDom;
using System.Collections.Generic;

namespace Quantum
{
	[AssetObjectConfig(CustomCreateAssetMenuName = "Quantum/BotSDK/Blackboard/AIBlackboard")]
	public unsafe partial class AIBlackboard
  {
		// ========== PUBLIC MEMBERS ==================================================================================

		public AIBlackboardEntry[] Entries;

		[NonSerialized] public Dictionary<string, Int32> Map;

    public AssetRefAIBlackboardInitializer InitializerRef;

		// ========== AssetObject INTERFACE ===========================================================================

		public override void Loaded(IResourceManager resourceManager, Native.Allocator allocator)
		{
			base.Loaded(resourceManager, allocator);

			Map = new Dictionary<string, Int32>();

			for (Int32 i = 0; i < Entries.Length; i++)
			{
				Map.Add(Entries[i].Key.Key, i);
			}
		}

		// ========== PUBLIC METHODS ==================================================================================

    public void Initialize(Frame frame, AIBlackboardComponent* blackboardComponent)
    {
      if(InitializerRef != null)
      {
        var initializer = frame.FindAsset<AIBlackboardInitializer>(InitializerRef.Id);
        AIBlackboardInitializer.InitializeBlackboard(frame, blackboardComponent, initializer);
      }
    }

		public bool TryGetEntryID(string key, out Int32 id)
		{
			return Map.TryGetValue(key, out id);
		}

		public string GetEntryName(Int32 id)
		{
			return Entries[id].Key.Key;
		}

		public bool HasEntry(string key)
		{
			for (int i = 0; i < Entries.Length; i++)
			{
				if (Entries[i].Key.Key == key)
				{
					return true;
				}
			}

			return false;
		}

		public AIBlackboardEntry GetEntry(string key)
		{
			for (int i = 0; i < Entries.Length; i++)
			{
				if (Entries[i].Key.Key == key)
				{
					return Entries[i];
				}
			}

			return default;
		}
	}
}
