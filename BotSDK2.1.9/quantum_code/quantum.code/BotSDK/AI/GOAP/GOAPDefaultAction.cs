using Photon.Deterministic;
using System.Collections.Generic;
using System;

namespace Quantum
{
	[Serializable]
  [AssetObjectConfig(GenerateAssetCreateMenu = false)]
	public unsafe partial class GOAPDefaultAction : GOAPAction
  {
		public AssetRefAIAction[] OnActivateLinks;
		public AssetRefAIAction[] OnUpdateLinks;
		public AssetRefAIAction[] OnDeactivateLinks;
	}
}