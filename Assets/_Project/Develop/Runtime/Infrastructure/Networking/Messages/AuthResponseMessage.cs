using Mirror;

namespace Infrastructure.Networking.Messages
{
    public struct AuthResponseMessage : NetworkMessage
    {
        public byte Code;
        public string Message;
    }
}