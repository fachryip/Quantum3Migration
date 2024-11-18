using Photon.Deterministic;
using System;

namespace Quantum
{
  public static unsafe partial class BTManager
  {
    // ========== PUBLIC MEMBERS ==================================================================================

    public class ThreadSafe
    {
      /// <summary>
      /// Made for internal use only.
      /// </summary>
      public static void ClearBTParams(BTParams btParams)
      {
        btParams.Reset();
      }

      /// <summary>
      /// Call this method every frame to update your BT Agent.
      /// You can optionally pass a Blackboard Component to it, if your Agent uses it
      /// </summary>
      public static void Update(FrameThreadSafe frame, EntityRef entity, AIBlackboardComponent* blackboard = null)
      {
        AIContext aiContext = new AIContext();
        Update(frame, entity, ref aiContext, blackboard);
      }

      /// <summary>
      /// Call this method every frame to update your BT Agent.
      /// You can optionally pass a Blackboard Component to it, if your Agent uses it
      /// </summary>
      public static void Update(FrameThreadSafe frame, EntityRef entity, ref AIContext aiContext, AIBlackboardComponent* blackboard = null)
      {
        var userParams = new BTParamsUser();
        InternalUpdate(frame, entity, ref userParams, ref aiContext, blackboard);
      }

      /// <summary>
      /// CAUTION: Use this overload with care.<br/>
      /// It allows the definition of custom parameters which are passed through the entire BT pipeline, for easy access.<br/>
      /// The user parameters struct needs to be created from scratch every time BEFORE calling the BT Update method.<br/>
      /// Make sure to also implement BTParamsUser.ClearUser(frame).
      /// </summary>
      /// <param name="userParams">Used to define custom user data. It needs to be created from scratch every time before calling this method.</param>
      public static void Update(FrameThreadSafe frame, EntityRef entity, ref BTParamsUser userParams,
        AIBlackboardComponent* blackboard = null)
      {
        AIContext aiContext = new AIContext();
        Update(frame, entity, ref userParams, ref aiContext, blackboard);
      }

      /// <summary>
      /// CAUTION: Use this overload with care.<br/>
      /// It allows the definition of custom parameters which are passed through the entire BT pipeline, for easy access.<br/>
      /// The user parameters struct needs to be created from scratch every time BEFORE calling the BT Update method.<br/>
      /// Make sure to also implement BTParamsUser.ClearUser(frame).
      /// </summary>
      /// <param name="userParams">Used to define custom user data. It needs to be created from scratch every time before calling this method.</param>
      public static void Update(FrameThreadSafe frame, EntityRef entity, ref BTParamsUser userParams, ref AIContext aiContext,
        AIBlackboardComponent* blackboard = null)
      {
        InternalUpdate(frame, entity, ref userParams, ref aiContext, blackboard);
      }

      private static void InternalUpdate(FrameThreadSafe frame, EntityRef entity, ref BTParamsUser userParams, ref AIContext aiContext,
        AIBlackboardComponent* blackboard = null)
      {
        if (frame.TryGetPointer<BTAgent>(entity, out var btAgent) == true)
        {
          BTParams btParams = new BTParams();
          btParams.SetDefaultParams(frame, btAgent, entity, blackboard);
          btParams.UserParams = userParams;

          btAgent->Update(ref btParams, ref aiContext);
        }
      }
    }
  }
}
