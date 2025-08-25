using Core.Interfaces;
using Core.Signals;
using Data.Configs;
using Domain.Input;
using Infrastructure.Networking.Managers;
using Infrastructure.Networking.Services;
using UnityEngine;
using Mirror;
using Zenject;

namespace Bootstrap.Installers
{
    public class GameInstaller : MonoInstaller
    {
        [Header("Configs")]
        [SerializeField] private PlayerConfig _playerConfig;

        [Header("Prefabs")]
        [SerializeField] private NetworkIdentity _netCubePrefab;

        [Header("Scene")]
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private PlayerInputHandler _inputHandler;

        [Header("Network")]
        [SerializeField] private GameNetworkManager _networkManager;
        [SerializeField] private MirrorConnectionService _connectionService;

        public override void InstallBindings()
        {
            BindSignalBus();
            BindSignals();
            BindNetworkManager();
            BindConnectionService();
            BindPlayerConfig();
            BindSpawnPoint();
            BindPlayerInputHandler();
            BindNetCubePrefabId();
        }

        private void BindSignalBus()
        {
            SignalBusInstaller.Install(Container);
        }

        private void BindSignals()
        {
            Container.DeclareSignal<OnStartHostSignal>();
            Container.DeclareSignal<OnStartConnectSignal>();
            Container.DeclareSignal<OnPlayerConnectedSignal>();
            Container.DeclareSignal<OnPlayerDisconnectedSignal>();
        }

        private void BindNetworkManager()
        {
            Container
                .Bind<GameNetworkManager>()
                .FromInstance(_networkManager);
        }

        private void BindConnectionService()
        {
            Container
                .Bind<IConnectionService>()
                .FromInstance(_connectionService);
        }

        private void BindPlayerConfig()
        {
            Container
                .Bind<PlayerConfig>()
                .FromInstance(_playerConfig);
        }

        private void BindSpawnPoint()
        {
            Container
                .Bind<Transform>()
                .FromInstance(_spawnPoint);
        }

        private void BindPlayerInputHandler()
        {
            Container
                .Bind<PlayerInputHandler>()
                .FromInstance(_inputHandler);
        }

        private void BindNetCubePrefabId()
        {
            var spawnId = _networkManager.spawnPrefabs
                .FindIndex(p => p.GetComponent<NetworkIdentity>().assetId == _netCubePrefab.assetId);

            Container
                .Bind<int>()
                .FromInstance(spawnId);
        }
    }
}