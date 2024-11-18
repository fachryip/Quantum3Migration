namespace Quantum
{
	[BotSDKHidden]
	[System.Serializable]
  [AssetObjectConfig(GenerateAssetCreateMenu = false)]
	public unsafe partial class DefaultAIFunctionByte : AIFunction<byte>
	{
		// ========== AIFunction INTERFACE ============================================================================

		public override byte Execute(Frame frame, EntityRef entity, ref AIContext aiContext)
		{
			return 0;
		}
	}
}
