using Core.Interfaces;
using Infrastructure.Networking.Messages;
using Infrastructure.Networking.Managers;
using UnityEngine;
using Mirror;
using Zenject;
using R3;

namespace Infrastructure.Networking.Services
{
    public class MirrorConnectionService : MonoBehaviour, IConnectionService
    {
        public Observable<Unit> OnClientConnected => _onClientConnected;
        public Observable<Unit> OnClientDisconnected => _onClientDisconnected;

        private Subject<Unit> _onClientConnected = new();
        private Subject<Unit> _onClientDisconnected = new();

        private string _pendingNickname;

        private GameNetworkManager _networkManager;
        private PlayerAuthenticator _playerAuthenticator => (PlayerAuthenticator)_networkManager.authenticator;

        [Inject]
        private void Construct(GameNetworkManager networkManager)
        {
            _networkManager = networkManager;
        }

        private void Awake()
        {
            _networkManager
                .OnClientConnected
                .Subscribe(_ => OnClientConnectedToGame())
                .AddTo(this);

            _networkManager
                .OnClientDisconnected
                .Subscribe(_ => OnClientDisconnectedFromGame())
                .AddTo(this);

            _playerAuthenticator
                .OnClientStartAuthenticate
                .Subscribe(_ => SendAuthMessage())
                .AddTo(this);
        }

        public void Host(string nickname)
        {
            Debug.Log("Starting as Host...");
            _pendingNickname = nickname;
            _networkManager.StartHost();
        }

        public void Connect(string nickname)
        {
            Debug.Log("Connecting as Client...");
            _pendingNickname = nickname;
            _networkManager.StartClient();
        }

        private void SendAuthMessage()
        {
            Debug.Log("Sending AuthMessage");
            var msg = new AuthRequestMessage { Nickname = _pendingNickname };
            NetworkClient.Send(msg);
        }

        private void OnClientConnectedToGame()
        {
            _onClientConnected.OnNext(Unit.Default);
        }

        private void OnClientDisconnectedFromGame()
        {
            _onClientDisconnected.OnNext(Unit.Default);
        }
    }
}