using System;
using System.Runtime.CompilerServices;

namespace Quantum
{
  public static partial class BotSDKEditorEvents
  {
    public static class UT
    {
      // --------------------
      // SETUP DEBUGGER
      // --------------------
      private static event Action<EntityRef, string> _setupDebugger;
      public static event Action<EntityRef, string> SetupDebugger
      {
        add => _setupDebugger += value;
        remove => _setupDebugger -= value;
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static void InvokeSetupDebugger(EntityRef entityRef, string utPath)
      {
        try
        {
          _setupDebugger?.Invoke(entityRef, utPath);
        }
        catch (Exception e)
        {
          Log.Exception(e);
        }
      }

      // --------------------
      // CONSIDERATION CHOSEN
      // --------------------
      private static event Action<EntityRef, long> _considerationChosen;
      public static event Action<EntityRef, long> ConsiderationChosen
      {
        add => _considerationChosen += value;
        remove => _considerationChosen -= value;
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static void InvokeConsiderationChosen(EntityRef entityRef, long considerationGuid)
      {
        try
        {
          _considerationChosen?.Invoke(entityRef, considerationGuid);
        }
        catch (Exception e)
        {
          Log.Exception(e);
        }
      }

      // --------------------
      // ON UPDATE
      // --------------------
      private static event Action<EntityRef> _onUpdate;
      public static event Action<EntityRef> OnUpdate
      {
        add => _onUpdate += value;
        remove => _onUpdate -= value;
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static void InvokeOnUpdate(EntityRef entityRef)
      {
        try
        {
          _onUpdate?.Invoke(entityRef);
        }
        catch (Exception e)
        {
          Log.Exception(e);
        }
      }
    }
  }
}