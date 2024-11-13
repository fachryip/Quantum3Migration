namespace Quantum
{
  public unsafe class MainSystem : SystemSignalsOnly
  {
    // Used to disable the systems which are not related to the chosen sample

    public override void OnInit(Frame f)
    {
    }

    public static bool IsSampleLoaded(Frame f, string sceneName)
    {
      var mapAsset = f.FindAsset<Map>(f.RuntimeConfig.Map.Id);
      return mapAsset.Scene == sceneName;
    }
  }
}
