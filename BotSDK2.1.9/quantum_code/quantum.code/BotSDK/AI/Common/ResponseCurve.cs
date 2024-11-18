using Photon.Deterministic;

namespace Quantum
{
	[BotSDKHidden]
	[System.Serializable]
  [AssetObjectConfig(GenerateAssetCreateMenu = false)]
	public unsafe partial class ResponseCurve : AIFunction<FP>
	{
		// ========== PUBLIC MEMBERS ==================================================================================

		public AIParamFP Input;

		[BotSDKHidden]
		public FPAnimationCurve Curve;
		
		public bool Clamp01 = true;

		public FP MultiplyFactor = 1;

		// ========== AssetObject INTERFACE ===========================================================================

		public override void Loaded(IResourceManager resourceManager, Native.Allocator allocator)
		{
			base.Loaded(resourceManager, allocator);
		}

		// ========== AIFunctionFP INTERFACE ==========================================================================

		public override FP Execute(Frame frame, EntityRef entity, ref AIContext aiContext)
		{
			return Execute((FrameThreadSafe)frame, entity, ref aiContext);
		}

		public override FP Execute(FrameThreadSafe frame, EntityRef entity, ref AIContext aiContext)
		{
      var blackboard = BotSDKSystem.GetBlackboardComponent(frame, entity, ref aiContext);
      var aiConfig = BotSDKSystem.GetAIConfig(frame, entity, ref aiContext);

      FP input = Input.Resolve(frame, entity, blackboard, aiConfig,  ref aiContext);
			FP result = Curve.Evaluate(input);

      if (Clamp01 == true)
			{
				if (result > 1) result = 1;
				else if (result < 0) result = 0;
			}
      
			return result * MultiplyFactor;
		}
	}
}
