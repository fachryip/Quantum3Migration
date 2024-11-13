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


public partial class GOAPRootAsset : AssetBase {
  public Quantum.GOAPRoot Settings;

  public override Quantum.AssetObject AssetObject => Settings;
  public new Quantum.GOAPRoot AssetObjectT => (Quantum.GOAPRoot)AssetObject;
  
  public override void Reset() {
    if (Settings == null) {
      Settings = new Quantum.GOAPRoot();
    }
    base.Reset();
  }
}

public static partial class GOAPRootAssetExts {
  public static GOAPRootAsset GetUnityAsset(this GOAPRoot data) {
    return data == null ? null : UnityDB.FindAsset<GOAPRootAsset>(data);
  }
}
