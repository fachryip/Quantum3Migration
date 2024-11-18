using Photon.Deterministic;
using System;
using Quantum.Prototypes;
using Quantum.Collections;

namespace Quantum
{
	[Serializable]
	public struct ResponseCurvePack
	{
		public AssetRefAIFunction ResponseCurveRef;
		[NonSerialized] public ResponseCurve ResponseCurve;
	}

	// ============================================================================================================

  [AssetObjectConfig(GenerateAssetCreateMenu = false)]
	public unsafe partial class Consideration
  {
		// ========== PUBLIC MEMBERS ==================================================================================

    [BotSDKHidden]
    public string Label;

		public AssetRefAIFunction RankRef;
		public AssetRefAIFunction CommitmentRef;
		public AssetRefConsideration[] NextConsiderationsRefs;
		public AssetRefAIAction[] OnEnterActionsRefs;
		public AssetRefAIAction[] OnUpdateActionsRefs;
		public AssetRefAIAction[] OnExitActionsRefs;

		public ResponseCurvePack[] ResponseCurvePacks;

		public FP BaseScore;

		public UTMomentumData MomentumData;
    // Should this Consideration use the highest nested Momentum instead of its own Momentum when doing comparisons?
    // PS: activating this does NOT prevent the Consideration to add and tick its own Momentum
    public bool UseNestedMomentum;

		public FP Cooldown;
    public bool CooldownCancelsMomentum = true;

    public byte Depth;
	}
}
