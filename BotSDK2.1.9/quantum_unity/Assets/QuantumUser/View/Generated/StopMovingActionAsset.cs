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


public partial class StopMovingActionAsset : AIActionAsset {
  public Quantum.StopMovingAction Settings;

  public override Quantum.AssetObject AssetObject => Settings;
  public new Quantum.StopMovingAction AssetObjectT => (Quantum.StopMovingAction)AssetObject;
  
}

public static partial class StopMovingActionAssetExts {
  public static StopMovingActionAsset GetUnityAsset(this StopMovingAction data) {
    return data == null ? null : UnityDB.FindAsset<StopMovingActionAsset>(data);
  }
}
