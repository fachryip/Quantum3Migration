using Photon.Deterministic;

namespace Quantum
{
	[System.Serializable]
	public unsafe class SeekIntruder : BTService
	{
		public AIBlackboardValueKey IntruderRef;

		protected override void OnUpdate(BTParams p, ref AIContext aiContext)
		{
			var f = p.Frame;
			var e = p.Entity;
			var bb = p.Blackboard;
			var t = f.Get<Transform2D>(e);

			EntityRef previousTarget = bb->GetEntityRef(f, IntruderRef.Key);

			var hits = f.Physics2D.OverlapShape(t, Shape2D.CreateCircle(2));
			EntityRef targetEntity = default;
			for (int i = 0; i < hits.Count; i++)
			{
				var hit = hits[i];

				if (hit.Entity != e && f.Has<LittleGuyComponent>(hit.Entity))
				{
					targetEntity = hit.Entity;
				}
			}

			if (targetEntity != default)
			{
				bb->Set(f, IntruderRef.Key, targetEntity)->TriggerDecorators(p);
			}
			else if (previousTarget != default)
			{
				bb->Set(f, IntruderRef.Key, default(EntityRef))->TriggerDecorators(p);
			}
		}
	}
}
