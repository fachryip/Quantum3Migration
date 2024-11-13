namespace Quantum
{
	[BotSDKHidden]
	[System.Serializable]
  [AssetObjectConfig(GenerateAssetCreateMenu = false)]
	public unsafe partial class DefaultAIFunctionString : AIFunction<string>
	{
		// ========== AIFunction INTERFACE ============================================================================

		public override string Execute(Frame frame, EntityRef entity, ref AIContext aiContext)
		{
			return null;
		}
	}
}
