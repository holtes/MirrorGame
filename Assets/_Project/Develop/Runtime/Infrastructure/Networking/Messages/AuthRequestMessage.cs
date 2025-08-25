using Mirror;

namespace Infrastructure.Networking.Messages
{
    public struct AuthRequestMessage : NetworkMessage
    {
        public string Nickname;
    }
}