using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Quantum;
using Quantum.Editor;
using UnityEditor;
using UnityEngine;

static class QuantumMigrationPreparation {
  public const string AssetExportRootOption = "-quantumAssetExportPath";
  public const string NewQuantumCodeGenPath = "Assets/QuantumUser/View/Generated";
  public const string PhotonRoot            = "Assets/Photon";
  public const string OldQuantumCodeGenPath = PhotonRoot + "/Quantum/Generated/AssetTypes";

  [MenuItem("Quantum/Migration Preparation/Add Migration Defines", priority = 5000)]
  public static void AddMigrationDefines() {
    foreach (var group in ValidBuildTargetGroups) {
      Log($"Checking defines for {group}");

      var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';');
      bool dirty = false;

      foreach (var define in new[] { "QUANTUM_ENABLE_MIGRATION", "QUANTUM_DISABLE_ASSET_OBJECT_POSTPROCESSOR", "QUANTUM_DISABLE_AUTO_CODEGEN" }) {
        if (Array.IndexOf(defines, define) < 0) {
          Array.Resize(ref defines, defines.Length + 1);
          defines[defines.Length - 1] = define;
          dirty = true;
          Log("Added define: " + define);
        } else {
          Log("Define already present: " + define);
        }
      }

      if (dirty) {
        PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(";", defines));
        Log($"Defines altered, saved");
      } else {
        Log($"Defines not changed");
      }
    }

    Log($"Finished adding defines");
  }
  
  [MenuItem("Quantum/Migration Preparation/Delete Prefab Standalone Assets", priority = 5001)]
  public static void DeletePrefabStandaloneAssets() {
    Log($"Deleting .{QuantumPrefabAssetImporter.Extension} assets");
    
    var assets = AssetDatabase.FindAssets($"glob:\"*.{QuantumPrefabAssetImporter.Extension}\"")
      .Distinct()
      .Select(AssetDatabase.GUIDToAssetPath)
      .ToArray();
    
    AssetDatabase.StartAssetEditing();
    try {
      foreach (var asset in assets) {
        Log($"Deleting {asset}");
        AssetDatabase.DeleteAsset(asset);
      }
    } finally {
      AssetDatabase.StopAssetEditing();
    }

    Log($"Deleted {assets.Length} .{QuantumPrefabAssetImporter.Extension} assets");
  }

  [MenuItem("Quantum/Migration Preparation/Export Assets", priority = 5002)]
  public static void ExportAssets() {
    string folderPath;
    if (Application.isBatchMode) {
      folderPath = GetCommandLineOption(AssetExportRootOption);
    } else {
      folderPath = EditorUtility.SaveFolderPanel("Export Quantum Assets to..", "", "QuantumExportedAssets");
      if (string.IsNullOrEmpty(folderPath)) {
        return;
      }
    }

    Log($"Exporting to {folderPath}");
    var count = ExportAssets(folderPath);
    Log($"Exported {count} assets");
  }

  [MenuItem("Quantum/Migration Preparation/Delete Photon", priority = 5003)]
  public static void DeletePhoton() {
    if (!Application.isBatchMode) {
      if (!EditorUtility.DisplayDialog("Warning",
            $"This operation will all contents of {PhotonRoot} " +
            "Please make sure you have run previous migration steps and have a backup of your project prior to doing this. " +
            "Are you sure you want to continue?",
            "OK", "Cancel")) {
        return;
      }
    }

    if (!Directory.Exists(OldQuantumCodeGenPath)) {
      throw new InvalidOperationException($"Quantum code gen path not found: {OldQuantumCodeGenPath}");
    }

    if (Directory.Exists(NewQuantumCodeGenPath)) {
      throw new InvalidOperationException($"New quantum code gen path already exists: {NewQuantumCodeGenPath}");
    }

    var newRoot = Path.GetDirectoryName(NewQuantumCodeGenPath);
    if (!string.IsNullOrEmpty(newRoot) && !Directory.Exists(newRoot)) {
      Directory.CreateDirectory(newRoot);
    }

    // first create target folder
    Log($"Moving {OldQuantumCodeGenPath} to {NewQuantumCodeGenPath}");
    FileUtil.MoveFileOrDirectory(OldQuantumCodeGenPath, NewQuantumCodeGenPath);

    Log("Moving done");
    
    Log($"Deleting built in components");
    foreach (var componentName in new[] {
        nameof(View),
        nameof(MapEntityLink),
        nameof(NavMeshPathfinder),
        nameof(NavMeshSteeringAgent),
        nameof(NavMeshAvoidanceAgent),
        nameof(NavMeshAvoidanceObstacle),
        nameof(Transform2D),
        nameof(Transform3D),
        nameof(PhysicsBody2D),
        nameof(PhysicsCollider2D),
        nameof(PhysicsJoints2D),
        "PhysicsCallbacks2D",
        nameof(PhysicsBody3D),
        nameof(PhysicsCollider3D),
        nameof(PhysicsJoints3D),
        "PhysicsCallbacks3D",
        nameof(Transform2DVertical),
        nameof(CharacterController3D),
        nameof(CharacterController2D),
      }) {
      var path = $"{NewQuantumCodeGenPath}/EntityComponent{componentName}.cs";
      Log($"Deleting {path}");
      FileUtil.DeleteFileOrDirectory(path);
      FileUtil.DeleteFileOrDirectory(path + ".meta");
    }
    Log($"Deleting built in components done");

    Log($"Patching QuantumEditorSettings");
    ArrayUtils.Add(ref QuantumEditorSettings.InstanceFailSilently.AssetSearchPaths, "Assets/QuantumUser/Resources");
    EditorUtility.SetDirty(QuantumEditorSettings.InstanceFailSilently);

    var assetsResourcesPath = QuantumEditorSettings.InstanceFailSilently.AssetResourcesPath;

    Log($"Deleting {PhotonRoot}");
    FileUtil.DeleteFileOrDirectory(PhotonRoot);

    if (!string.IsNullOrEmpty(assetsResourcesPath)) {
      Log($"Deleting {assetsResourcesPath}");
      FileUtil.DeleteFileOrDirectory(assetsResourcesPath);
    }

    bool success = true;
    if (Directory.Exists(PhotonRoot)) {
      Error($"Directory {PhotonRoot} still exists");
      success = false;
    }

    ConsumeResult(success);
  }

  public static int ExportAssets(string folderPath) {
    if (!Directory.Exists(folderPath)) {
      Directory.CreateDirectory(folderPath);
    }

    var assets = AssetDBGeneration.GatherQuantumAssets();

    foreach (var assetObject in assets) {
      if (!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(assetObject.GetInstanceID(), out var guid, out long fileId)) {
        throw new InvalidOperationException();
      }

      var json = EditorJsonUtility.ToJson(assetObject.AssetObject, true);
      var filePath = Path.Combine(folderPath, $"{guid}_{fileId}.json");
      File.WriteAllText(filePath, json);
    }

    return assets.Count;
  }
  
  private static bool IsEnumValueObsolete<T>(string valueName) where T : System.Enum {
    var fi = typeof(T).GetField(valueName);
    var attributes = fi.GetCustomAttributes(typeof(System.ObsoleteAttribute), false);
    return attributes?.Length > 0;
  }

  private static IEnumerable<BuildTargetGroup> ValidBuildTargetGroups {
    get {
      foreach (var name in System.Enum.GetNames(typeof(BuildTargetGroup))) {
        if (IsEnumValueObsolete<BuildTargetGroup>(name))
          continue;
        var group = (BuildTargetGroup)System.Enum.Parse(typeof(BuildTargetGroup), name);
        if (group == BuildTargetGroup.Unknown)
          continue;

        yield return group;
      }
    }
  }

  private static string GetCommandLineOption(string option) {
    var cmdArgs = System.Environment.GetCommandLineArgs();
    var optionIndex = Array.IndexOf(cmdArgs, option);
    if (optionIndex < 0 && optionIndex >= cmdArgs.Length - 1) {
      throw new InvalidOperationException($"Missing {option} argument");
    }

    return cmdArgs[optionIndex + 1];
  }

  static void ConsumeResult(bool success, [CallerMemberName] string memberName = null) {
    if (success) {
      Log($"{memberName} succeeded");
    } else {
      string msg = $"{memberName} failed, check previous log messages";
      if (Application.isBatchMode) {
        Error(msg);
        EditorApplication.Exit(1);
      } else {
        throw new InvalidOperationException(msg);
      }
    }
  }

  static void Log(string message) {
    Debug.Log($"[Quantum Migration Preparation] {message}");
  }

  static void Error(string message) {
    Debug.LogError($"[Quantum Migration Preparation] {message}");
  }
}