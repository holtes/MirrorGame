using Core.Interfaces;
using Infrastructure.Networking.Messages;
using Infrastructure.Networking.Managers;
using UnityEngine;
using Mirror;
using Zenject;
using R3;

namespace Infrastructure.Networking.Services
{
    public class MirrorPlayerService : NetworkBehaviour, IPlayerService
    {
        [SyncVar(hook = nameof(OnNicknameSync))]
        private string _nickname;

        public Observable<string> OnNicknameChanged => _onNicknameChanged;

        private Subject<string> _onNicknameChanged = new();

        private GameNetworkManager _networkManager;

        [Inject]
        private void Construct(GameNetworkManager gameNetworkManager)
        {
            _networkManager = gameNetworkManager;
        }

        #region Server
        public override void OnStartServer()
        {
            base.OnStartServer();
            _networkManager
                .OnPlayerChatMessageRecieved
                .Where(data => data.conn == connectionToClient)
                .Subscribe(data => RpcReceiveMessage(data.msg.Text))
                .AddTo(this);
        }

        [Server]
        public void SetNickname(string nickname)
        {
            _nickname = nickname;
        }
        #endregion

        #region Client
        public void SendNickMessage()
        {
            var message = $"Привет от {_nickname}";
            NetworkClient.Send(new PlayerChatMessage { Text = message });
        }

        [ClientRpc]
        public void RpcReceiveMessage(string message)
        {
            Debug.Log(message);
        }

        private void OnNicknameSync(string oldVal, string newVal)
        {
            _onNicknameChanged.OnNext(newVal);
        }

        public void SpawnNetObj(int objId, Vector3 position)
        {
            var spawnRequestMessage = new SpawnRequestMessage { RequestedObjId = objId, SpawnPosition = position };
            NetworkClient.Send(spawnRequestMessage);
        }
        #endregion
    }
}