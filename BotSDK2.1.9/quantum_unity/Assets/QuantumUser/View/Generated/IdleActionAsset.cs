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


public partial class IdleActionAsset : AIActionAsset {
  public Quantum.IdleAction Settings;

  public override Quantum.AssetObject AssetObject => Settings;
  public new Quantum.IdleAction AssetObjectT => (Quantum.IdleAction)AssetObject;
  
}

public static partial class IdleActionAssetExts {
  public static IdleActionAsset GetUnityAsset(this IdleAction data) {
    return data == null ? null : UnityDB.FindAsset<IdleActionAsset>(data);
  }
}
