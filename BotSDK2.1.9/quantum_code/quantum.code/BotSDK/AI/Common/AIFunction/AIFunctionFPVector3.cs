using Photon.Deterministic;

namespace Quantum
{
	[BotSDKHidden]
	[System.Serializable]
  [AssetObjectConfig(GenerateAssetCreateMenu = false)]
	public unsafe partial class DefaultAIFunctionFPVector3 : AIFunction<FPVector3>
	{
		// ========== AIFunction INTERFACE ============================================================================

		public override FPVector3 Execute(Frame frame, EntityRef entity, ref AIContext aiContext)
		{
			return FPVector3.Zero;
		}
	}
}
