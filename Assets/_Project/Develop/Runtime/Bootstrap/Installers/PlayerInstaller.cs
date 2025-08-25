using Core.Interfaces;
using Domain.Models;
using Infrastructure.Networking.Components;
using Infrastructure.Networking.Services;
using UnityEngine;
using Zenject;

namespace Bootstrap.Installers
{
    public class PlayerInstaller : MonoInstaller
    {
        [SerializeField] private BaseNetworkEntity _netEntity;
        [SerializeField] private MirrorPlayerService _playerNetService;

        public override void InstallBindings()
        {
            BindBaseNetworkEntity();
            BindPlayerService();
            BindPlayerModel();
        }

        private void BindBaseNetworkEntity()
        {
            Container
                .Bind<INetworkEntity>()
                .FromInstance(_netEntity);
        }

        private void BindPlayerService()
        {
            Container
                .Bind<IPlayerService>()
                .FromInstance(_playerNetService);
        }

        private void BindPlayerModel()
        {
            Container
                .Bind<PlayerModel>()
                .AsSingle();
        }
    }
}