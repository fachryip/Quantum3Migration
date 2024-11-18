using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum
{
  [System.Serializable]
  public unsafe class NoIntruderNearby : BTDecorator
  {
    public AIBlackboardValueKey IntruderRef;

    public override void OnStartedRunning(BTParams p, ref AIContext aiContext)
    {
      base.OnStartedRunning(p, ref aiContext);
      p.Blackboard->RegisterReactiveDecorator(p.Frame, IntruderRef.Key, this);
    }

    public override void OnExit(BTParams p, ref AIContext aiContext)
    {
      base.OnExit(p, ref aiContext);
      p.Blackboard->UnregisterReactiveDecorator(p.Frame, IntruderRef.Key, this);
    }

    public override bool CheckConditions(BTParams p, ref AIContext aiContext)
    {
      var target = p.Blackboard->GetEntityRef(p.Frame, IntruderRef.Key);
      return target == default;
    }
  }
}
