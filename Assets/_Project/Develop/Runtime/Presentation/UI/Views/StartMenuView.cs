using UnityEngine;
using UnityEngine.UI;
using TMPro;
using R3;

namespace Presentation.UI.Views
{
    public class StartMenuView : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _nicknameInput;
        [SerializeField] private Button _hostBtn;
        [SerializeField] private Button _connectBtn;

        public Observable<string> OnHostBtnClicked => _onHostBtnClicked;
        public Observable<string> OnConnectBtnClicked => _onConnectBtnClicked;

        private Subject<string> _onHostBtnClicked = new();
        private Subject<string> _onConnectBtnClicked = new();

        private void Awake()
        {
            _hostBtn
                .OnClickAsObservable()
                .Subscribe(_ => HostBtnClick())
                .AddTo(this);

            _connectBtn
                .OnClickAsObservable()
                .Subscribe(_ => ConnectBtnClick())
                .AddTo(this);
        }

        private void HostBtnClick()
        {
            _onHostBtnClicked.OnNext(CheckPlayerNickname(_nicknameInput.text));
        }

        private void ConnectBtnClick()
        {
            _onConnectBtnClicked.OnNext(CheckPlayerNickname(_nicknameInput.text));
        }

        private string CheckPlayerNickname(string nickname)
        {
            return string.IsNullOrWhiteSpace(nickname)
                ? $"Player_{Random.Range(1000, 9999)}"
                : _nicknameInput.text;
        }
    }
}