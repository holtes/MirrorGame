using Mirror;

namespace Infrastructure.Networking.Messages
{
    public struct PlayerChatMessage : NetworkMessage
    {
        public string Text;
    }
}