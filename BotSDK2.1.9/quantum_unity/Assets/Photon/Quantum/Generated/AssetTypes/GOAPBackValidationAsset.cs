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

[CreateAssetMenu(menuName = "Quantum/GOAPBackValidation", order = Quantum.EditorDefines.AssetMenuPriorityStart + 156)]
public partial class GOAPBackValidationAsset : AssetBase {
  public Quantum.GOAPBackValidation Settings;

  public override Quantum.AssetObject AssetObject => Settings;
  public new Quantum.GOAPBackValidation AssetObjectT => (Quantum.GOAPBackValidation)AssetObject;
  
  public override void Reset() {
    if (Settings == null) {
      Settings = new Quantum.GOAPBackValidation();
    }
    base.Reset();
  }
}

public static partial class GOAPBackValidationAssetExts {
  public static GOAPBackValidationAsset GetUnityAsset(this GOAPBackValidation data) {
    return data == null ? null : UnityDB.FindAsset<GOAPBackValidationAsset>(data);
  }
}
