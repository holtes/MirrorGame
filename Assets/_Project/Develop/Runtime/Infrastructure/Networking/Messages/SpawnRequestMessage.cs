using UnityEngine;
using Mirror;

namespace Infrastructure.Networking.Messages
{
    public struct SpawnRequestMessage : NetworkMessage
    {
        public int RequestedObjId;
        public Vector3 SpawnPosition;
    }
}