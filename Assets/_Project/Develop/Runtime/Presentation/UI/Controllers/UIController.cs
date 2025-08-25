using Core.Signals;
using UnityEngine;
using Zenject;
using TSS;
using R3;
using Presentation.UI.Views;

namespace Presentation.UI.Controllers
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private TSSCore _tssCore;
        [SerializeField] private StartMenuView _startMenuView;

        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        private void Awake()
        {
            _signalBus.Subscribe<OnPlayerConnectedSignal>(CloseMenu);
            _signalBus.Subscribe<OnPlayerDisconnectedSignal>(OpenMenu);

            _startMenuView.OnHostBtnClicked
                .Subscribe(StartHostSession)
                .AddTo(this);

            _startMenuView.OnConnectBtnClicked
                .Subscribe(StartConnection)
                .AddTo(this);
        }

        private void OnDestroy()
        {
            _signalBus.Unsubscribe<OnPlayerConnectedSignal>(CloseMenu);
            _signalBus.Unsubscribe<OnPlayerDisconnectedSignal>(OpenMenu);
        }

        private void OpenMenu()
        {
            _tssCore.SelectState("StartMenu");
        }

        private void CloseMenu()
        {
            _tssCore.CloseAll();
        }

        private void StartHostSession(string nickname)
        {
            _signalBus.Fire(new OnStartHostSignal(nickname));
        }

        private void StartConnection(string nickname)
        {
            _signalBus.Fire(new OnStartConnectSignal(nickname));
        }
    }
}