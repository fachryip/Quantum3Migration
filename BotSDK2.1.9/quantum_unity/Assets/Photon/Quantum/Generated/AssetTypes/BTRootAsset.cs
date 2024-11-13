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


public partial class BTRootAsset : BTDecoratorAsset {
  public Quantum.BTRoot Settings;

  public override Quantum.AssetObject AssetObject => Settings;
  public new Quantum.BTRoot AssetObjectT => (Quantum.BTRoot)AssetObject;
  
  public override void Reset() {
    if (Settings == null) {
      Settings = new Quantum.BTRoot();
    }
    base.Reset();
  }
}

public static partial class BTRootAssetExts {
  public static BTRootAsset GetUnityAsset(this BTRoot data) {
    return data == null ? null : UnityDB.FindAsset<BTRootAsset>(data);
  }
}
