using UnityEngine;

namespace Data.Configs
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Configs/PlayerConfig")]
    public class PlayerConfig : ScriptableObject
    {
        [SerializeField] private float _playerSpeed = 5f;
        [SerializeField] private float _playerGravity = -9.81f;

        public float PlayerSpeed => _playerSpeed;
        public float PlayerGravity => _playerGravity;
    }
}