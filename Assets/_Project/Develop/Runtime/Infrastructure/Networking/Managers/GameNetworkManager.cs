using Infrastructure.Networking.Messages;
using Infrastructure.Networking.Services;
using UnityEngine;
using Zenject;
using Mirror;
using R3;

namespace Infrastructure.Networking.Managers
{
    public class GameNetworkManager : NetworkManager
    {
        public Observable<Unit> OnClientConnected => _onClientConnected;
        public Observable<Unit> OnClientDisconnected => _onClientDisconnected;
        public Observable<(NetworkConnectionToClient conn, PlayerChatMessage msg)> OnPlayerChatMessageRecieved => _onPlayerChatMessageRecieved;

        private Subject<Unit> _onClientConnected = new();
        private Subject<Unit> _onClientDisconnected = new();
        private Subject<(NetworkConnectionToClient conn, PlayerChatMessage msg)> _onPlayerChatMessageRecieved = new();

        private DiContainer _playerContainer;
        private Transform _spawnPoint;

        [Inject]
        private void Construct(DiContainer playerContainer, Transform spawnPoint)
        {
            _playerContainer = playerContainer;
            _spawnPoint = spawnPoint;
        }

        #region Server
        public override void OnStartServer()
        {
            base.OnStartServer();
            NetworkServer.RegisterHandler<PlayerChatMessage>(OnPlayerChatMessage, false);
            NetworkServer.RegisterHandler<SpawnRequestMessage>(OnSpawnRequestMessage, false);
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            NetworkServer.UnregisterHandler<PlayerChatMessage>();
            NetworkServer.UnregisterHandler<SpawnRequestMessage>();
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            Debug.Log("[GameNetworkManager] OnServerAddPlayer for conn: " + conn.connectionId);

            var playerObj = _playerContainer.InstantiatePrefab(playerPrefab,
                _spawnPoint.position, Quaternion.identity, _spawnPoint.parent);

            playerObj.name = $"{playerPrefab.name} [connId={conn.connectionId}]";

            if (!playerObj.TryGetComponent<MirrorPlayerService>(out var playerService))
            {
                Debug.LogError("Error: Failed to init playerObj");
                NetworkServer.AddPlayerForConnection(conn, playerObj);
                return;
            }

            NetworkServer.AddPlayerForConnection(conn, playerObj);

            playerService.SetNickname((string)conn.authenticationData);
        }

        private void OnPlayerChatMessage(NetworkConnectionToClient conn, PlayerChatMessage msg)
        {
            Debug.Log($"Got chat from conn {conn.connectionId}: {msg.Text}");

            _onPlayerChatMessageRecieved.OnNext((conn, msg));
        }

        private void OnSpawnRequestMessage(NetworkConnectionToClient conn, SpawnRequestMessage msg)
        {
            var go = Instantiate(spawnPrefabs[msg.RequestedObjId], msg.SpawnPosition,
                Quaternion.identity, _spawnPoint.parent);
            NetworkServer.Spawn(go);
        }
        #endregion

        #region Client
        public override void OnStartClient()
        {
            base.OnStartClient();

            NetworkClient.UnregisterPrefab(playerPrefab);

            NetworkClient.RegisterPrefab(
                playerPrefab,
                spawnHandler: (msg) =>
                {
                    return _playerContainer.InstantiatePrefab(
                            playerPrefab,
                            msg.position,
                            msg.rotation,
                            _spawnPoint.parent
                        );
                },
                unspawnHandler: go => Destroy(go)
            );
        }

        public override void OnClientConnect()
        {
            base.OnClientConnect();
            _onClientConnected.OnNext(Unit.Default);
            Debug.Log("ClientConnected");
        }

        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
            _onClientDisconnected.OnNext(Unit.Default);
        }
        #endregion
    }
}