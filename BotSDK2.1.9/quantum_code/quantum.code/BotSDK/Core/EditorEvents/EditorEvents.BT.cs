using System;
using System.Runtime.CompilerServices;

namespace Quantum
{
  public static partial class BotSDKEditorEvents
  {
    public static class BT
    {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      private static void InvokeAction(Action<EntityRef, long> action, EntityRef entityRefParam, long longParam)
      {
        try
        {
          action?.Invoke(entityRefParam, longParam);
        }
        catch (Exception e)
        {
          Log.Exception(e);
        }
      }

      // --------------------
      // ON SETUP DEBUGGER
      // --------------------
      private static event Action<EntityRef, string> _onSetupDebugger;
      public static event Action<EntityRef, string> OnSetupDebugger
      {
        add => _onSetupDebugger += value;
        remove => _onSetupDebugger -= value;
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static void InvokeOnSetupDebugger(EntityRef entityRef, string treePath)
      {
        try
        {
          _onSetupDebugger?.Invoke(entityRef, treePath);
        }
        catch (Exception e)
        {
          Log.Exception(e);
        }
      }

      // --------------------
      // ON TREE COMPLETED
      // --------------------
      private static event Action<EntityRef> _onTreeCompleted;
      public static event Action<EntityRef> OnTreeCompleted
      {
        add => _onTreeCompleted += value;
        remove => _onTreeCompleted -= value;
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static void InvokeOnTreeCompleted(EntityRef entityRef)
      {
        try
        {
          _onTreeCompleted?.Invoke(entityRef);
        }
        catch (Exception e)
        {
          Log.Exception(e);
        }
      }

      // --------------------
      // ON NODE ENTER
      // --------------------
      private static event Action<EntityRef, long> _onNodeEnter;
      public static event Action<EntityRef, long> OnNodeEnter
      {
        add => _onNodeEnter += value;
        remove => _onNodeEnter -= value;
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static void InvokeOnNodeEnter(EntityRef entityRef, long nodeId)
      {
        InvokeAction(_onNodeEnter, entityRef, nodeId);
      }

      // --------------------
      // ON NODE EXIT
      // --------------------
      private static event Action<EntityRef, long> _onNodeExit;
      public static event Action<EntityRef, long> OnNodeExit
      {
        add => _onNodeExit += value;
        remove => _onNodeExit -= value;
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static void InvokeOnNodeExit(EntityRef entityRef, long nodeId)
      {
        InvokeAction(_onNodeExit, entityRef, nodeId);
      }

      // --------------------
      // ON NODE SUCCESS
      // --------------------
      private static event Action<EntityRef, long> _onNodeSuccess;
      public static event Action<EntityRef, long> OnNodeSuccess
      {
        add => _onNodeSuccess += value;
        remove => _onNodeSuccess -= value;
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static void InvokeOnNodeSuccess(EntityRef entityRef, long nodeId)
      {
        InvokeAction(_onNodeSuccess, entityRef, nodeId);
      }

      // --------------------
      // ON NODE FAILURE
      // --------------------
      private static event Action<EntityRef, long> _onNodeFailure;
      public static event Action<EntityRef, long> OnNodeFailure
      {
        add => _onNodeFailure += value;
        remove => _onNodeFailure -= value;
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static void InvokeOnNodeFailure(EntityRef entityRef, long nodeId)
      {
        InvokeAction(_onNodeFailure, entityRef, nodeId);
      }

      // --------------------
      // ON DECORATOR CHECKED
      // --------------------
      private static event Action<EntityRef, long, bool> _onDecoratorChecked;
      public static event Action<EntityRef, long, bool> OnDecoratorChecked
      {
        add => _onDecoratorChecked += value;
        remove => _onDecoratorChecked -= value;
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static void InvokeOnDecoratorChecked(EntityRef entityRef, long nodeId, bool success)
      {
        try
        {
          _onDecoratorChecked?.Invoke(entityRef, nodeId, success);
        }
        catch (Exception e)
        {
          Log.Exception(e);
        }
      }

      // --------------------
      // ON DECORATOR RESET
      // --------------------
      private static event Action<EntityRef, long> _onDecoratorReset;
      public static event Action<EntityRef, long> OnDecoratorReset
      {
        add => _onDecoratorReset += value;
        remove => _onDecoratorReset -= value;
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static void InvokeOnDecoratorReset(EntityRef entityRef, long nodeId)
      {
        InvokeAction(_onDecoratorReset, entityRef, nodeId);
      }
    }
  }
}
