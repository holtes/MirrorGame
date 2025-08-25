using Core.Interfaces;
using Core.Signals;
using UnityEngine;
using Zenject;
using R3;

namespace Domain.Controllers
{
    public class GameController : MonoBehaviour
    {
        private IConnectionService _connectionService;
        private SignalBus _signalBus;

        [Inject]
        private void Construct(IConnectionService connectionService, SignalBus signalBus)
        {
            _connectionService = connectionService;
            _signalBus = signalBus;
        }

        private void Awake()
        {
            _signalBus.Subscribe<OnStartHostSignal>(StartHost);
            _signalBus.Subscribe<OnStartConnectSignal>(StartConnection);

            _connectionService
                .OnClientConnected
                .Subscribe(_ => ConnectPlayer())
                .AddTo(this);

            _connectionService
                .OnClientDisconnected
                .Subscribe(_ => DisconnectPlayer())
                .AddTo(this);
        }

        private void OnDestroy()
        {
            _signalBus.Unsubscribe<OnStartHostSignal>(StartHost);
            _signalBus.Unsubscribe<OnStartConnectSignal>(StartConnection);
        }

        private void StartHost(OnStartHostSignal signal)
        {
            _connectionService.Host(signal.Nickname);
        }

        private void StartConnection(OnStartConnectSignal signal)
        {
            _connectionService.Connect(signal.Nickname);
        }

        private void ConnectPlayer()
        {
            _signalBus.Fire(new OnPlayerConnectedSignal());
        }

        private void DisconnectPlayer()
        {
            _signalBus.Fire(new OnPlayerDisconnectedSignal());
        }
    }
}