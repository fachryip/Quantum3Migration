using System;

namespace Quantum
{

	[Serializable]
	public unsafe partial class BTSelectorRandom : BTComposite
	{
		// ========== PROTECTED METHODS ===============================================================================

		public override void OnEnter(BTParams btParams, ref AIContext aiContext)
		{
			base.OnEnter(btParams, ref aiContext);

			var max = ChildInstances.Length;
			var randomChildId = btParams.Frame.RNG->Next(0, max);
			SetCurrentChildId((FrameThreadSafe)btParams.Frame, randomChildId, btParams.Agent);
		}

		protected override BTStatus OnUpdate(BTParams btParams, ref AIContext aiContext)
		{
			var chosenChildId = GetCurrentChildId(btParams.FrameThreadSafe, btParams.Agent);
			var childInstance = _childInstances[chosenChildId];
			var status = childInstance.Execute(btParams, ref aiContext);

			return status;
		}

		// ========== INTERNAL METHODS ================================================================================

		internal override bool ChildCompletedRunning(BTParams btParams, BTStatus childResult)
		{
			if (childResult == BTStatus.Abort && btParams.Agent->IsAborting == true)
			{
				return true;
			}

			if (childResult == BTStatus.Failure || childResult == BTStatus.Abort)
			{
				SetStatus(btParams.FrameThreadSafe, BTStatus.Failure, btParams.Agent);
        BotSDKEditorEvents.BT.InvokeOnNodeFailure(btParams.Entity, Guid.Value);
			}
			else
			{
				SetStatus(btParams.FrameThreadSafe, BTStatus.Success, btParams.Agent);
				BotSDKEditorEvents.BT.InvokeOnNodeSuccess(btParams.Entity, Guid.Value);
			}

			BotSDKEditorEvents.BT.InvokeOnNodeExit(btParams.Entity, Guid.Value);

			return true;
		}
	}
}