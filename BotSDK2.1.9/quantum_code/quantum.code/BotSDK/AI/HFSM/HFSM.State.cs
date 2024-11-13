using Photon.Deterministic;
using System;

namespace Quantum
{
	[AssetObjectConfig(GenerateLinkingScripts = true, GenerateAssetCreateMenu = false, GenerateAssetResetMethod = false)]
	public unsafe partial class HFSMState : AssetObject
	{
		// ========== PUBLIC MEMBERS ==================================================================================

    [BotSDKHidden]
    public string Label;

    [BotSDKHidden]
    public byte Index;

    public AssetRefAIAction[] OnUpdateLinks;
		public AssetRefAIAction[] OnEnterLinks;
		public AssetRefAIAction[] OnExitLinks;
		public HFSMTransition[] Transitions;

		public AssetRefHFSMState[] ChildrenLinks;
		public AssetRefHFSMState ParentLink;
		public int Level;

		[NonSerialized]
		public AIAction[] OnUpdate;
		[NonSerialized]
		public AIAction[] OnEnter;
		[NonSerialized]
		public AIAction[] OnExit;
		[NonSerialized]
		public HFSMState[] Children;
		[NonSerialized]
		public HFSMState Parent;

		// ========== AssetObject INTERFACE ===========================================================================

		public override void Loaded(IResourceManager resourceManager, Native.Allocator allocator)
		{
			base.Loaded(resourceManager, allocator);

			OnUpdate = new AIAction[OnUpdateLinks == null ? 0 : OnUpdateLinks.Length];
			if (OnUpdateLinks != null)
			{
				for (Int32 i = 0; i < OnUpdateLinks.Length; i++)
				{
					OnUpdate[i] = (AIAction)resourceManager.GetAsset(OnUpdateLinks[i].Id);
				}
			}
			OnEnter = new AIAction[OnEnterLinks == null ? 0 : OnEnterLinks.Length];
			if (OnEnterLinks != null)
			{
				for (Int32 i = 0; i < OnEnterLinks.Length; i++)
				{
					OnEnter[i] = (AIAction)resourceManager.GetAsset(OnEnterLinks[i].Id);
				}
			}
			OnExit = new AIAction[OnExitLinks == null ? 0 : OnExitLinks.Length];
			if (OnExitLinks != null)
			{
				for (Int32 i = 0; i < OnExitLinks.Length; i++)
				{
					OnExit[i] = (AIAction)resourceManager.GetAsset(OnExitLinks[i].Id);
				}
			}

			Children = new HFSMState[ChildrenLinks == null ? 0 : ChildrenLinks.Length];
			if (ChildrenLinks != null)
			{
				for (Int32 i = 0; i < ChildrenLinks.Length; i++)
				{
					Children[i] = (HFSMState)resourceManager.GetAsset(ChildrenLinks[i].Id);
				}
			}

			Parent = (HFSMState)resourceManager.GetAsset(ParentLink.Id);
			if (Transitions != null)
			{
				for (int i = 0; i < Transitions.Length; i++)
				{
					Transitions[i].Setup(resourceManager);
				}
			}
		}

		// ========== INTERNAL METHODS ================================================================================

		internal Boolean UpdateState(FrameThreadSafe frame, FP deltaTime, HFSMData* hfsmData, EntityRef entity, ref AIContext aiContext)
		{
			HFSMState parent = Parent;
			Boolean transition = false;

			if (parent != null)
			{
				transition = parent.UpdateState(frame, deltaTime, hfsmData, entity, ref aiContext);
			}

			if (transition == true)
				return true;

			*hfsmData->Times.GetPointer(Level) += deltaTime;

			UpdateActions(frame, entity, ref aiContext, OnUpdate);
			return CheckStateTransitions(frame, hfsmData, entity, ref aiContext, 0);
		}

		internal Boolean Event(FrameThreadSafe frame, HFSMData* hfsmData, EntityRef entity, Int32 eventInt, ref AIContext aiContext)
		{
			HFSMState p = Parent;
			Boolean transition = false;
			if (p != null)
			{
				transition = p.Event(frame, hfsmData, entity, eventInt, ref aiContext);
			}

			if (transition)
			{
				return true;
			}

			return CheckStateTransitions(frame, hfsmData, entity, ref aiContext, eventInt);
		}

		internal void EnterState(FrameThreadSafe frame, HFSMData* hfsmData, EntityRef entity, ref AIContext aiContext)
		{
			*hfsmData->Times.GetPointer(Level) = FP._0;
			UpdateActions(frame, entity, ref aiContext, OnEnter);

      if (hfsmData->UsesStateHistory == true)
      {
        hfsmData->StatesHistory = (byte)(hfsmData->StatesHistory | 1 << Index);
      }

      if (Children != null && Children.Length > 0)
			{
        HFSMState child;
        if(hfsmData->UsesStateHistory == false)
        {
          child = Children[0];
        }
        else
        {
          child = FindChildInHistory(frame, hfsmData);
        }

				HFSMManager.ThreadSafe.ChangeState(child, frame, hfsmData, entity, "", ref aiContext);
			}
		}

    // Returns the last child state updated before leaving this state
    private HFSMState FindChildInHistory(FrameThreadSafe frame, HFSMData* hfsmData)
    {
      for (int i = 0; i < Children.Length; i++)
      {
        if((hfsmData->StatesHistory & 1 << Children[i].Index) != 0)
        {
          return Children[i];
        }
      }

      return Children[0];
    }

		internal void ExitState(HFSMState nextState, FrameThreadSafe frame, HFSMData* hfsmData, EntityRef entity, ref AIContext aiContext)
		{
			if (nextState != null && nextState.IsChildOf(this) == true)
				return;

      if(hfsmData->UsesStateHistory == true && IsSiblingOf(frame, hfsmData, nextState) == true)
      {
        hfsmData->StatesHistory = (byte)(hfsmData->StatesHistory ^ 1 << Index);
      }

      UpdateActions(frame, entity, ref aiContext, OnExit);
			Parent?.ExitState(nextState, frame, hfsmData, entity, ref aiContext);
		}

		internal bool IsChildOf(HFSMState state)
		{
			if (Parent == null)
				return false;

			if (Parent == state)
				return true;

			return Parent.IsChildOf(state);
		}

    internal bool IsSiblingOf(FrameThreadSafe frame, HFSMData* hfsmData, HFSMState state)
    {
      HFSMState[] siblings;

      if(Parent != null)
      {
        siblings = Parent.Children;
      }
      else
      {
        siblings = frame.FindAsset<HFSMRoot>(hfsmData->Root.Id).States;
      }

      for (int i = 0; i < siblings.Length; i++)
      {
        if (siblings[i] == state) return true;
      }

      return false;
    }

		// ========== PRIVATE METHODS =================================================================================

    private void UpdateActions(FrameThreadSafe frame, EntityRef entity,
      ref AIContext aiContext, AIAction[] actions)
    {
      for (int i = 0; i < actions.Length; i++)
      {
        actions[i].Update(frame, entity, ref aiContext);
      }
    }

		private bool CheckStateTransitions(FrameThreadSafe frame, HFSMData* hfsmData, EntityRef entity, ref AIContext aiContext, Int32 eventKey = 0)
		{
			hfsmData->Time = *hfsmData->Times.GetPointer(Level);

			return CheckTransitions(frame, Transitions, hfsmData, entity, eventKey, ref aiContext);
		}

		private static bool CheckTransitions(FrameThreadSafe frame, HFSMTransition[] transitions, HFSMData* hfsmData,
			EntityRef entity, int eventKey, ref AIContext aiContext, int depth = 0)
		{
			// Just to avoid accidental loops
			if (depth == 10)
				return false;

			if (transitions == null)
				return false;

			for (int i = 0; i < transitions.Length; i++)
			{
				var transition = transitions[i];

				if (transition.State == null && transition.TransitionSet == null)
					continue;

				// Only consider evaluating the event if this transition HAS a event as requisite (EventKey != 0)
				if (transition.EventKey != 0 && transition.EventKey != eventKey)
					continue;

				if (transition.Decision != null && transition.Decision.DecideThreadSafe(frame, entity, ref aiContext) == false)
					continue;

				if (transition.State != null)
				{
					HFSMManager.ThreadSafe.ChangeState(transition.State, frame, hfsmData, entity, transition.Id, ref aiContext);
					return true;
				}
				else if (CheckTransitions(frame, transition.TransitionSet.Transitions, hfsmData, entity, eventKey, ref aiContext, depth + 1) == true)
				{
					return true;
				}
			}

			return false;
		}
	}
}
