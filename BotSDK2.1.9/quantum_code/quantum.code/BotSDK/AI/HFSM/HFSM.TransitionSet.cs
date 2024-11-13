using Photon.Deterministic;
using System;

namespace Quantum
{
  [AssetObjectConfig(GenerateAssetCreateMenu = false)]
	public partial class HFSMTransitionSet
  {
		// ========== PUBLIC MEMBERS ==================================================================================

    [BotSDKHidden]
    public string Label;
		public AssetRefHFSMDecision PrerequisiteLink;

		public HFSMTransition[] Transitions;

		[NonSerialized]
		public HFSMDecision Prerequisite;

		// ========== AssetObject INTERFACE ===========================================================================

		public override void Loaded(IResourceManager resourceManager, Native.Allocator allocator)
		{
			base.Loaded(resourceManager, allocator);

			Prerequisite = (HFSMDecision)resourceManager.GetAsset(PrerequisiteLink.Id);

			if (Transitions != null)
			{
				for (int i = 0; i < Transitions.Length; i++)
				{
					Transitions[i].Setup(resourceManager);
				}
			}
		}
	}
}

