using System;

namespace Quantum
{
  [Serializable]
  [AssetObjectConfig(GenerateAssetCreateMenu = false)]
  public unsafe partial class BTForceResult : BTDecorator
  {
    // ========== PUBLIC MEMBERS ==================================================================================

    public BTStatus Result;

    // ========== BTNode INTERFACE ================================================================================

    protected override BTStatus OnUpdate(BTParams btParams, ref AIContext aiContext)
    {
      if (_childInstance != null)
        _childInstance.Execute(btParams, ref aiContext);

      return Result;
    }

    public override Boolean DryRun(BTParams btParams, ref AIContext aiContext)
    {
      return true;
    }
  }
}
