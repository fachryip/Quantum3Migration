// <auto-generated>
// This code was auto-generated by a tool, every time
// the tool executes this code will be reset.
//
// If you need to extend the classes generated to add
// fields or methods to them, please create partial
// declarations in another file.
// </auto-generated>
#pragma warning disable 0109
#pragma warning disable 1591


namespace Quantum.Prototypes.Unity {
  using Photon.Deterministic;
  using Quantum;
  using Quantum.Core;
  using Quantum.Collections;
  using Quantum.Inspector;
  using Quantum.Physics2D;
  using Quantum.Physics3D;
  using Byte = System.Byte;
  using SByte = System.SByte;
  using Int16 = System.Int16;
  using UInt16 = System.UInt16;
  using Int32 = System.Int32;
  using UInt32 = System.UInt32;
  using Int64 = System.Int64;
  using UInt64 = System.UInt64;
  using Boolean = System.Boolean;
  using String = System.String;
  using Object = System.Object;
  using FlagsAttribute = System.FlagsAttribute;
  using SerializableAttribute = System.SerializableAttribute;
  using MethodImplAttribute = System.Runtime.CompilerServices.MethodImplAttribute;
  using MethodImplOptions = System.Runtime.CompilerServices.MethodImplOptions;
  using FieldOffsetAttribute = System.Runtime.InteropServices.FieldOffsetAttribute;
  using StructLayoutAttribute = System.Runtime.InteropServices.StructLayoutAttribute;
  using LayoutKind = System.Runtime.InteropServices.LayoutKind;
  #if QUANTUM_UNITY //;
  using TooltipAttribute = UnityEngine.TooltipAttribute;
  using HeaderAttribute = UnityEngine.HeaderAttribute;
  using SpaceAttribute = UnityEngine.SpaceAttribute;
  using RangeAttribute = UnityEngine.RangeAttribute;
  using HideInInspectorAttribute = UnityEngine.HideInInspector;
  using PreserveAttribute = UnityEngine.Scripting.PreserveAttribute;
  using FormerlySerializedAsAttribute = UnityEngine.Serialization.FormerlySerializedAsAttribute;
  using MovedFromAttribute = UnityEngine.Scripting.APIUpdating.MovedFromAttribute;
  using CreateAssetMenu = UnityEngine.CreateAssetMenuAttribute;
  using RuntimeInitializeOnLoadMethodAttribute = UnityEngine.RuntimeInitializeOnLoadMethodAttribute;
  #endif //;
  
  [System.SerializableAttribute()]
  public unsafe partial class BotDataPrototype : Quantum.QuantumUnityPrototypeAdapter<Quantum.Prototypes.BotDataPrototype> {
    public Quantum.Prototypes.InputPrototype BotInput;
    public Quantum.QuantumEntityPrototype TargetCollectible;
    public AssetRef<AIConfig> Config;
    public QBoolean Loaded;
    partial void ConvertUser(Quantum.QuantumEntityPrototypeConverter converter, ref Quantum.Prototypes.BotDataPrototype prototype);
    public override Quantum.Prototypes.BotDataPrototype Convert(Quantum.QuantumEntityPrototypeConverter converter) {
      var result = new Quantum.Prototypes.BotDataPrototype();
      converter.Convert(this.BotInput, out result.BotInput);
      converter.Convert(this.TargetCollectible, out result.TargetCollectible);
      converter.Convert(this.Config, out result.Config);
      converter.Convert(this.Loaded, out result.Loaded);
      ConvertUser(converter, ref result);
      return result;
    }
  }
  [System.ObsoleteAttribute("Use BotDataPrototype instead")]
  public unsafe partial class BotData_Prototype : BotDataPrototype {
  }
  [System.SerializableAttribute()]
  public unsafe partial class LittleGuyComponentPrototype : Quantum.QuantumUnityPrototypeAdapter<Quantum.Prototypes.LittleGuyComponentPrototype> {
    public PlayerRef PlayerRef;
    public Quantum.QEnum32<BotType> BotType;
    public Quantum.Prototypes.Unity.BotDataPrototype BotData;
    public QBoolean IsBot;
    partial void ConvertUser(Quantum.QuantumEntityPrototypeConverter converter, ref Quantum.Prototypes.LittleGuyComponentPrototype prototype);
    public override Quantum.Prototypes.LittleGuyComponentPrototype Convert(Quantum.QuantumEntityPrototypeConverter converter) {
      var result = new Quantum.Prototypes.LittleGuyComponentPrototype();
      converter.Convert(this.PlayerRef, out result.PlayerRef);
      converter.Convert(this.BotType, out result.BotType);
      converter.Convert(this.BotData, out result.BotData);
      converter.Convert(this.IsBot, out result.IsBot);
      ConvertUser(converter, ref result);
      return result;
    }
  }
  [System.ObsoleteAttribute("Use LittleGuyComponentPrototype instead")]
  public unsafe partial class LittleGuyComponent_Prototype : LittleGuyComponentPrototype {
  }
}
#pragma warning restore 0109
#pragma warning restore 1591
