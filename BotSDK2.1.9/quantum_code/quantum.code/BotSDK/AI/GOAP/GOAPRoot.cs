using System;
using Photon.Deterministic;

namespace Quantum
{
  [AssetObjectConfig(GenerateAssetCreateMenu = false)]
	public partial class GOAPRoot
  {
		public AssetRefGOAPGoal[]   GoalRefs;
		public AssetRefGOAPAction[] ActionRefs;
	}
}