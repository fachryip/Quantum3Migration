namespace Quantum
{
	[System.Serializable]
  [AssetObjectConfig(GenerateAssetCreateMenu = false)]
	public unsafe class DebugAction : AIAction
  {
		public string Message;

		public override void Update(Frame frame, EntityRef entity, ref AIContext aiContext)
		{
			Log.Info($"[Frame {frame.Number} - IsVerified? {frame.IsVerified}] {Message}");
		}
	}
}
