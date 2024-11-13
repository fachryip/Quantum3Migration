using System;
using System.Collections.Generic;
using Photon.Deterministic;

namespace Quantum
{
  [AssetObjectConfig(GenerateAssetCreateMenu = false)]
	public partial class HFSMRoot : AssetObject
  {
		// ========== PUBLIC MEMBERS ==================================================================================

    [BotSDKHidden]
    public string Label;

		public AssetRefHFSMState[] StatesLinks;

    [NonSerialized]
    public HFSMState[] States;

    public AssetRefHFSMState InitialState
		{
			get
			{
				if (StatesLinks != null)
				{
					return StatesLinks[0];
				}
				return default;
			}
		}

		public string[] EventsNames;

		[NonSerialized]
		public Dictionary<string, int> RegisteredEvents = new Dictionary<string, int>();

		// ========== PUBLIC METHODS ==================================================================================

		public string GetEventName(int eventID)
		{
			foreach (var kvp in RegisteredEvents)
			{
				if (kvp.Value == eventID)
					return kvp.Key;
			}
			return "";
		}

		// ========== AssetObject INTERFACE ===========================================================================

		public override void Loaded(IResourceManager resourceManager, Native.Allocator allocator)
		{
			base.Loaded(resourceManager, allocator);

      States = new HFSMState[StatesLinks == null ? 0 : StatesLinks.Length];
      if (StatesLinks != null)
      {
        for (Int32 i = 0; i < StatesLinks.Length; i++)
        {
          States[i] = (HFSMState)resourceManager.GetAsset(StatesLinks[i].Id);
        }
      }

      RegisteredEvents.Clear();
			for (int i = 0; i < EventsNames.Length; i++)
			{
				RegisteredEvents.Add(EventsNames[i], i + 1);
			}
		}
	}
}