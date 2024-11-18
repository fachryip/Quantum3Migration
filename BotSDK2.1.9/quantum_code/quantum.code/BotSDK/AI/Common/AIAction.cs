using System;
using Photon.Deterministic;

namespace Quantum
{
  public abstract unsafe partial class AIAction
	{
    // ========== PUBLIC MEMBERS ==================================================================================

    [BotSDKHidden]
		public string Label;

		// ========== AIAction INTERFACE ================================================================================

		public virtual void Update(Frame frame, EntityRef entity, ref AIContext aiContext)
		{
		}

		public virtual void Update(FrameThreadSafe frame, EntityRef entity, ref AIContext aiContext)
		{
			Update((Frame)frame, entity, ref aiContext);
		}
	}
}
