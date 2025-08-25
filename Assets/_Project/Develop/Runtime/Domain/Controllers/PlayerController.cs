using Core.Interfaces;
using Domain.Input;
using Domain.Models;
using UnityEngine;
using Zenject;
using R3;
using Presentation.Player.Views;

namespace Domain.Controllers
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Transform _spawnNetObjPoint;
        [SerializeField] private PlayerView _view;

        private CompositeDisposable _controlsStream = new CompositeDisposable();

        private PlayerModel _model;
        private PlayerInputHandler _input;
        private INetworkEntity _netEntity;
        private IPlayerService _playerNetService;

        [Inject]
        private void Construct
        (
            PlayerModel playerModel,
            PlayerInputHandler playerInput,
            INetworkEntity networkEntity,
            IPlayerService playerService
        )
        {
            _model = playerModel;
            _input = playerInput;
            _netEntity = networkEntity;
            _playerNetService = playerService;
        }

        private void Awake()
        {
            _netEntity.OnLocalPlayerStarted
                .Subscribe(_ => RegisterControls())
                .AddTo(this);

            _netEntity.OnLocalPlayerStopped
                .Subscribe(_ => UnregisterControls())
                .AddTo(this);

            _playerNetService.OnNicknameChanged
                .Subscribe(SetNickname)
                .AddTo(this);

            _view.Init(_model.PlayerGravity);
        }

        private void OnDestroy()
        {
            UnregisterControls();
        }

        private void RegisterControls()
        {
            _input.OnMoveInput
                    .Subscribe(MovePlayer)
                    .AddTo(_controlsStream);

            _input.OnSendMessagePressed
                .Subscribe(_ => SendNickMessage())
                .AddTo(_controlsStream);

            _input.OnSpawnNetObjPressed
                .Subscribe(_ => SpawnNetObj())
                .AddTo(_controlsStream);
        }

        private void UnregisterControls()
        {
            _controlsStream.Clear();
        }

        private void MovePlayer(Vector2 moveInput)
        {
            var playerSpeed = moveInput * _model.PlayerSpeed;
            var worldSpeed = new Vector3(playerSpeed.x, 0f, playerSpeed.y);
            _view.SetPlayerSpeed(worldSpeed);
        }

        private void SetNickname(string nick)
        {
            _model.SetNickname(nick);
            _view.SetNickname(nick);
        }

        private void SendNickMessage()
        {
            _playerNetService.SendNickMessage();
        }

        private void SpawnNetObj()
        {
            _playerNetService.SpawnNetObj(_model.SpawnableNetObjId, _spawnNetObjPoint.position);
        }
    }
}