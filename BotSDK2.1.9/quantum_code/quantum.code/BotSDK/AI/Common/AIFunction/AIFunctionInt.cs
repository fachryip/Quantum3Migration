namespace Quantum
{
	[BotSDKHidden]
	[System.Serializable]
  [AssetObjectConfig(GenerateAssetCreateMenu = false)]
	public unsafe partial class DefaultAIFunctionInt : AIFunction<int>
	{
		// ========== AIFunction INTERFACE ============================================================================

		public override int Execute(Frame frame, EntityRef entity, ref AIContext aiContext)
		{
			return 0;
		}
	}
}
