using Photon.Deterministic;
using System;
using Quantum.Collections;

namespace Quantum
{
	public unsafe partial struct AIBlackboardComponent
	{
		#region Getters
		public QBoolean GetBoolean(Frame frame, string key)
		{
			var bbValue = GetBlackboardValue(frame, key);
			return *bbValue.BooleanValue;
		}

		public byte GetByte(Frame frame, string key)
		{
			var bbValue = GetBlackboardValue(frame, key);
			return *bbValue.ByteValue;
		}

		public Int32 GetInteger(Frame frame, string key)
		{
			var bbValue = GetBlackboardValue(frame, key);
			return *bbValue.IntegerValue;
		}

		public FP GetFP(Frame frame, string key)
		{
			var bbValue = GetBlackboardValue(frame, key);
			return *bbValue.FPValue;
		}

		public FPVector2 GetVector2(Frame frame, string key)
		{
			var bbValue = GetBlackboardValue(frame, key);
			return *bbValue.FPVector2Value;
		}

		public FPVector3 GetVector3(Frame frame, string key)
		{
			var bbValue = GetBlackboardValue(frame, key);
			return *bbValue.FPVector3Value;
		}

		public EntityRef GetEntityRef(Frame frame, string key)
		{
			var bbValue = GetBlackboardValue(frame, key);
			return *bbValue.EntityRefValue;
		}

		public AssetRef GetAssetRef(Frame frame, string key)
		{
			var bbValue = GetBlackboardValue(frame, key);
			return *bbValue.AssetRefValue;
		}

    public bool TryGetID(Frame frame, string key, out Int32 id)
    {
      var bbAsset = frame.FindAsset<AIBlackboard>(Board.Id);

      return bbAsset.TryGetEntryID(key, out id);
    }
    #endregion

    #region Setters
    public BlackboardEntry* Set(Frame frame, string key, QBoolean value)
    {
      QDictionary<int, BlackboardEntry> entries = frame.ResolveDictionary(Entries);
      if(entries.TryGetValuePointer(key.GetHashCode(), out var blackboardEntry) == true)
      {
        *blackboardEntry->Value.BooleanValue = value;
        return blackboardEntry;
      }
      else
      {
        Log.Error($"[Bot SDK] Blackboard entry with name {key} was not found");
        return null;
      }
    }

		public BlackboardEntry* Set(Frame frame, string key, byte value)
		{
      QDictionary<int, BlackboardEntry> entries = frame.ResolveDictionary(Entries);
      if (entries.TryGetValuePointer(key.GetHashCode(), out var blackboardEntry) == true)
      {
        *blackboardEntry->Value.ByteValue = value;
        return blackboardEntry;
      }
      else
      {
        Log.Error($"[Bot SDK] Blackboard entry with name {key} was not found");
        return null;
      }
    }

		public BlackboardEntry* Set(Frame frame, string key, Int32 value)
		{
      QDictionary<int, BlackboardEntry> entries = frame.ResolveDictionary(Entries);
      if (entries.TryGetValuePointer(key.GetHashCode(), out var blackboardEntry) == true)
      {
        *blackboardEntry->Value.IntegerValue = value;
        return blackboardEntry;
      }
      else
      {
        Log.Error($"[Bot SDK] Blackboard entry with name {key} was not found");
        return null;
      }
    }

		public BlackboardEntry* Set(Frame frame, string key, FP value)
		{
      QDictionary<int, BlackboardEntry> entries = frame.ResolveDictionary(Entries);
      if (entries.TryGetValuePointer(key.GetHashCode(), out var blackboardEntry) == true)
      {
        *blackboardEntry->Value.FPValue = value;
        return blackboardEntry;
      }
      else
      {
        Log.Error($"[Bot SDK] Blackboard entry with name {key} was not found");
        return null;
      }
    }

		public BlackboardEntry* Set(Frame frame, string key, FPVector2 value)
		{
      QDictionary<int, BlackboardEntry> entries = frame.ResolveDictionary(Entries);
      if (entries.TryGetValuePointer(key.GetHashCode(), out var blackboardEntry) == true)
      {
        *blackboardEntry->Value.FPVector2Value = value;
        return blackboardEntry;
      }
      else
      {
        Log.Error($"[Bot SDK] Blackboard entry with name {key} was not found");
        return null;
      }
    }

		public BlackboardEntry* Set(Frame frame, string key, FPVector3 value)
		{
      QDictionary<int, BlackboardEntry> entries = frame.ResolveDictionary(Entries);
      if (entries.TryGetValuePointer(key.GetHashCode(), out var blackboardEntry) == true)
      {
        *blackboardEntry->Value.FPVector3Value = value;
        return blackboardEntry;
      }
      else
      {
        Log.Error($"[Bot SDK] Blackboard entry with name {key} and hash {key.GetHashCode()} was not found");
        return null;
      }
    }

		public BlackboardEntry* Set(Frame frame, string key, EntityRef value)
		{
      QDictionary<int, BlackboardEntry> entries = frame.ResolveDictionary(Entries);
      if (entries.TryGetValuePointer(key.GetHashCode(), out var blackboardEntry) == true)
      {
        *blackboardEntry->Value.EntityRefValue = value;
        return blackboardEntry;
      }
      else
      {
        Log.Error($"[Bot SDK] Blackboard entry with name {key} was not found");
        return null;
      }
    }

		public BlackboardEntry* Set(Frame frame, string key, AssetRef value)
		{
      QDictionary<int, BlackboardEntry> entries = frame.ResolveDictionary(Entries);
      if (entries.TryGetValuePointer(key.GetHashCode(), out var blackboardEntry) == true)
      {
        *blackboardEntry->Value.AssetRefValue = value;
        return blackboardEntry;
      }
      else
      {
        Log.Error($"[Bot SDK] Blackboard entry with name {key} was not found");
        return null;
      }
    }
		#endregion

		#region Helpers
		public BlackboardEntry* GetBlackboardEntry(Frame frame, string key)
		{
			var entries = frame.ResolveDictionary(Entries);
      entries.TryGetValuePointer(key.GetHashCode(), out var entry);
      return entry;
		}

		public BlackboardValue GetBlackboardValue(Frame frame, string key)
		{
			//Assert.Check(string.IsNullOrEmpty(key) == false, "The Key cannot be empty or null.");

			var entries = frame.ResolveDictionary(Entries);
      entries.TryGetValuePointer(key.GetHashCode(), out var entry);
      return entry->Value;
    }

		public bool HasEntry(Frame frame, string key)
		{
			var entries = frame.ResolveDictionary(Entries);
      return entries.ContainsKey(key.GetHashCode());
		}
		#endregion

		// -- THREADSAFE

		#region Getters
		public QBoolean GetBoolean(FrameThreadSafe frame, string key)
		{
			var bbValue = GetBlackboardValue(frame, key);
			return *bbValue.BooleanValue;
		}

		public byte GetByte(FrameThreadSafe frame, string key)
		{
			var bbValue = GetBlackboardValue(frame, key);
			return *bbValue.ByteValue;
		}

		public Int32 GetInteger(FrameThreadSafe frame, string key)
		{
			var bbValue = GetBlackboardValue(frame, key);
			return *bbValue.IntegerValue;
		}

		public FP GetFP(FrameThreadSafe frame, string key)
		{
			var bbValue = GetBlackboardValue(frame, key);
			return *bbValue.FPValue;
		}

		public FPVector2 GetVector2(FrameThreadSafe frame, string key)
		{
			var bbValue = GetBlackboardValue(frame, key);
			return *bbValue.FPVector2Value;
		}

		public FPVector3 GetVector3(FrameThreadSafe frame, string key)
		{
			var bbValue = GetBlackboardValue(frame, key);
			return *bbValue.FPVector3Value;
		}

		public EntityRef GetEntityRef(FrameThreadSafe frame, string key)
		{
			var bbValue = GetBlackboardValue(frame, key);
			return *bbValue.EntityRefValue;
		}

		public AssetRef GetAssetRef(FrameThreadSafe frame, string key)
		{
			var bbValue = GetBlackboardValue(frame, key);
			return *bbValue.AssetRefValue;
		}

    public bool TryGetID(FrameThreadSafe frame, string key, out Int32 id)
    {
      var bbAsset = frame.FindAsset<AIBlackboard>(Board.Id);

      return bbAsset.TryGetEntryID(key, out id);
    }
    #endregion

    #region Setters
    public BlackboardEntry* Set(FrameThreadSafe frame, string key, QBoolean value)
    {
      QDictionary<int, BlackboardEntry> entries = frame.ResolveDictionary(Entries);
      if (entries.TryGetValuePointer(key.GetHashCode(), out var blackboardEntry) == true)
      {
        *blackboardEntry->Value.BooleanValue = value;
        return blackboardEntry;
      }
      else
      {
        Log.Error($"[Bot SDK] Blackboard entry with name {key} was not found");
        return null;
      }
    }

		public BlackboardEntry* Set(FrameThreadSafe frame, string key, byte value)
		{
      QDictionary<int, BlackboardEntry> entries = frame.ResolveDictionary(Entries);
      if (entries.TryGetValuePointer(key.GetHashCode(), out var blackboardEntry) == true)
      {
        *blackboardEntry->Value.ByteValue = value;
        return blackboardEntry;
      }
      else
      {
        Log.Error($"[Bot SDK] Blackboard entry with name {key} was not found");
        return null;
      }
    }

		public BlackboardEntry* Set(FrameThreadSafe frame, string key, Int32 value)
		{
      QDictionary<int, BlackboardEntry> entries = frame.ResolveDictionary(Entries);
      if (entries.TryGetValuePointer(key.GetHashCode(), out var blackboardEntry) == true)
      {
        *blackboardEntry->Value.IntegerValue = value;
        return blackboardEntry;
      }
      else
      {
        Log.Error($"[Bot SDK] Blackboard entry with name {key} was not found");
        return null;
      }
    }

		public BlackboardEntry* Set(FrameThreadSafe frame, string key, FP value)
		{
      QDictionary<int, BlackboardEntry> entries = frame.ResolveDictionary(Entries);
      if (entries.TryGetValuePointer(key.GetHashCode(), out var blackboardEntry) == true)
      {
        *blackboardEntry->Value.FPValue = value;
        return blackboardEntry;
      }
      else
      {
        Log.Error($"[Bot SDK] Blackboard entry with name {key} was not found");
        return null;
      }
    }

		public BlackboardEntry* Set(FrameThreadSafe frame, string key, FPVector2 value)
		{
      QDictionary<int, BlackboardEntry> entries = frame.ResolveDictionary(Entries);
      if (entries.TryGetValuePointer(key.GetHashCode(), out var blackboardEntry) == true)
      {
        *blackboardEntry->Value.FPVector2Value = value;
        return blackboardEntry;
      }
      else
      {
        Log.Error($"[Bot SDK] Blackboard entry with name {key} was not found");
        return null;
      }
    }

		public BlackboardEntry* Set(FrameThreadSafe frame, string key, FPVector3 value)
		{
      QDictionary<int, BlackboardEntry> entries = frame.ResolveDictionary(Entries);
      if (entries.TryGetValuePointer(key.GetHashCode(), out var blackboardEntry) == true)
      {
        *blackboardEntry->Value.FPVector3Value = value;
        return blackboardEntry;
      }
      else
      {
        Log.Error($"[Bot SDK] Blackboard entry with name {key} was not found");
        return null;
      }
    }

		public BlackboardEntry* Set(FrameThreadSafe frame, string key, EntityRef value)
		{
      QDictionary<int, BlackboardEntry> entries = frame.ResolveDictionary(Entries);
      if (entries.TryGetValuePointer(key.GetHashCode(), out var blackboardEntry) == true)
      {
        *blackboardEntry->Value.EntityRefValue = value;
        return blackboardEntry;
      }
      else
      {
        Log.Error($"[Bot SDK] Blackboard entry with name {key} was not found");
        return null;
      }
    }

		public BlackboardEntry* Set(FrameThreadSafe frame, string key, AssetRef value)
		{
      QDictionary<int, BlackboardEntry> entries = frame.ResolveDictionary(Entries);
      if (entries.TryGetValuePointer(key.GetHashCode(), out var blackboardEntry) == true)
      {
        *blackboardEntry->Value.AssetRefValue = value;
        return blackboardEntry;
      }
      else
      {
        Log.Error($"[Bot SDK] Blackboard entry with name {key} was not found");
        return null;
      }
    }
		#endregion

		#region Helpers
		public BlackboardEntry* GetBlackboardEntry(FrameThreadSafe frame, string key)
		{
      var entries = frame.ResolveDictionary(Entries);
      entries.TryGetValuePointer(key.GetHashCode(), out var entry);
      return entry;
    }

		public BlackboardValue GetBlackboardValue(FrameThreadSafe frame, string key)
		{
      Assert.Check(string.IsNullOrEmpty(key) == false, "The Key cannot be empty or null.");

      var entries = frame.ResolveDictionary(Entries);
      entries.TryGetValuePointer(key.GetHashCode(), out var entry);
      return entry->Value;
    }

		public bool HasEntry(FrameThreadSafe frame, string key)
		{
      var entries = frame.ResolveDictionary(Entries);
      return entries.ContainsKey(key.GetHashCode());
    }
		#endregion
	}
}
