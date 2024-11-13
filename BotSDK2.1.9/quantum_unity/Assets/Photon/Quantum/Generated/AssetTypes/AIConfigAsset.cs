// <auto-generated>
// This code was auto-generated by a tool, every time
// the tool executes this code will be reset.
//
// If you need to extend the classes generated to add
// fields or methods to them, please create partial  
// declarations in another file.
// </auto-generated>

using Quantum;
using UnityEngine;

[CreateAssetMenu(menuName = "Quantum/BotSDK/AIConfig", order = Quantum.EditorDefines.AssetMenuPriorityStart + -1)]
public partial class AIConfigAsset : AssetBase {
  public Quantum.AIConfig Settings;

  public override Quantum.AssetObject AssetObject => Settings;
  public new Quantum.AIConfig AssetObjectT => (Quantum.AIConfig)AssetObject;
  
  public override void Reset() {
    if (Settings == null) {
      Settings = new Quantum.AIConfig();
    }
    base.Reset();
  }
}

public static partial class AIConfigAssetExts {
  public static AIConfigAsset GetUnityAsset(this AIConfig data) {
    return data == null ? null : UnityDB.FindAsset<AIConfigAsset>(data);
  }
}
