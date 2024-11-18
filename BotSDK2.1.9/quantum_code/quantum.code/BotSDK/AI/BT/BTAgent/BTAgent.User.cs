using Photon.Deterministic;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Quantum
{
	public unsafe partial struct BTAgent : IBotSDKDebugInfoProvider
	{
		// ========== PUBLIC MEMBERS ==================================================================================

		// Used to setup info on the Unity debugger
		public string GetTreeAssetName(Frame frame) => frame.FindAsset<BTRoot>(Tree.Id).Path;
		public string GetTreeAssetName(FrameThreadSafe frame) => frame.FindAsset<BTRoot>(Tree.Id).Path;

		public bool IsAborting => AbortNodeId != 0;

		public AIConfig GetConfig(Frame frame)
		{
			return frame.FindAsset<AIConfig>(Config.Id);
		}

		public AIConfig GetConfig(FrameThreadSafe frame)
		{
			return frame.FindAsset<AIConfig>(Config.Id);
		}

		public void Init(Frame frame, EntityRef entityRef, BTAgent* agent, AssetRefBTNode tree)
		{
			if (this.Tree != default)
			{
				Free(frame);
			}

			// -- Cache the tree
			BTRoot treeAsset = frame.FindAsset<BTRoot>(tree.Id);
			this.Tree = treeAsset;

			// -- Trigger the debugging event (mostly for the Unity side)
			BotSDKEditorEvents.BT.InvokeOnSetupDebugger(entityRef, treeAsset.Path);

			// -- Allocate data
			// Success/Fail/Running
			NodesStatus = frame.AllocateList<Byte>(treeAsset.NodesAmount);

			// Node data, such as FP for timers, Integers for IDs
			BTDataValues = frame.AllocateList<BTDataValue>(4);

			// The Services contained in the current sub-tree,
			// which should be updated considering its intervals
			ActiveServices = frame.AllocateList<AssetRefBTService>(4);

			// Next tick in which each service shall be updated
			ServicesEndTimes = frame.AllocateList<FP>(4);

			// The Dynamic Composites contained in the current sub-tree,
			// which should be re-checked every tick
			DynamicComposites = frame.AllocateList<AssetRefBTComposite>(4);

			AbortNodeId = 0;

			// -- Find the blackboard to be passed (if any)
			AIBlackboardComponent* blackboard = null;
			if (frame.Has<AIBlackboardComponent>(entityRef))
			{
				blackboard = frame.Unsafe.GetPointer<AIBlackboardComponent>(entityRef);
			}

			// -- Initialize the tree
			treeAsset.InitializeTree(frame, agent, blackboard);
		}

		/// <summary>
		/// Used to free every collection contained on the BTAgent component and to reset to default all of its fields.
		/// <br/>IMPORTANT: It is crucial to Free such data before destroying the entity to avoid memory leaks.
		/// </summary>
		/// <param name="frame"></param>
		/// <param name="entity"></param>
		public void Free(Frame frame)
		{
			Tree = default;
			Current = default;

			if (NodesStatus.Ptr != Ptr.Null)
			{
				frame.FreeList<Byte>(NodesStatus);
				frame.FreeList<FP>(ServicesEndTimes);
				frame.FreeList<BTDataValue>(BTDataValues);
				frame.FreeList<AssetRefBTService>(ActiveServices);
				frame.FreeList<AssetRefBTComposite>(DynamicComposites);
			}

			AbortNodeId = 0;
		}

		// TODO: allow this to be called from outside the main BT pipeline as to avoid the need of converting from FrameThreadSafe to Frame
		private void ClearContextualData(BTParams btParams)
		{
			var frame = btParams.Frame;

			btParams.Agent->AbortNodeId = default;

			var times = frame.ResolveList<FP>(ServicesEndTimes);
			for (int i = 0; i < times.Count; i++)
			{
				times[i] = 0;
			}

			frame.ResolveList<AssetRefBTService>(ActiveServices).Clear();
			frame.FreeList<AssetRefBTService>(ActiveServices);
			ActiveServices = frame.AllocateList<AssetRefBTService>(4);

			frame.ResolveList<AssetRefBTComposite>(DynamicComposites).Clear();

			var entries = frame.ResolveDictionary(btParams.Blackboard->Entries);
			foreach (var kvp in entries)
			{
				if (kvp.Value.ReactiveDecorators.Ptr != default)
				{
					var reactiveDecorators = frame.ResolveList(kvp.Value.ReactiveDecorators);
					for (int i = reactiveDecorators.Count - 1; i >= 0; i--)
					{
						reactiveDecorators.RemoveAt(i);
					}
				}
			}
		}

		public void Update(ref BTParams btParams, ref AIContext aiContext)
		{
			AssetRefBTRoot tree = btParams.Agent->Tree;
			if (tree != default)
			{
				// We always load the root asset to force it's Loaded callback to be called, if it was not yet
				// The root then also the entire tree, forcing the Loaded calls
				// This is useful for late joiners who did not have the tree loaded yet, thus potentially having non-cached data
				btParams.FrameThreadSafe.FindAsset<BTRoot>(tree.Id);
			}

			if (btParams.Agent->Current == null)
			{
				// Caching based on the ID as the asset refs won't cast
				btParams.Agent->Current.Id = btParams.Agent->Tree.Id;
			}

			RunDynamicComposites(btParams, ref aiContext);

			BTNode node = btParams.FrameThreadSafe.FindAsset<BTNode>(btParams.Agent->Current.Id);
			UpdateSubtree(btParams, node, ref aiContext);

			BTManager.ClearBTParams(btParams);
		}

		public unsafe void AbortLowerPriority(BTParams btParams, BTNode node, ref AIContext aiContext, BTAbort abortType)
		{
			// Go up and find the next interesting node (composite or root)
			var topNode = node;
			while (
				topNode.NodeType != BTNodeType.Composite &&
				topNode.NodeType != BTNodeType.Root)
			{
				topNode = topNode.Parent;
			}

			if (topNode.NodeType == BTNodeType.Root)
			{
				return;
			}

			var nodeAsComposite = (topNode as BTComposite);
			nodeAsComposite.AbortNodes(btParams,
				ref aiContext,
				abortType,
				nodeAsComposite.GetCurrentChildId(btParams.FrameThreadSafe, btParams.Agent) + 1);
		}

		// Used to react to blackboard changes which are observed by Decorators
		// This is triggered by the Blackboard Entry itself, which has a list of Decorators that observes it
		public unsafe void OnDecoratorReaction(BTParams btParams, BTNode node, BTAbort abort,
			out bool abortSelf, ref AIContext aiContext)
		{
			abortSelf = false;

			var status = node.GetStatus(btParams.FrameThreadSafe, btParams.Agent);

			// If re-running didn't result in False, it means no Abort happend. Just return
			var dryRunResult = node.DryRun(btParams, ref aiContext);
			if (dryRunResult == true)
			{
				return;
			}

			if (abort.IsSelf() && (status == BTStatus.Running || status == BTStatus.Inactive))
			{
				abortSelf = true;
				node.OnAbort(btParams, ref aiContext, abort);
			}

			if (abort.IsLowerPriority())
			{
				AbortLowerPriority(btParams, node, ref aiContext, abort);
			}
		}

		// ========== PRIVATE METHODS =================================================================================

		// We run the dynamic composites contained on the current sub-tree (if any)
		// If any of them result in "False", we abort the current sub-tree
		// and take the execution back to the topmost decorator so the agent can choose another path
		private void RunDynamicComposites(BTParams btParams, ref AIContext aiContext)
		{
			var frame = btParams.FrameThreadSafe;
			var dynamicComposites = frame.ResolveList<AssetRefBTComposite>(DynamicComposites);

			for (int i = 0; i < dynamicComposites.Count; i++)
			{
				var compositeRef = dynamicComposites.GetPointer(i);
				var composite = frame.FindAsset<BTComposite>(compositeRef->Id);
				var dynamicResult = composite.OnDynamicRun(btParams, ref aiContext);

				if (dynamicResult == false)
				{
					// We'll resume the execution from the topmost decorator
					btParams.Agent->Current = composite.TopmostDecorator;

					// We find the decorator instance
					var topmostDecorator = frame.FindAsset<BTNode>(composite.TopmostDecorator.Id);

					// Trigger the OnExit callback on the entire chain of decorators/composites/leafs
					// so they can dispose data as we're leaving the subtree
					OnDynamicFailureExit(btParams, ref aiContext, topmostDecorator);

					// As this will be treated now as the Current node, it is gonna go through the Update Subtree process
					// So this node cannot be treated as Inactive, otherwise it will re-trigger the decorators checks again
					// To prevent running it again, we set it to Failure like the entire chain of Decorators failed for this Dynamic
					// Even if only one of them faield. The topmost is treated as a failure and, upon Update, it will just go up
					// on the tree again
					topmostDecorator.SetStatus(btParams.FrameThreadSafe, BTStatus.Failure, btParams.Agent);
					BotSDKEditorEvents.BT.InvokeOnDecoratorChecked(btParams.Entity, topmostDecorator.Guid.Value, false);

					dynamicComposites.Remove(*compositeRef);
					return;
				}
			}
		}

		private void OnDynamicFailureExit(BTParams btParams, ref AIContext aiContext, BTNode node)
		{
			// Exit the node, which might trigger debugger callbacks to de-color them
			node.OnExit(btParams, ref aiContext);

			// Propagate the exiting chain
			if (node.NodeType == BTNodeType.Composite)
			{
				var composite = (BTComposite)node;
				for (int i = 0; i < composite.ChildInstances.Length; i++)
				{
					var child = composite.ChildInstances[i];
					if(child.GetStatus(btParams.FrameThreadSafe, btParams.Agent) == BTStatus.Running)
					{
						OnDynamicFailureExit(btParams, ref aiContext, child);
					}
				}
			}
			else if (node.NodeType == BTNodeType.Decorator)
			{
				var decorator = (BTDecorator)node;
				OnDynamicFailureExit(btParams, ref aiContext, decorator.ChildInstance);
			}
		}

		private void UpdateSubtree(BTParams btParams, BTNode node, ref AIContext aiContext, bool continuingAbort = false)
		{
			// Start updating the tree from the Current agent's node
			var result = node.Execute(btParams, ref aiContext, continuingAbort);

			// If the current node completes, go up in the tree until we hit a composite
			// Run that one. On success or fail continue going up.
			while (result != BTStatus.Running && node.Parent != null)
			{
				// As we're traversing the tree up (i.e exiting nodes), allow for nodes to free resources
				node.OnExit(btParams, ref aiContext);

				// Allow for some nodes to be executed again
				// This is essential for resuming nodes which stopped their execution due to Running
				// child nodes
				node = node.Parent;
				bool shouldExecuteAgain = node.ChildCompletedRunning(btParams, result);
				if (shouldExecuteAgain == true)
				{
					result = node.Execute(btParams, ref aiContext, continuingAbort);
				}

				if (node.NodeType == BTNodeType.Decorator)
				{
					((BTDecorator)node).EvaluateAbortNode(btParams);
				}
			}

			BTService.TickServices(btParams, ref aiContext);

			if (result != BTStatus.Running)
			{
				BTNode tree = btParams.FrameThreadSafe.FindAsset<BTNode>(btParams.Agent->Tree.Id);
				tree.OnExit(btParams, ref aiContext);
				tree.OnReset(btParams, ref aiContext);

				btParams.Agent->Current.Id = btParams.Agent->Tree.Id;

				ClearContextualData(btParams);
				BotSDKEditorEvents.BT.InvokeOnTreeCompleted(btParams.Entity);

				//Log.Info("Behaviour Tree completed with result '{0}'. It will re-start from '{1}'", result, btParams.Agent->Current.Id);
			}
		}

		#region DebugInfo
		public static BTAgentsIterator Iterator = new BTAgentsIterator();

		public class BTAgentsIterator : IEnumerator<IBotSDKDebugInfo>
		{
			private int _index = -1;

			private BTAgent _btAgent;
			private AIBlackboardComponent _blackboardComponent;

			public void Initialize(BTAgent btAgent, AIBlackboardComponent blackboardComponent)
			{
				Reset();
				_btAgent = btAgent;
				_blackboardComponent = blackboardComponent;
			}

			public IBotSDKDebugInfo Current
			{
				get
				{
					return new BotSDKDebugInfoBT
					{
						BTAgent = _btAgent,
						DebuggedBlackboardComponent = _blackboardComponent,
					};
				}
			}

			object IEnumerator.Current => throw new NotImplementedException();

			public void Dispose() { }

			public bool MoveNext()
			{
				_index++;
				return _index == 0 ? true : false;
			}

			public void Reset()
			{
				_index = -1;
			}
		}

		public DelegateGetDebugInfo GetDebugInfo()
		{
			return GetDebugInfoList;
		}

		public static IEnumerator<IBotSDKDebugInfo> GetDebugInfoList(Frame frame, EntityRef entity, void* ptr)
		{
			frame.TryGet<BTAgent>(entity, out var btAgent);
			frame.TryGet<AIBlackboardComponent>(entity, out var blackboardComponent);

			Iterator.Initialize(btAgent, blackboardComponent);
			return Iterator;
		}
		#endregion
	}
}