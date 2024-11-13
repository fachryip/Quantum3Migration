using Photon.Deterministic;
using System;

namespace Quantum
{
    partial class RuntimeConfig
    {
        // Which HFSMs will be created. Player agnostic
        public AssetRefHFSMRoot[] HFSMNpcs;
        // Which GOAPs will be created. Player agnostic
        public AssetRefGOAPRoot[] GOAPNpcs;
        // Which BTs will be created. Player agnostic
        public AssetRefBTRoot[] BTNpcs;

        // Which HFSMs should take control when replacing disconnected players
        public AssetRefHFSMRoot ReplacementHFSM;

        // The Blackboard used by the HFSM agents
        public AssetRefAIBlackboardInitializer HFSMBlackboardInitializer;
        // The Blackboard used by the BT agents
        public AssetRefAIBlackboardInitializer BTBlackboardInitializer;
        // The Blackboard used by the UT agents
        public AssetRefAIBlackboardInitializer UTBlackboardInitializer;

        // The AIConfig used by the HFSMs
        public AssetRefAIConfig AIConfig;

        // Should players be replaced if they disconnect?
        public bool ReplaceOnDisconnect;

        // Should the room be filled if not enough players connect to it?
        public bool FillRoom;

        // How many time should be waited before filling the room with bots?
        public FP FillRoomCooldown;

        partial void SerializeUserData(BitStream stream)
        {
            stream.SerializeArrayLength(ref HFSMNpcs);
            for (int i = 0; i < HFSMNpcs.Length; i++)
            {
                stream.Serialize(ref HFSMNpcs[i].Id.Value);
            }

            stream.SerializeArrayLength(ref GOAPNpcs);
            for (int i = 0; i < GOAPNpcs.Length; i++)
            {
                stream.Serialize(ref GOAPNpcs[i].Id.Value);
            }

            stream.SerializeArrayLength(ref BTNpcs);
            for (int i = 0; i < BTNpcs.Length; i++)
            {
                stream.Serialize(ref BTNpcs[i].Id.Value);
            }

            stream.Serialize(ref ReplacementHFSM.Id.Value);

            stream.Serialize(ref HFSMBlackboardInitializer.Id.Value);
            stream.Serialize(ref BTBlackboardInitializer.Id.Value);
            stream.Serialize(ref UTBlackboardInitializer.Id.Value);

            stream.Serialize(ref AIConfig.Id.Value);

            stream.Serialize(ref ReplaceOnDisconnect);
            stream.Serialize(ref FillRoom);
            stream.Serialize(ref FillRoomCooldown);
        }
    }
}