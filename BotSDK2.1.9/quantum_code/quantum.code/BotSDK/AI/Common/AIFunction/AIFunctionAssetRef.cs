namespace Quantum
{
	[BotSDKHidden]
	[System.Serializable]
  [AssetObjectConfig(GenerateAssetCreateMenu = false)]
	public unsafe partial class DefaultAIFunctionAssetRef : AIFunction<AssetRef>
	{
		// ========== AIFunction INTERFACE ============================================================================

		public override AssetRef Execute(Frame frame, EntityRef entity, ref AIContext aiContext)
		{
			return null;
		}
	}
}
