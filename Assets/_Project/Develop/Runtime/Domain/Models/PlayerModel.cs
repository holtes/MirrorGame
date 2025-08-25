using Data.Configs;

namespace Domain.Models
{
    public class PlayerModel
    {
        public float PlayerSpeed { get; private set; }
        public float PlayerGravity { get; private set; }
        public string Nickname { get; private set; }
        public int SpawnableNetObjId { get; private set; }

        public PlayerModel(PlayerConfig playerConfig, int spawnableNetObjId)
        {
            PlayerSpeed = playerConfig.PlayerSpeed;
            PlayerGravity = playerConfig.PlayerGravity;
            SpawnableNetObjId = spawnableNetObjId;
        }

        public void SetNickname(string nickname)
        {
            Nickname = nickname;
        }
    }
}